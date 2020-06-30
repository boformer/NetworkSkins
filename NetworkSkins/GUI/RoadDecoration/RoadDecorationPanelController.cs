using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NetworkSkins.GUI.Abstraction;
using NetworkSkins.Net;
using NetworkSkins.Persistence;
using NetworkSkins.Skins;
using NetworkSkins.Skins.Modifiers;

namespace NetworkSkins.GUI.RoadDecoration
{
    public class RoadDecorationPanelController : FeaturePanelController
    {
        public override bool Enabled => base.Enabled && (CanHideNodeMarkings || HasArrows || HasSigns 
            || HasDecoration || HasTransportStops || HasTrafficLights);

        public bool NodeMarkingsHidden { get; private set; }
        public bool ArrowsHidden { get; private set; }
        public bool SignsHidden { get; private set; }
        public bool DecorationHidden { get; private set; }
        public bool TransportStopsHidden { get; private set; }
        public bool TrafficLightsHidden { get; private set; }

        public bool CanHideNodeMarkings { get; private set; } = false;
        public bool HasArrows { get; private set; } = false;
        public bool HasSigns { get; private set; } = false;
        public bool HasDecoration { get; private set; } = false;
        public bool HasTransportStops { get; private set; } = false;
        public bool HasTrafficLights { get; private set; } = false;

        private MethodInfo _canHideMarkings;

        public RoadDecorationPanelController()
        {
            _canHideMarkings = FindCanHideMarkingsMethod();
        }

        private static MethodInfo FindCanHideMarkingsMethod()
        {
            try
            {
                var assembly = Assembly.Load("HideCrosswalks");
                if (assembly == null) return null;

                var method = assembly.GetType("HideCrosswalks.NetInfoExt")?
                    .GetMethod("GetCanHideMarkings", BindingFlags.Static | BindingFlags.Public); 
                if (method != null) return method;

                return assembly.GetType("HideTMPECrosswalks.Utils.PrefabUtils")?
                    .GetMethod("CanHideMarkings", BindingFlags.Static | BindingFlags.Public);
            }
            catch
            {
                return null;
            }
        }

        public void SetNodeMarkingsHidden(bool nodeMarkingsHidden)
        {
            if (NodeMarkingsHidden == nodeMarkingsHidden) return;

            NodeMarkingsHidden = nodeMarkingsHidden;

            Save();

            OnChanged();
        }

        public void SetArrowsHidden(bool arrowsHidden) {
            if (ArrowsHidden == arrowsHidden) return;

            ArrowsHidden = arrowsHidden;

            Save();

            OnChanged();
        }

        public void SetSignsHidden(bool signsHidden) {
            if (SignsHidden == signsHidden) return;

            SignsHidden = signsHidden;

            Save();

            OnChanged();
        }

        public void SetDecorationHidden(bool decorationHidden) {
            if (DecorationHidden == decorationHidden) return;

            DecorationHidden = decorationHidden;

            Save();

            OnChanged();
        }

        public void SetTransportStopsHidden(bool transportStopsHidden) {
            if (TransportStopsHidden == transportStopsHidden) return;

            TransportStopsHidden = transportStopsHidden;

            Save();

            OnChanged();
        }

        public void SetTrafficLightsHidden(bool trafficLightsHidden) {
            if (TrafficLightsHidden == trafficLightsHidden) return;

            TrafficLightsHidden = trafficLightsHidden;

            Save();

            OnChanged();
        }

        public override void Reset()
        {
            if (!Enabled || !NodeMarkingsHidden) return;

            NodeMarkingsHidden = false;
            ArrowsHidden = false;
            SignsHidden = false;
            DecorationHidden = false;
            TransportStopsHidden = false;
            TrafficLightsHidden = false;

            Save();

            OnChanged();
        }

        protected override void Build() {
            Refresh();
            
            Load();
        }

        protected override void BuildWithModifiers(List<NetworkSkinModifier> modifiers)
        {
            Refresh();

            NodeMarkingsHidden = false;
            ArrowsHidden = false;
            SignsHidden = false;
            DecorationHidden = false;
            TransportStopsHidden = false;
            TrafficLightsHidden = false;

            foreach (var modifier in modifiers)
            {
                if (modifier is RoadDecorationModifier roadDecorationModifier)
                {
                    NodeMarkingsHidden = roadDecorationModifier.nodeMarkingsHidden;
                    ArrowsHidden = roadDecorationModifier.arrowsHidden;
                    SignsHidden = roadDecorationModifier.signsHidden;
                    DecorationHidden = roadDecorationModifier.decorationHidden;
                    TransportStopsHidden = roadDecorationModifier.transportStopsHidden;
                    TrafficLightsHidden = roadDecorationModifier.trafficLightsHidden;
                    break;
                }
            }
        }

        private bool CanHideMarkings(NetInfo prefab)
        {
            return _canHideMarkings != null && (bool)_canHideMarkings.Invoke(null, new object[] { prefab });
        }

        private void Refresh()
        {
            CanHideNodeMarkings = CanHideMarkings(Prefab);
            var variations = NetUtils.GetPrefabVariations(Prefab);
            HasArrows = variations.Any(prefab => NetUtils.GetMatchingLaneProp(prefab, laneProp => RoadDecorationUtils.IsArrow(laneProp.m_finalProp)) != null);
            HasSigns = variations.Any(prefab => NetUtils.GetMatchingLaneProp(prefab, laneProp => RoadDecorationUtils.IsSign(laneProp.m_finalProp)) != null);
            HasDecoration = variations.Any(prefab => NetUtils.GetMatchingLaneProp(prefab, laneProp => RoadDecorationUtils.IsDecoration(laneProp.m_finalProp)) != null);
            HasTransportStops = variations.Any(prefab => NetUtils.GetMatchingLaneProp(prefab, laneProp => RoadDecorationUtils.IsTransportStop(laneProp.m_finalProp)) != null);
            HasTrafficLights = variations.Any(prefab => NetUtils.GetMatchingLaneProp(prefab, laneProp => RoadDecorationUtils.IsTrafficLight(laneProp.m_finalProp)) != null);
        }

        protected override Dictionary<NetInfo, List<NetworkSkinModifier>> BuildModifiers()
        {
            var modifiers = new Dictionary<NetInfo, List<NetworkSkinModifier>>();

            if (NodeMarkingsHidden || ArrowsHidden || SignsHidden || DecorationHidden || TransportStopsHidden || TrafficLightsHidden)
            {
                var prefabModifiers = new List<NetworkSkinModifier>
                {
                    new RoadDecorationModifier(NodeMarkingsHidden, ArrowsHidden, SignsHidden, DecorationHidden, TransportStopsHidden, TrafficLightsHidden)
                };

                var subPrefabs = NetUtils.GetPrefabVariations(Prefab);
                foreach (var subPrefab in subPrefabs)
                {
                    modifiers[subPrefab] = prefabModifiers;
                    /*if (NetTextureUtils.HasRoadTexture(subPrefab))
                    {
                        modifiers[subPrefab] = prefabModifiers;
                    }*/
                }
            }

            return modifiers;
        }

        #region Active Selection Data
        private const string NodeMarkingsHiddenKey = "NodeMarkingsHidden";
        private const string ArrowsHiddenKey = "ArrowsHidden";
        private const string SignsHiddenKey = "SignsHidden";
        private const string DecorationHiddenKey = "DecorationHidden";
        private const string TransportStopsHiddenKey = "TransportStopsHidden";
        private const string TrafficLightsHiddenKey = "TrafficLightsHidden";

        private void Load()
        {
            NodeMarkingsHidden = ActiveSelectionData.Instance.GetBoolValue(Prefab, NodeMarkingsHiddenKey) ?? false;
            ArrowsHidden = ActiveSelectionData.Instance.GetBoolValue(Prefab, ArrowsHiddenKey) ?? false;
            SignsHidden = ActiveSelectionData.Instance.GetBoolValue(Prefab, SignsHiddenKey) ?? false;
            DecorationHidden = ActiveSelectionData.Instance.GetBoolValue(Prefab, DecorationHiddenKey) ?? false;
            TransportStopsHidden = ActiveSelectionData.Instance.GetBoolValue(Prefab, TransportStopsHiddenKey) ?? false;
            TrafficLightsHidden = ActiveSelectionData.Instance.GetBoolValue(Prefab, TrafficLightsHiddenKey) ?? false;
        }

        private void Save()
        {
            if (NodeMarkingsHidden)
                ActiveSelectionData.Instance.SetBoolValue(Prefab, NodeMarkingsHiddenKey, true);
            else
                ActiveSelectionData.Instance.ClearValue(Prefab, NodeMarkingsHiddenKey);

            if (ArrowsHidden)
                ActiveSelectionData.Instance.SetBoolValue(Prefab, ArrowsHiddenKey, true);
            else
                ActiveSelectionData.Instance.ClearValue(Prefab, ArrowsHiddenKey);

            if (SignsHidden)
                ActiveSelectionData.Instance.SetBoolValue(Prefab, SignsHiddenKey, true);
            else
                ActiveSelectionData.Instance.ClearValue(Prefab, SignsHiddenKey);

            if (DecorationHidden)
                ActiveSelectionData.Instance.SetBoolValue(Prefab, DecorationHiddenKey, true);
            else
                ActiveSelectionData.Instance.ClearValue(Prefab, DecorationHiddenKey);

            if (TransportStopsHidden)
                ActiveSelectionData.Instance.SetBoolValue(Prefab, TransportStopsHiddenKey, true);
            else
                ActiveSelectionData.Instance.ClearValue(Prefab, TransportStopsHiddenKey);

            if (TrafficLightsHidden)
                ActiveSelectionData.Instance.SetBoolValue(Prefab, TrafficLightsHiddenKey, true);
            else
                ActiveSelectionData.Instance.ClearValue(Prefab, TrafficLightsHiddenKey);
        }
        #endregion
    }
}

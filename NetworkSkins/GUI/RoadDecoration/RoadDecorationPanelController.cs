using System.Collections.Generic;
using NetworkSkins.GUI.Abstraction;
using NetworkSkins.Net;
using NetworkSkins.Persistence;
using NetworkSkins.Skins;
using NetworkSkins.Skins.Modifiers;

namespace NetworkSkins.GUI.RoadDecoration
{
    public class RoadDecorationPanelController : FeaturePanelController
    {
        public override bool Enabled => base.Enabled && _canHideNodeMarkings;

        public bool NodeMarkingsHidden { get; private set; }

        public bool _canHideNodeMarkings = false;


        public void SetNodeMarkingsHidden(bool nodeMarkingsHidden)
        {
            if (NodeMarkingsHidden == nodeMarkingsHidden) return;

            NodeMarkingsHidden = nodeMarkingsHidden;

            SaveNodeMarkingsHidden();

            OnChanged();
        }

        public override void Reset()
        {
            if (!Enabled || !NodeMarkingsHidden) return;

            NodeMarkingsHidden = false;

            SaveNodeMarkingsHidden();

            OnChanged();
        }


        protected override void Build()
        {
            RefreshCanHideNodeMarkings();

            NodeMarkingsHidden = LoadNodeMarkingsHidden() ?? false;
        }

        protected override void BuildWithModifiers(List<NetworkSkinModifier> modifiers)
        {
            RefreshCanHideNodeMarkings();

            NodeMarkingsHidden = false;

            foreach (var modifier in modifiers)
            {
                if (modifier is RoadDecorationModifier roadDecorationModifier)
                {
                    NodeMarkingsHidden = roadDecorationModifier.nodeMarkingsHidden;
                    break;
                }
            }
        }

        private void RefreshCanHideNodeMarkings()
        {
            _canHideNodeMarkings = Prefab.m_netAI is RoadBaseAI && Prefab.m_hasPedestrianLanes && Prefab.m_hasForwardVehicleLanes;
        }

        protected override Dictionary<NetInfo, List<NetworkSkinModifier>> BuildModifiers()
        {
            var modifiers = new Dictionary<NetInfo, List<NetworkSkinModifier>>();

            if (NodeMarkingsHidden)
            {
                var prefabModifiers = new List<NetworkSkinModifier>
                {
                    new RoadDecorationModifier(NodeMarkingsHidden)
                };

                var subPrefabs = NetUtils.GetPrefabVariations(Prefab);
                foreach (var subPrefab in subPrefabs)
                {
                    if (NetTextureUtils.HasRoadTexture(subPrefab))
                    {
                        modifiers[subPrefab] = prefabModifiers;
                    }
                }
            }

            return modifiers;
        }

        #region Active Selection Data
        private const string NodeMarkingsHiddenKey = "NodeMarkingsHidden";

        private bool? LoadNodeMarkingsHidden()
        {
            return ActiveSelectionData.Instance.GetBoolValue(Prefab, NodeMarkingsHiddenKey);
        }

        private void SaveNodeMarkingsHidden()
        {
            if (NodeMarkingsHidden)
            {
                ActiveSelectionData.Instance.SetBoolValue(Prefab, NodeMarkingsHiddenKey, true);
            }
            else
            {
                ActiveSelectionData.Instance.ClearValue(Prefab, NodeMarkingsHiddenKey);
            }
        }
        #endregion
    }
}

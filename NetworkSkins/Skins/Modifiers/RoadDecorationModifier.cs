using ColossalFramework.IO;
using NetworkSkins.Net;

namespace NetworkSkins.Skins.Modifiers
{
    public class RoadDecorationModifier : NetworkSkinModifier
    {
        public readonly bool nodeMarkingsHidden;
        public readonly bool arrowsHidden;
        public readonly bool signsHidden;
        public readonly bool decorationHidden;
        public readonly bool transportStopsHidden;
        public readonly bool trafficLightsHidden;

        public RoadDecorationModifier(
            bool nodeMarkingsHidden, 
            bool arrowsHidden, 
            bool signsHidden, 
            bool decorationHidden, 
            bool transportStopsHidden, 
            bool trafficLightsHidden) 
            : base(NetworkSkinModifierType.RoadDecoration) 
        {
            this.nodeMarkingsHidden = nodeMarkingsHidden;
            this.arrowsHidden = arrowsHidden;
            this.signsHidden = signsHidden;
            this.decorationHidden = decorationHidden;
            this.transportStopsHidden = transportStopsHidden;
            this.trafficLightsHidden = trafficLightsHidden;
        }

        public override void Apply(NetworkSkin skin)
        {
            skin.m_nodeMarkingsHidden = nodeMarkingsHidden;

            if (skin.m_lanes != null) {
                for (var l = 0; l < skin.m_lanes.Length; l++) {
                    var laneProps = skin.m_lanes[l]?.m_laneProps?.m_props;
                    if (laneProps == null) continue;

                    for (var p = skin.m_lanes[l].m_laneProps.m_props.Length - 1; p >= 0; p--) {
                        if (arrowsHidden && RoadDecorationUtils.IsArrow(laneProps[p]?.m_finalProp)
                            || signsHidden && RoadDecorationUtils.IsSign(laneProps[p]?.m_finalProp)
                            || decorationHidden && RoadDecorationUtils.IsDecoration(laneProps[p]?.m_finalProp)
                            || transportStopsHidden && RoadDecorationUtils.IsTransportStop(laneProps[p]?.m_finalProp) 
                            || trafficLightsHidden && RoadDecorationUtils.IsTrafficLight(laneProps[p]?.m_finalProp)) {
                            skin.RemoveLaneProp(l, p);
                        }
                    }
                }
            }
        }

        #region Serialization
        protected override void SerializeImpl(DataSerializer s)
        {
            s.WriteBool(nodeMarkingsHidden);
            s.WriteBool(arrowsHidden);
            s.WriteBool(signsHidden);
            s.WriteBool(decorationHidden);
            s.WriteBool(transportStopsHidden);
            s.WriteBool(trafficLightsHidden);
        }

        public static RoadDecorationModifier DeserializeImpl(DataSerializer s)
        {
            var nodeMarkingsHidden = s.ReadBool();

            if (s.version < 1) {
                return new RoadDecorationModifier(nodeMarkingsHidden, false, false, false, false, false);
            }

            var arrowsHidden = s.ReadBool();
            var signsHidden = s.ReadBool();
            var decorationHidden = s.ReadBool();
            var transportStopsHidden = s.ReadBool();
            var trafficLightsHidden = s.ReadBool();
            return new RoadDecorationModifier(nodeMarkingsHidden, arrowsHidden, signsHidden, decorationHidden, transportStopsHidden, trafficLightsHidden);
        }
        #endregion

        #region Equality
        protected bool Equals(RoadDecorationModifier other)
        {
            return nodeMarkingsHidden == other.nodeMarkingsHidden;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != GetType())
            {
                return false;
            }

            return Equals((RoadDecorationModifier)obj);
        }

        public override int GetHashCode()
        {
            return 1034881573 + nodeMarkingsHidden.GetHashCode();
        }
        #endregion
    }
}

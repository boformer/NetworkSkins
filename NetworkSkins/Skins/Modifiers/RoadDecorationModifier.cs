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
        public readonly bool levelCrossingsHidden;

        public RoadDecorationModifier(
            bool nodeMarkingsHidden,
            bool arrowsHidden,
            bool signsHidden,
            bool decorationHidden,
            bool transportStopsHidden,
            bool trafficLightsHidden,
            bool levelCrossingsHidden)
            : base(NetworkSkinModifierType.RoadDecoration)
        {
            this.nodeMarkingsHidden = nodeMarkingsHidden;
            this.arrowsHidden = arrowsHidden;
            this.signsHidden = signsHidden;
            this.decorationHidden = decorationHidden;
            this.transportStopsHidden = transportStopsHidden;
            this.trafficLightsHidden = trafficLightsHidden;
            this.levelCrossingsHidden = levelCrossingsHidden;
        }

        public override void Apply(NetworkSkin skin)
        {
            skin.m_nodeMarkingsHidden = nodeMarkingsHidden;

            if (skin.m_lanes != null)
            {
                for (var l = 0; l < skin.m_lanes.Length; l++)
                {
                    var laneProps = skin.m_lanes[l]?.m_laneProps?.m_props;
                    if (laneProps == null) continue;

                    for (var p = skin.m_lanes[l].m_laneProps.m_props.Length - 1; p >= 0; p--)
                    {
                        if (arrowsHidden && RoadDecorationUtils.IsArrow(laneProps[p]?.m_finalProp)
                            || signsHidden && RoadDecorationUtils.IsSign(laneProps[p]?.m_finalProp)
                            || decorationHidden && RoadDecorationUtils.IsDecoration(laneProps[p]?.m_finalProp)
                            || transportStopsHidden && RoadDecorationUtils.IsTransportStop(laneProps[p])
                            || (trafficLightsHidden && RoadDecorationUtils.IsTrafficLight(laneProps[p]))
                            || levelCrossingsHidden && RoadDecorationUtils.IsLevelCrossing(laneProps[p]))
                        {
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
            s.WriteBool(levelCrossingsHidden);
        }

        public static RoadDecorationModifier DeserializeImpl(DataSerializer s)
        {
            var nodeMarkingsHidden = s.ReadBool();

            if (s.version < 1)
            {
                return new RoadDecorationModifier(nodeMarkingsHidden, false, false, false, false, false, false);
            }


            var arrowsHidden = s.ReadBool();
            var signsHidden = s.ReadBool();
            var decorationHidden = s.ReadBool();
            var transportStopsHidden = s.ReadBool();
            var trafficLightsHidden = s.ReadBool();

            if (s.version < 2)
            {
                return new RoadDecorationModifier(nodeMarkingsHidden, arrowsHidden, signsHidden, decorationHidden, transportStopsHidden, trafficLightsHidden, false);
            }

            var levelCrossingsHidden = s.ReadBool();

            return new RoadDecorationModifier(nodeMarkingsHidden, arrowsHidden, signsHidden, decorationHidden, transportStopsHidden, trafficLightsHidden, levelCrossingsHidden);
        }
        #endregion

        #region Equality
        protected bool Equals(RoadDecorationModifier other)
        {
            return nodeMarkingsHidden == other.nodeMarkingsHidden && arrowsHidden == other.arrowsHidden && signsHidden == other.signsHidden && decorationHidden == other.decorationHidden && transportStopsHidden == other.transportStopsHidden && trafficLightsHidden == other.trafficLightsHidden && levelCrossingsHidden == other.levelCrossingsHidden;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((RoadDecorationModifier)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = nodeMarkingsHidden.GetHashCode();
                hashCode = (hashCode * 397) ^ arrowsHidden.GetHashCode();
                hashCode = (hashCode * 397) ^ signsHidden.GetHashCode();
                hashCode = (hashCode * 397) ^ decorationHidden.GetHashCode();
                hashCode = (hashCode * 397) ^ transportStopsHidden.GetHashCode();
                hashCode = (hashCode * 397) ^ trafficLightsHidden.GetHashCode();
                hashCode = (hashCode * 397) ^ levelCrossingsHidden.GetHashCode();
                return hashCode;
            }
        }
        #endregion
    }
}
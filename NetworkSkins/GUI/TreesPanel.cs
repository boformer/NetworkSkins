using ColossalFramework.UI;
using NetworkSkins.Net;

namespace NetworkSkins.GUI
{
    public class TreesPanel : PanelBase
    {
        private UITabstrip tabStrip;

        public override void Build(Layout layout) {
            base.Build(layout);

        }

        public bool HasTrees(NetInfo prefab, LanePosition position) {
            if (prefab.m_lanes == null) return false;

            foreach (var lane in prefab.m_lanes)
                if (lane?.m_laneProps?.m_props != null && position.IsCorrectSide(lane.m_position))
                    foreach (var laneProp in lane.m_laneProps.m_props) {
                        if (laneProp?.m_finalTree != null) return true;
                    }
            return false;
        }

        public TreeInfo GetDefaultTree(NetInfo prefab, LanePosition position) {
            if (prefab.m_lanes == null) return null;

            foreach (var lane in prefab.m_lanes)
                if (lane?.m_laneProps?.m_props != null && position.IsCorrectSide(lane.m_position))
                    foreach (var laneProp in lane.m_laneProps.m_props) {
                        if (laneProp?.m_finalTree != null) return laneProp.m_finalTree;
                    }

            return null;
        }
    }
}

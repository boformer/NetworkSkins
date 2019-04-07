namespace NetworkSkins.Net
{
    public static class TreeUtils
    {
        public static TreeInfo GetDefaultTree(NetInfo prefab, LanePosition position)
        {
            if (prefab?.m_lanes != null)
            {
                foreach (var lane in prefab.m_lanes)
                {
                    if (lane?.m_laneProps?.m_props == null || !position.IsCorrectSide(lane.m_position)) continue;

                    foreach (var laneProp in lane.m_laneProps.m_props)
                    {
                        if (laneProp?.m_finalTree != null)
                        {
                            return laneProp.m_finalTree;
                        }
                    }
                }
            }
 
            return null;
        }

        public static float GetDefaultRepeatDistance(NetInfo prefab, LanePosition position)
        {
            if (prefab?.m_lanes != null)
            {
                foreach (var lane in prefab.m_lanes)
                {
                    if (lane?.m_laneProps?.m_props == null || !position.IsCorrectSide(lane.m_position)) continue;

                    foreach (var laneProp in lane.m_laneProps.m_props)
                    {
                        if (laneProp?.m_finalTree != null)
                        {
                            return laneProp.m_repeatDistance;
                        }
                    }
                }
            }
            
            return 20f;
        }
    }
}

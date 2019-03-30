using System;

namespace NetworkSkins.Skins
{
    public class SimpleTreeModifier : NetworkSkinModifier
    {
        public readonly TreeInfo Tree;

        public SimpleTreeModifier(TreeInfo tree)
        {
            Tree = tree;
        }

        public override void Apply(NetworkSkin skin)
        {
            if (skin.m_lanes == null) return;

            for (var l = 0; l < skin.m_lanes.Length; l++)
            {
                var laneProps = skin.m_lanes[l]?.m_laneProps?.m_props;
                if (laneProps == null) continue;

                for (var p = 0; p < laneProps.Length; p++)
                {
                    if (laneProps[p]?.m_tree != null || laneProps[p]?.m_tree != null)
                    {
                        skin.UpdateLaneProp(l, p, laneProp =>
                        {
                            laneProp.m_tree = Tree;
                            laneProp.m_finalTree = Tree;
                        });
                    }
                }
            }
        }
    }
}

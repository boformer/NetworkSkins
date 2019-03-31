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

        #region Equality
        protected bool Equals(SimpleTreeModifier other)
        {
            return Equals(Tree, other.Tree);
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

            if (obj.GetType() != this.GetType())
            {
                return false;
            }

            return Equals((SimpleTreeModifier) obj);
        }

        public override int GetHashCode()
        {
            return (Tree != null ? Tree.GetHashCode() : 0);
        }
        #endregion
    }
}

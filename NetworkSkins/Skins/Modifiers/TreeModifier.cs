using ColossalFramework.IO;
using NetworkSkins.Net;
using NetworkSkins.Skins.Serialization;

namespace NetworkSkins.Skins.Modifiers
{
    public class TreeModifier : NetworkSkinModifier
    {
        public readonly LanePosition Position;

        public readonly TreeInfo Tree;
        public readonly float RepeatDistance;

        public TreeModifier(LanePosition position, TreeInfo tree = null, float repeatDistance = 20) : base(NetworkSkinModifierType.Tree)
        {
            Position = position;
            Tree = tree;
            RepeatDistance = repeatDistance;
        }

        public override void Apply(NetworkSkin skin)
        {
            if (skin.m_lanes == null) return;

            for (var l = 0; l < skin.m_lanes.Length; l++)
            {
                if(!Position.IsCorrectSide(skin.m_lanes[l].m_position)) continue;

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
                            laneProp.m_repeatDistance = RepeatDistance;
                        });
                    }
                }
            }
        }



        #region Serialization
        protected override void SerializeImpl(DataSerializer s)
        {
            s.WriteInt32((int)Position);
            s.WriteUniqueString(Tree?.name);
            s.WriteFloat(RepeatDistance);
        }

        public static TreeModifier DeserializeImpl(DataSerializer s, IPrefabCollection prefabCollection, NetworkSkinLoadErrors errors)
        {
            var position = (LanePosition) s.ReadInt32();
            var tree = prefabCollection.FindPrefab<TreeInfo>(s.ReadUniqueString(), errors);
            var repeatDistance = s.ReadFloat();

            return new TreeModifier(position, tree, repeatDistance);
        }
        #endregion

        #region Equality

        protected bool Equals(TreeModifier other)
        {
            return Position == other.Position && Equals(Tree, other.Tree) && RepeatDistance.Equals(other.RepeatDistance);
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

            return Equals((TreeModifier) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (int) Position;
                hashCode = (hashCode * 397) ^ (Tree != null ? Tree.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ RepeatDistance.GetHashCode();
                return hashCode;
            }
        }
        #endregion
    }
}

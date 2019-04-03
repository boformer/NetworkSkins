using ColossalFramework.IO;

namespace NetworkSkins.Skins
{
    public class TreeModifier : NetworkSkinModifier
    {
        public readonly TreeInfo LeftTree;
        public readonly float LeftTreeRepeatDistance;

        public readonly TreeInfo MiddleTree;
        public readonly float MiddleTreeRepeatDistance;

        public readonly TreeInfo RightTree;
        public readonly float RighTreeRepeatDistance;

        public TreeModifier(
            TreeInfo leftTree = null, float leftTreeRepeatDistance = 20,
            TreeInfo middleTree = null, float middleTreeRepeatDistance = 20,
            TreeInfo rightTree = null, float righTreeRepeatDistance = 20
            ) : base(NetworkSkinModifierType.Tree)
        {
            LeftTree = leftTree;
            LeftTreeRepeatDistance = leftTreeRepeatDistance;
            MiddleTree = middleTree;
            MiddleTreeRepeatDistance = middleTreeRepeatDistance;
            RightTree = rightTree;
            RighTreeRepeatDistance = righTreeRepeatDistance;
        }

        public TreeModifier(TreeInfo tree = null, float repeatDistance = 20) : base(NetworkSkinModifierType.Tree)
        {
            LeftTree = tree;
            LeftTreeRepeatDistance = repeatDistance;
            MiddleTree = tree;
            MiddleTreeRepeatDistance = repeatDistance;
            RightTree = tree;
            RighTreeRepeatDistance = repeatDistance;
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
                        var tree = GetTree(skin.m_lanes[l].m_position);
                        var repeatDistance = GetRepeatDistance(skin.m_lanes[l].m_position);
                        skin.UpdateLaneProp(l, p, laneProp =>
                        {
                            laneProp.m_tree = tree;
                            laneProp.m_finalTree = tree;
                            laneProp.m_repeatDistance = repeatDistance;
                        });
                    }
                }
            }
        }

        private TreeInfo GetTree(float lanePosition)
        {
            if (lanePosition < -0.5f)
            {
                return LeftTree;
            }
            else if (lanePosition > 0.5f)
            {
                return RightTree;
            }
            else
            {
                return MiddleTree;
            }
        }

        private float GetRepeatDistance(float lanePosition)
        {
            if (lanePosition < -0.5f)
            {
                return LeftTreeRepeatDistance;
            }
            else if (lanePosition > 0.5f)
            {
                return RighTreeRepeatDistance;
            }
            else
            {
                return MiddleTreeRepeatDistance;
            }
        }

        #region Serialization
        protected override void SerializeImpl(DataSerializer s)
        {
            s.WriteUniqueString(LeftTree?.name);
            s.WriteFloat(LeftTreeRepeatDistance);

            s.WriteUniqueString(MiddleTree?.name);
            s.WriteFloat(MiddleTreeRepeatDistance);

            s.WriteUniqueString(RightTree?.name);
            s.WriteFloat(RighTreeRepeatDistance);
        }

        public static TreeModifier DeserializeImpl(DataSerializer s, NetworkSkinLoadErrors errors)
        {
            var leftTree = NetworkSkinSerializationUtils.FindPrefab<TreeInfo>(s.ReadUniqueString(), errors);
            var leftTreeRepeatDistance = s.ReadFloat();

            var middleTree = NetworkSkinSerializationUtils.FindPrefab<TreeInfo>(s.ReadUniqueString(), errors);
            var middleTreeRepeatDistance = s.ReadFloat();

            var rightTree = NetworkSkinSerializationUtils.FindPrefab<TreeInfo>(s.ReadUniqueString(), errors);
            var rightTreeRepeatDistance = s.ReadFloat();

            return new TreeModifier(
                leftTree, leftTreeRepeatDistance,
                middleTree, middleTreeRepeatDistance,
                rightTree, rightTreeRepeatDistance
            );
        }
        #endregion

        #region Equality
        protected bool Equals(TreeModifier other)
        {
            return Equals(LeftTree, other.LeftTree) && LeftTreeRepeatDistance.Equals(other.LeftTreeRepeatDistance)
                && Equals(MiddleTree, other.MiddleTree) && MiddleTreeRepeatDistance.Equals(other.MiddleTreeRepeatDistance)
                && Equals(RightTree, other.RightTree) && RighTreeRepeatDistance.Equals(other.RighTreeRepeatDistance);
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

            return Equals((TreeModifier)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (LeftTree != null ? LeftTree.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ LeftTreeRepeatDistance.GetHashCode();
                hashCode = (hashCode * 397) ^ (MiddleTree != null ? MiddleTree.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ MiddleTreeRepeatDistance.GetHashCode();
                hashCode = (hashCode * 397) ^ (RightTree != null ? RightTree.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ RighTreeRepeatDistance.GetHashCode();
                return hashCode;
            }
        }
        #endregion
    }
}

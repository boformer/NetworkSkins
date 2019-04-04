using NetworkSkins.Skins.Serialization;
using UnityEngine;

namespace NetworkSkins.Tests
{
    public class TestNetManager : INetManager
    {
        public const ushort TestSegment = 1;
        public const ushort TestSegment2 = 42;
        public const ushort TestNode = 1000;
        public const ushort TestNode2 = 69;

        private readonly TestPrefabCollection _prefabs;

        public TestNetManager()
        {
            _prefabs = TestPrefabCollection.FindInstance();
        }

        public bool IsSegmentCreated(ushort segment)
        {
            TestUtils.LogTest($"IsSegmentCreated {segment}");
            return segment == TestSegment || segment == TestSegment2;
        }

        public NetInfo GetSegmentInfo(ushort segment)
        {
            if (segment == TestSegment || segment == TestSegment2)
            {
                return _prefabs.TestNet;
            }
            else
            {
                return null;
            }
        }

        public bool IsNodeCreated(ushort node)
        {
            TestUtils.LogTest($"IsNodeCreated {node}");
            return node == TestNode || node == TestNode2;
        }

        public NetInfo GetNodeInfo(ushort node)
        {
            if (node == TestNode || node == TestNode2)
            {
                return _prefabs.TestNet;
            }
            else
            {
                return null;
            }
        }
    }
}

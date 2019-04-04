using System.Collections.Generic;
using System.IO;
using System.Linq;
using ColossalFramework.IO;
using NetworkSkins.Skins;
using NetworkSkins.Skins.Modifiers;
using NetworkSkins.Skins.Serialization;
using UnityEngine;

namespace NetworkSkins.Tests
{
    public class SkinSerializationTest
    {
        public void TestSerialization()
        {
            var prefabs = TestPrefabCollection.CreateInstance();

            Debug.Log("prefabs found");

            var skin = new NetworkSkin(prefabs.TestNet, new List<NetworkSkinModifier>
            {
                new TerrainSurfaceModifier(NetworkGroundType.Ruined),
                new ColorModifier(new Color32(1, 2, 3, 4)),
                new StreetLightModifier(prefabs.TestProp, 42f),
                new TreeModifier(prefabs.TestTree, 42f, null, 1f, prefabs.TestTree2, 69f),
                new PillarModifier(prefabs.TestBuilding, null, null, null, prefabs.TestBuilding2),
                new CatenaryModifier(prefabs.TestProp),
            }) {UseCount = 2};

            Debug.Log("skin created");

            var dataContainer = new TestNetworkSkinDataContainer
            {
                TestAppliedSkins = new List<NetworkSkin> {skin},
                TestSegmentSkins = new NetworkSkin[NetManager.MAX_SEGMENT_COUNT],
                TestNodeSkins = new NetworkSkin[NetManager.MAX_NODE_COUNT]
            };
            dataContainer.TestSegmentSkins[TestNetManager.TestSegment] = skin;
            dataContainer.TestNodeSkins[TestNetManager.TestNode] = skin;

            Debug.Log("dataContainer created");

            // Serialize
            byte[] data;
            using (var stream = new MemoryStream())
            {
                DataSerializer.Serialize(stream, DataSerializer.Mode.Memory, NetworkSkinDataContainer.Version, dataContainer);
                data = stream.ToArray();
            }

            Debug.Log("dataContainer serialized");

            // Deserialize
            TestNetworkSkinDataContainer result;
            using (var stream = new MemoryStream(data))
            {
                result = DataSerializer.Deserialize<TestNetworkSkinDataContainer>(stream, DataSerializer.Mode.Memory);
            }

            Debug.Log("result.Errors.PrefabErrors.Count == 0");
            Assert.IsTrue(result.Errors.PrefabErrors.Count == 0);

            Debug.Log("result.TestAppliedSkins.Count == dataContainer.TestAppliedSkins.Count");
            Assert.IsTrue(result.TestAppliedSkins.Count == dataContainer.TestAppliedSkins.Count);

            var resultSkin = result.TestAppliedSkins[0];

            Debug.Log("resultSkin.UseCount == skin.UseCount");
            Assert.IsTrue(resultSkin.UseCount == skin.UseCount);

            Debug.Log("resultSkin.Prefab == skin.Prefab");
            Assert.IsTrue(resultSkin.Prefab == skin.Prefab);

            Debug.Log("resultSkin.Modifiers.SequenceEqual(skin.Modifiers)");
            Assert.IsTrue(resultSkin.Modifiers.SequenceEqual(skin.Modifiers));

            Debug.Log("result.TestNodeSkins.Count(s => s == resultSkin) == 1");
            Assert.IsTrue(result.TestSegmentSkins[TestNetManager.TestSegment] == resultSkin);

            Debug.Log("result.TestSegmentSkins.Count(s => s != null) == 1");
            Assert.IsTrue(result.TestSegmentSkins.Count(s => s != null) == 1);

            Debug.Log("result.TestNodeSkins.Count(s => s == resultSkin) == 1");
            Assert.IsTrue(result.TestNodeSkins[TestNetManager.TestNode] == resultSkin);

            Debug.Log("result.TestNodeSkins.Count(s => s != null) == 1");
            Assert.IsTrue(result.TestNodeSkins.Count(s => s != null) == 1);

            TestPrefabCollection.DestroyInstance();
        }
    }

    public class TestNetworkSkinDataContainer : NetworkSkinDataContainer
    {
        public List<NetworkSkin> TestAppliedSkins;

        public NetworkSkin[] TestSegmentSkins;

        public NetworkSkin[] TestNodeSkins;

        protected override void Initialize()
        {
            if (TestAppliedSkins == null)
            {
                TestAppliedSkins = new List<NetworkSkin>();
            }
            AppliedSkins = TestAppliedSkins;

            if (TestSegmentSkins == null)
            {
                TestSegmentSkins = new NetworkSkin[global::NetManager.MAX_SEGMENT_COUNT];
            }
            SegmentSkins = TestSegmentSkins;

            if (TestNodeSkins == null)
            {
                TestNodeSkins = new NetworkSkin[global::NetManager.MAX_NODE_COUNT];
            }
            NodeSkins = TestNodeSkins;

            PrefabCollection = TestPrefabCollection.FindInstance();
            NetManager = new TestNetManager();
        }
    }
}

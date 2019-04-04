using NetworkSkins.Skins.Serialization;
using UnityEngine;

namespace NetworkSkins.Tests
{
    public class TestPrefabCollection : MonoBehaviour, IPrefabCollection
    {
        private const string GoName = "TestPrefabCollection";

        public NetInfo TestNet;
        public BuildingInfo TestBuilding;
        public BuildingInfo TestBuilding2;
        public PropInfo TestProp;
        public TreeInfo TestTree;
        public TreeInfo TestTree2;

        public static TestPrefabCollection FindInstance()
        {
            return GameObject.Find(GoName).GetComponent<TestPrefabCollection>();
        }

        public static TestPrefabCollection CreateInstance()
        {
            return new GameObject(GoName).AddComponent<TestPrefabCollection>();
        }

        public static void DestroyInstance()
        {
            DestroyImmediate(GameObject.Find(GoName));
        }

        public void Awake()
        {
            TestNet = AddPrefab<NetInfo>("TestNet");
            TestBuilding = AddPrefab<BuildingInfo>("TestBuilding");
            TestBuilding2 = AddPrefab<BuildingInfo>("TestBuilding2");
            TestProp = AddPrefab<PropInfo>("TestProp");
            TestTree = AddPrefab<TreeInfo>("TestTree");
            TestTree2 = AddPrefab<TreeInfo>("TestTree2");
        }

        private T AddPrefab<T>(string prefabName) where T : PrefabInfo
        {
            var prefab = new GameObject().AddComponent<T>();
            prefab.gameObject.transform.parent = gameObject.transform;
            prefab.name = prefabName;
            return prefab;
        }

        public T FindPrefab<T>(string prefabName, NetworkSkinLoadErrors errors) where T : PrefabInfo
        {
            Debug.Log($"FindPrefab<{typeof(T)}> {prefabName}");

            if (typeof(T) == typeof(NetInfo) && prefabName == TestNet.name)
            {
                return TestNet as T;
            }
            else if (typeof(T) == typeof(BuildingInfo) && prefabName == TestBuilding.name)
            {
                return TestBuilding as T;
            }
            else if (typeof(T) == typeof(BuildingInfo) && prefabName == TestBuilding2.name)
            {
                return TestBuilding2 as T;
            }
            else if (typeof(T) == typeof(PropInfo) && prefabName == TestProp.name)
            {
                return TestProp as T;
            }
            else if (typeof(T) == typeof(TreeInfo) && prefabName == TestTree.name)
            {
                return TestTree as T;
            }
            else if (typeof(T) == typeof(TreeInfo) && prefabName == TestTree2.name)
            {
                return TestTree2 as T;
            }
            else
            {
                if (prefabName != null)
                {
                    errors.PrefabNotFound(prefabName);
                }
                return null;
            }
        }
    }
}
using ColossalFramework.IO;
using NetworkSkins.Skins.Modifiers;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace NetworkSkins.Skins.Serialization
{
    // Utility that can be used by external mods to serialize modifier data
    public static class ModifierDataSerializer
    {
        public static byte[] Serialize(List<NetworkSkinModifier> modifiers)
        {
            var dataContainer = new Data();
            dataContainer.Modifiers = modifiers;
        
            using (var stream = new MemoryStream())
            {
                DataSerializer.Serialize(stream, DataSerializer.Mode.Memory, NetworkSkinDataContainer.Version, dataContainer);
                return stream.ToArray();
            }
        }

        public static List<NetworkSkinModifier> Deserialize(byte[] data)
        {
            return Deserialize(data, out var errors);
        }


        public static List<NetworkSkinModifier> Deserialize(byte[] data, out NetworkSkinLoadErrors errors)
        {
            using (var stream = new MemoryStream(data))
            {
                var dataContainer = DataSerializer.Deserialize<Data>(stream, DataSerializer.Mode.Memory, NetworkSkinsMod.ResolveSerializedType);
                errors = dataContainer.Errors;
                return dataContainer.Modifiers;
            }
        }

        public class Data : IDataContainer
        {
            public List<NetworkSkinModifier> Modifiers;
            public NetworkSkinLoadErrors Errors;

            protected IPrefabCollection PrefabCollection;
            protected INetManager NetManager;

            // can be overridden for testing
            protected void Initialize()
            {
                Modifiers = new List<NetworkSkinModifier>();
                Errors = new NetworkSkinLoadErrors();

                PrefabCollection = new GamePrefabCollection();
                NetManager = new GameNetManager();
            }

            public void Serialize(DataSerializer s)
            {
                s.WriteInt32(Modifiers.Count);
                foreach (var modifier in Modifiers)
                {
                    modifier.Serialize(s);
                }
            }

            public void Deserialize(DataSerializer s)
            {
                Initialize();

                var modifiersCount = s.ReadInt32();
                for (var m = 0; m < modifiersCount; m++)
                {
                    var modifier = NetworkSkinModifier.Deserialize(s, PrefabCollection, Errors);
                    if (modifier != null)
                    {
                        Modifiers.Add(modifier);
                    }
                }
            }

            public void AfterDeserialize(DataSerializer s) { }
        }
    }

#if DEBUG
    public static class ModifierDataSerializerSerializerExample
    {
        public static void Run()
        {
            var modifiers1 = new List<NetworkSkinModifier> {
                new TreeModifier(Net.LanePosition.Left, PrefabCollection<TreeInfo>.GetLoaded(0)),
                new TreeModifier(Net.LanePosition.Right, PrefabCollection<TreeInfo>.GetLoaded(0)),
                new TerrainSurfaceModifier(Net.Surface.Gravel)
            };

            var bytes1 = ModifierDataSerializer.Serialize(modifiers1);

            var base64 = System.Convert.ToBase64String(bytes1);
            Debug.Log($"base64: {base64}");

            var bytes2 = System.Convert.FromBase64String(base64);

            NetworkSkinLoadErrors errors;
            var modifiers2 = ModifierDataSerializer.Deserialize(bytes2, out errors);

            errors.MaybeShowErrors();

            Debug.Log($"Equals: {modifiers1.SequenceEqual(modifiers2)}");
        }
    }
#endif
}

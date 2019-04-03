using ICities;
using NetworkSkins.Skins;

namespace NetworkSkins
{
    public class NetworkSkinsSerializableData : SerializableDataExtensionBase
    {
        public override void OnSaveData()
        {
            base.OnSaveData();
            NetworkSkinManager.instance.OnSaveData();
        }
    }
}

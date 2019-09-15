using ICities;
using NetworkSkins.Skins;

namespace NetworkSkins.Persistence
{
    public class NetworkSkinManagerData : SerializableDataExtensionBase
    {
        public override void OnSaveData()
        {
            base.OnSaveData();
            NetworkSkinManager.instance.OnSaveData();
        }
    }
}

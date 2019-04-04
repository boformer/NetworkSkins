namespace NetworkSkins.Skins.Serialization
{
    public interface IPrefabCollection
    {
        T FindPrefab<T>(string prefabName, NetworkSkinLoadErrors errors) where T : PrefabInfo;
    }
}

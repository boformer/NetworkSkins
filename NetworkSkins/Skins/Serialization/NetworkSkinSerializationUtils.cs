namespace NetworkSkins.Skins
{
    public static class NetworkSkinSerializationUtils
    {
        public static T FindPrefab<T>(string prefabName, NetworkSkinLoadErrors errors) where T : PrefabInfo
        {
            if (prefabName != null)
            {
                var prefab = PrefabCollection<T>.FindLoaded(prefabName);
                if (prefab == null)
                {
                    errors.PrefabNotFound(prefabName);
                }

                return prefab;
            }
            else
            {
                return null;
            }
        }
    }
}

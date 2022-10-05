namespace NetworkSkins.Helpers {
    using UnityEngine;
    public interface IPersistency {
        string GetValue(NetInfo prefab, string key);
        void SetValue(NetInfo prefab, string key, string value);
        float? GetFloatValue(NetInfo prefab, string key);
        void SetFloatValue(NetInfo prefab, string key, float value);
        Color? GetColorValue(NetInfo prefab, string key);
        void SetColorValue(NetInfo prefab, string key, Color value);
        bool? GetBoolValue(NetInfo prefab, string key);
        void SetBoolValue(NetInfo prefab, string key, bool value);
        void ClearValue(NetInfo prefab, string key);
    }
}

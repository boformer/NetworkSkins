namespace NetworkSkins.Helpers {
    using ColossalFramework.UI;
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public class NSPersistencyWrapper : IPersistency {
        public object Instance { get; private set; }
        private static class Delegates {
            public delegate string GetValue(NetInfo prefab, string key);
            public delegate void SetValue(NetInfo prefab, string key, string value);
            public delegate float? GetFloatValue(NetInfo prefab, string key);
            public delegate void SetFloatValue(NetInfo prefab, string key, float value);
            public delegate Color? GetColorValue(NetInfo prefab, string key);
            public delegate void SetColorValue(NetInfo prefab, string key, Color value);
            public delegate bool? GetBoolValue(NetInfo prefab, string key);
            public delegate void SetBoolValue(NetInfo prefab, string key, bool value);
            public delegate void ClearValue(NetInfo prefab, string key);
        }

        private readonly Delegates.GetValue getValue_;
        private readonly Delegates.SetValue setValue_;
        private readonly Delegates.GetFloatValue getFloatValue_;
        private readonly Delegates.SetFloatValue setFloatValue_;
        private readonly Delegates.GetColorValue getColorValue_;
        private readonly Delegates.SetColorValue setColorValue_;
        private readonly Delegates.GetBoolValue getBoolValue_;
        private readonly Delegates.SetBoolValue setBoolValue_;
        private readonly Delegates.ClearValue clearValue_;

        public NSPersistencyWrapper(object instance) {
            Instance = instance;
            getValue_ = DelegateUtil.CreateClosedDelegate<Delegates.GetValue>(instance);
            setValue_ = DelegateUtil.CreateClosedDelegate<Delegates.SetValue>(instance);
            getFloatValue_ = DelegateUtil.CreateClosedDelegate<Delegates.GetFloatValue>(instance);
            setFloatValue_ = DelegateUtil.CreateClosedDelegate<Delegates.SetFloatValue>(instance);
            getColorValue_ = DelegateUtil.CreateClosedDelegate<Delegates.GetColorValue>(instance);
            setColorValue_ = DelegateUtil.CreateClosedDelegate<Delegates.SetColorValue>(instance);
            getBoolValue_ = DelegateUtil.CreateClosedDelegate<Delegates.GetBoolValue>(instance);
            setBoolValue_ = DelegateUtil.CreateClosedDelegate<Delegates.SetBoolValue>(instance);
            clearValue_ = DelegateUtil.CreateClosedDelegate<Delegates.ClearValue>(instance);
        }

        public string GetValue(NetInfo prefab, string key) => getValue_(prefab, key);
        public void SetValue(NetInfo prefab, string key, string value) => setValue_(prefab, key, value);
        public float? GetFloatValue(NetInfo prefab, string key) => getFloatValue_(prefab, key);
        public void SetFloatValue(NetInfo prefab, string key, float value) => setFloatValue_(prefab, key, value);
        public Color? GetColorValue(NetInfo prefab, string key) => getColorValue_(prefab, key);
        public void SetColorValue(NetInfo prefab, string key, Color value) => setColorValue_(prefab, key, value);
        public bool? GetBoolValue(NetInfo prefab, string key) => getBoolValue_(prefab, key);
        public void SetBoolValue(NetInfo prefab, string key, bool value) => setBoolValue_(prefab, key, value);
        public void ClearValue(NetInfo prefab, string key) => clearValue_(prefab, key);
    }
}

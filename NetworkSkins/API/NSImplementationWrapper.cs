namespace NetworkSkins.API {
    using ColossalFramework.IO;
    using System;

    public class NSImplementationWrapper {
        private static class Delegates {
            public delegate void Serialize(object impl, ICloneable data, DataSerializer s);
            public delegate ICloneable Deserialize(object impl, DataSerializer s);
            public delegate ICloneable GetDefaultData(object impl, NetInfo network);
            public delegate void OnPreNSLoaded(object impl);
            public delegate void OnPostNSLoaded(object impl);
            public delegate int get_ID(object impl);
        }

        public object Implemenation;

        private Delegates.Serialize serialize_;
        private Delegates.Deserialize deserialize_;
        private Delegates.GetDefaultData getDefaultData_;
        private Delegates.OnPreNSLoaded onPreNSLoaded_;
        private Delegates.OnPostNSLoaded onPostNSLoaded_;
        private Delegates.get_ID get_ID_;

        public NSImplementationWrapper(object impl) {
            Implemenation = impl;
            Type type = impl.GetType();
            serialize_ = DelegateUtil.CreateDelegate<Delegates.Serialize>(type, true);
            deserialize_ = DelegateUtil.CreateDelegate<Delegates.Deserialize>(type, true);
            getDefaultData_ = DelegateUtil.CreateDelegate<Delegates.GetDefaultData>(type, true);
            onPreNSLoaded_ = DelegateUtil.CreateDelegate<Delegates.OnPreNSLoaded>(type, true);
            onPostNSLoaded_ = DelegateUtil.CreateDelegate<Delegates.OnPostNSLoaded>(type, true);
            get_ID_ = DelegateUtil.CreateDelegate<Delegates.get_ID>(type, true);
        }


        public int ID => get_ID_(Implemenation);
        public void Serialize(ICloneable data, DataSerializer s) => serialize_(Implemenation, data, s);
        public ICloneable Deserialize(DataSerializer s) => deserialize_(Implemenation, s);
        public void OnPreNSLoaded() => onPreNSLoaded_(Implemenation);
        public void OnPostNSLoaded() => onPostNSLoaded_(Implemenation);
        public ICloneable GetDefaultData(NetInfo network) => getDefaultData_(Implemenation, network);
    }
}

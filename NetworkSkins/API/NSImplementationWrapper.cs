namespace NetworkSkins.API {
    using ColossalFramework.IO;
    using ColossalFramework.UI;
    using System;

    public class NSImplementationWrapper {
        private static class Delegates {
            public delegate string Encode64(object impl, ICloneable data);
            public delegate ICloneable Decode64(object impl, string base64Data, Version dataVersion);
            public delegate void OnPreNSLoaded(object impl);
            public delegate void OnPostNSLoaded(object impl);
            public delegate string get_ID(object impl);
            public delegate Version get_Version(object impl);
        }

        public object Implemenation;

        private Delegates.Encode64 encode64_;
        private Delegates.Decode64 decode64_;
        private Delegates.OnPreNSLoaded onPreNSLoaded_;
        private Delegates.OnPostNSLoaded onPostNSLoaded_;
        private Delegates.get_ID get_ID_;
        private Delegates.get_Version get_Version_;

        public NSImplementationWrapper(object impl) {
            Implemenation = impl;
            Type type = impl.GetType();
            encode64_ = DelegateUtil.CreateDelegate<Delegates.Encode64>(type, true);
            decode64_ = DelegateUtil.CreateDelegate<Delegates.Decode64>(type, true);
            onPreNSLoaded_ = DelegateUtil.CreateDelegate<Delegates.OnPreNSLoaded>(type, true);
            onPostNSLoaded_ = DelegateUtil.CreateDelegate<Delegates.OnPostNSLoaded>(type, true);
            get_ID_ = DelegateUtil.CreateDelegate<Delegates.get_ID>(type, true);
            get_Version_ = DelegateUtil.CreateDelegate<Delegates.get_Version>(type, true);
        }

        public string ID => get_ID_(Implemenation);
        public Version Version => get_Version_(Implemenation);
        public string Encode64(ICloneable data) {
            if(data == null)
                return null;
            else
                return encode64_(Implemenation, data);
        }
        public ICloneable Decode64(string base64Data, Version dataVersion) {
            if(base64Data == null)
                return null;
            else
                return decode64_(Implemenation, base64Data, dataVersion);
        }

        public void OnPreNSLoaded() => onPreNSLoaded_(Implemenation);
        public void OnPostNSLoaded() => onPostNSLoaded_(Implemenation);

        public UITextureAtlas Atlas => throw new NotImplementedException();
        public string BackGroundSprite => throw new NotImplementedException();
    }
}

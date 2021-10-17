namespace NetworkSkins.Skins {
    using ColossalFramework.IO;
    using NetworkSkins.API;
    using System;
    using System.Linq;

    public class CustomDataColloction : DictionarySoft<int, ICloneable>, ICloneable {
        CustomDataColloction() { }

        public CustomDataColloction(NetInfo network) {
            foreach(var impl in API.Instance.ImplementationWrappers) {
                var value = impl.GetDefaultData(network);
                if(value is not null)
                    this[impl.ID] = value;
            }
        }

        #region serialization
        public void Serialize(DataSerializer s) {
            s.WriteInt32Array(Keys.ToArray());
            foreach(var pair in this) {
                var impl = API.Instance.GetImplementationWrapper(pair.Key);
                var data = pair.Value;
                impl.Serialize(data, s);
            }
        }

        public static CustomDataColloction Deserialize(DataSerializer s) {
            var ret = new CustomDataColloction();
            var keys = s.ReadInt32Array();
            foreach(int key in keys) {
                var impl = API.Instance.GetImplementationWrapper(key);
                if(impl != null) {
                    var data = impl.Deserialize(s);
                    ret[key] = data;
                }
            }
            return ret;
        }
        #endregion

        #region comparison
        private bool Equals(CustomDataColloction rhs) {
            var keys = Keys.Union(rhs.Keys);
            return keys.All(key => this[key].Equals(rhs[key]));
        }

        public override bool Equals(object obj) =>
            obj is CustomDataColloction data && Equals(data);


        public override int GetHashCode() {
            int hc = Count;
            foreach(object item in Values) {
                if(item != null) {
                    hc = unchecked(hc * 314159 + item.GetHashCode());
                }
            }
            return hc;
        }
        #endregion

        public CustomDataColloction Clone() {
            var ret = new CustomDataColloction();
            foreach(var pair in this) {
                ret[pair.Key] = pair.Value?.Clone() as ICloneable;
            }
            return ret;
        }

        object ICloneable.Clone() => Clone();
    }
}

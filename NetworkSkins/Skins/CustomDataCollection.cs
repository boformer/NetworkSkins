namespace NetworkSkins.Skins {
    using NetworkSkins.API;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using NetworkSkins.Persistence;
    using UnityEngine;

    public class CustomDataDTO : ICloneable {
        public string ID;
        public string Version;
        public string Base64Data;
        public CustomDataDTO Clone() => this.MemberwiseClone() as CustomDataDTO;
        object ICloneable.Clone() => Clone();

        public override int GetHashCode() {
            int hc = ID.GetHashCode();
            hc = unchecked(hc * 314159 + Base64Data.GetHashCode());
            return hc;
        }

        public override bool Equals(object obj) {
            return 
                obj is CustomDataDTO dto && 
                ID == dto.ID && 
                Base64Data == dto.Base64Data;
        }
    }

    public class CustomDataCollection : Dictionary<string, ICloneable>, ICloneable {
        private ICloneable[] datas_;
        public CustomDataCollection() {
            datas_ = new ICloneable[NSAPI.Instance.ImplementationWrappers.Count];
            NSAPI.Instance.EventImplementationAdded += OnImplementationAdded;
            NSAPI.Instance.EventImplementationRemoving += OnImplementationRemoving;
        }
        ~CustomDataCollection() {
            NSAPI.Instance.EventImplementationAdded -= OnImplementationAdded;
            NSAPI.Instance.EventImplementationRemoving -= OnImplementationRemoving;
        }

        public new ICloneable this[string key] {
            get {
                if(base.TryGetValue(key, out ICloneable ret))
                    return ret;
                return null;
            }
            set {
                base[key] = value;
                var index = NSAPI.Instance.GetImplementationIndex(key);
                if (index >= 0) {
                    EnsureIndex(index);
                    datas_[index] = value;
                }
            }
        }

        public ICloneable this[int index] {
            get {
                EnsureIndex(index);
                return datas_[index];
            }
        }

        private void EnsureIndex(int index, bool silent = false) {
            if(index < datas_.Length)
                return;
            
            int count = NSAPI.Instance.ImplementationWrappers.Count;
            if(index >= count) {
                if (!silent) {
                    Debug.LogWarning($"Warning: expected index < NSAPI.Instance.ImplementationWrappers.Count. But Index={index} Count={count}");
                }
                count = index + 1;
            }

            var newDatas = new ICloneable[count];
            Array.Copy(datas_, newDatas, datas_.Length);
            datas_ = newDatas;
        }

        private void OnImplementationRemoving(NSImplementationWrapper impl) {
            var data = this[impl.ID];
            if (data is not CustomDataDTO) {
                var dto = new CustomDataDTO {
                    ID = impl.ID,
                    Version = impl.DataVersion.ToString(),
                    Base64Data = impl.Encode64(data),
                };
                this[impl.ID] = dto;
            }
        }

        private void OnImplementationAdded(NSImplementationWrapper impl) {
            if (this.TryGetValue(impl.ID, out ICloneable data) && data is CustomDataDTO dto) {
                if (impl.ID != dto.ID) {
                    Debug.LogError($"ids do not match: impl.ID={impl.ID} while dto.ID:{dto.ID}");
                    return;
                }
                this[impl.ID] = impl.Decode64(dto.Base64Data, new Version(dto.Version));
            }
        }

        #region serialization
        public string Encode64() {
            List<CustomDataDTO> dtos = new List<CustomDataDTO>(Count);
            foreach(var pair in this) {
                if(pair.Value is null) {
                    continue;
                } else if(pair.Value is CustomDataDTO dto) {
                    // implementation is abscent so dto was not fully decoded.
                    dtos.Add(dto);
                } else {
                    var impl = NSAPI.Instance.GetImplementationWrapper(pair.Key);
                    dtos.Add(new CustomDataDTO {
                        ID = impl.ID,
                        Version = impl.DataVersion.ToString(),
                        Base64Data = impl.Encode64(pair.Value),
                    });
                }
            }
            if(dtos.Count == 0)
                return null;
            else 
                return XMLUtil.Serialize(dtos.ToArray());
        }

        public static CustomDataCollection Decode64(string base64Data) {
            var ret = new CustomDataCollection();
            if(base64Data is not null) {
                CustomDataDTO[] dtos = XMLUtil.Deserialize<CustomDataDTO[]>(base64Data);
                foreach(var dto in dtos) {
                    var impl = NSAPI.Instance.GetImplementationWrapper(dto.ID);
                    ICloneable data = impl != null
                        ? impl.Decode64(dto.Base64Data, new Version(dto.Version))
                        : dto;
                    ret[dto.ID] = data;
                }
            }
            return ret;
        }
        #endregion

        #region comparison
        private bool Equals(CustomDataCollection rhs) {
            var keys = Keys.Union(rhs.Keys);
            return keys.All(key => this[key].Equals(rhs[key]));
        }

        public override bool Equals(object obj) =>
            obj is CustomDataCollection data && Equals(data);


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

        public CustomDataCollection Clone() {
            var ret = new CustomDataCollection();
            foreach(var pair in this) {
                ret[pair.Key] = pair.Value?.Clone() as ICloneable;
            }
            return ret;
        }

        object ICloneable.Clone() => Clone();
    }
}

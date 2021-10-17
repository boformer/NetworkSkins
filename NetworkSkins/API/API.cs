namespace NetworkSkins.API {
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class API {
        public static API Instance;
        public List<NSImplementationWrapper> ImplementationWrappers = new List<NSImplementationWrapper>();
        public void AddImplementation(object impl) {
            ImplementationWrappers.Add(new NSImplementationWrapper(impl));
        }
        public bool RemoveImplementation(object impl) {
            var item = ImplementationWrappers.FirstOrDefault(item => item.Implemenation == impl);
            return ImplementationWrappers.Remove(item);
        }
        public NSImplementationWrapper GetImplementationWrapper(int id) {
            return Instance.ImplementationWrappers.FirstOrDefault(item => item.ID == id);
        }
    }
}

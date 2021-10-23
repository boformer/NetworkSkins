namespace NetworkSkins.Helpers {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using NetworkSkins.API;

    public static class NSHelpers {
        public static object GetSegmentSkinData(string dataID, ushort segmentID) {
            return NSAPI.Instance.GetSegmentSkinData( dataID,segmentID);
        }

        public static void RegisterImplementation(INSImplementation impl) {
            NSAPI.Instance.AddImplementation(impl);
        }
    }
}

namespace NetworkSkins.Helpers {
    using ColossalFramework.IO;
    using System;

    public interface NSImplementation {
        int ID { get; }

        void Serialize(ICloneable data, DataSerializer s);
        ICloneable Deserialize(DataSerializer s);
        void OnPreNSLoaded();
        void OnPostNSLoaded();

        ICloneable GetDefaultData(ushort segmentID);
    }
}
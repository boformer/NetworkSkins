namespace NetworkSkins.Helpers {
    using ColossalFramework.IO;
    using System;

    public interface INSImplementation {
        int ID { get; }
        Version DataVersion { get; }

        /// <summary>Converts data to base 64 string.</summary>
        /// <param name="data">data returned by <see cref="Copy(InstanceID)"/> </param>
        public abstract string Encode64(ICloneable data);

        /// <summary>Decode the data encoded by <see cref="Encode64(object)"/>.</summary>
        /// <param name="base64Data">The base 64 string that was encoded in <see cref="Encode64(object)"/></param>
        /// <param name="dataVersion"><see cref="DataVersion"/> when data was stored</param>
        /// <returns>The data the integrated mod encoded</returns>
        ICloneable Decode64(string base64Data, Version dataVersion);
        void OnPreNSLoaded();
        void OnPostNSLoaded();
        void OnSkinApplied(object data, InstanceID instanceID);

    }
}
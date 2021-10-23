namespace NetworkSkins.Helpers {
    using ColossalFramework.UI;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Note: data should implement GetHash() and Equals()
    /// </summary>
    public interface INSImplementation : INSPersistancy, INSGUUIImplementation, INSControllerImpelementation {
        string ID { get; }
        int Index { get; set; }
        void OnBeforeNSLoaded();
        void OnAfterNSLoaded();
        void OnNSDisabled();
        void OnSkinApplied(object data, InstanceID instanceID);
    }

    public interface INSPersistancy {
        Version DataVersion { get; }

        /// <summary>Converts data to base 64 string.</summary>
        /// <param name="data">data returned by <see cref="Copy(InstanceID)"/> </param>
        string Encode64(ICloneable data);

        /// <summary>Decode the data encoded by <see cref="Encode64(object)"/>.</summary>
        /// <param name="base64Data">The base 64 string that was encoded in <see cref="Encode64(object)"/></param>
        /// <param name="dataVersion"><see cref="DataVersion"/> when data was stored</param>
        /// <returns>The data the integrated mod encoded</returns>
        ICloneable Decode64(string base64Data, Version dataVersion);
    }

    public interface INSGUUIImplementation {
        /// <summary>
        /// Atlas for button.
        /// </summary>
        UITextureAtlas Atlas { get; }
        
        /// <summary>
        /// Sprite for the button.
        /// </summary>
        string BackGroundSprite { get; }
        
        /// <summary>
        /// Add UI elements to the panel using the data from your controller.
        /// </summary>
        /// <param name="panel">panel is vertical auto-arrange auto-fit children</param>
        void BuildPanel(UIPanel panel);
        
        /// <summary>
        /// Refresh the state of your UI elements using the values from your controller.
        /// </summary>
        /// <param name="netInfo"></param>
        void RefreshUI(NetInfo netInfo);
    }

    /// <summary>
    /// get selected prefab from NetTool.m_prefab
    /// </summary>
    public interface INSControllerImpelementation {
        /// <summary>
        /// determine if the selected prefab is supported by your mod.
        /// </summary>
        bool Enabled { get; }

        /// <summary>
        /// Note: data should implement GetHash() and Equals()
        /// </summary>
        /// <returns>custom data for the selected prefab and/or its elevations. return null if there is no data or data is default.</returns>
        Dictionary<NetInfo, ICloneable> BuildCustomData();

        /// <summary>
        /// User used pipet tool to select an skin. load data and then save to ActiveSelectionData for the selected prefab
        /// </summary>
        void LoadWithData(ICloneable data);
        
        /// <summary>
        /// Use ActiveSelectionData to load data for the selected prefab
        /// </summary>
        void LoadActiveSelection();

        /// <summary>
        /// user pressed reset button for the active skin. clear data from ActiveSelectionData for the selected prefab.
        /// </summary>
        void Reset();
    }
}
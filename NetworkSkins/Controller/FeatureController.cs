using System;
using System.Collections.Generic;
using NetworkSkins.Skins;

namespace NetworkSkins.Controller
{
    public abstract class FeatureController
    {
        protected NetInfo Prefab { get; private set; } = null;

        public Dictionary<NetInfo, List<NetworkSkinModifier>> Modifiers { get; private set; } = new Dictionary<NetInfo, List<NetworkSkinModifier>>();

        /// <summary>
        /// The button for this feature should only be show when this is true
        /// </summary>
        public abstract bool Enabled { get; }

        /// <summary>
        /// Event is called after the items for the new prefab have been built
        /// and after there was a change to the modifiers.
        /// </summary>
        public event ChangedEventHandler EventChanged;
        public delegate void ChangedEventHandler();

        public void OnPrefabChanged(NetInfo prefab)
        {
            if (Prefab == prefab) return;

            Prefab = prefab ?? throw new ArgumentNullException(nameof(prefab));

            Build();

            OnChanged();
        }

        protected abstract void Build();

        protected void OnChanged()
        {
            Modifiers = BuildModifiers();

            EventChanged?.Invoke();
        }
       
        protected abstract Dictionary<NetInfo, List<NetworkSkinModifier>> BuildModifiers();
    }
}

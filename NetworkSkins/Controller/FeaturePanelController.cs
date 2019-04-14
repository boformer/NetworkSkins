using System.Collections.Generic;
using NetworkSkins.Skins;

namespace NetworkSkins.Controller
{
    public abstract class FeaturePanelController
    {
        public NetInfo Prefab { get; private set; }

        public Dictionary<NetInfo, List<NetworkSkinModifier>> Modifiers { get; private set; } = new Dictionary<NetInfo, List<NetworkSkinModifier>>();

        /// <summary>
        /// The button for this feature should only be show when this is true
        /// </summary>
        public virtual bool Enabled => Prefab != null;

        /// <summary>
        /// Event is called after the items for the new prefab have been built
        /// and after there was a change to the modifiers.
        /// </summary>
        public event ModifiersChangedEventHandler EventModifiersChanged;
        public delegate void ModifiersChangedEventHandler();

        public void OnPrefabChanged(NetInfo prefab)
        {
            if (Prefab == prefab) return;

            Prefab = prefab;

            if (Prefab != null)
            {
                Build();
            }

            OnChanged();
        }

        protected abstract void Build();

        protected virtual void OnChanged()
        {
            if (Enabled && Prefab != null)
            {
                Modifiers = BuildModifiers();
            }
            else
            {
                Modifiers = new Dictionary<NetInfo, List<NetworkSkinModifier>>();
            }

            EventModifiersChanged?.Invoke();
        }

        protected abstract Dictionary<NetInfo, List<NetworkSkinModifier>> BuildModifiers();
    }
}

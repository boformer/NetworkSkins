using System.Collections.Generic;
using NetworkSkins.Net;
using NetworkSkins.Skins;

namespace NetworkSkins.GUI.Abstraction
{
    public abstract class FeaturePanelControllerBase
    {
        public virtual NetInfo Prefab { get; private set; }

        /// <summary>
        /// The button for this feature should only be shown when this is true
        /// </summary>
        public virtual bool Enabled => Prefab != null;

        /// <summary>
        /// Event is called after the items for the new prefab have been built
        /// and after there was a change to the modifiers.
        /// </summary>
        public event ModifiersChangedEventHandler EventModifiersChanged;
        public delegate void ModifiersChangedEventHandler();

        /// <summary>
        /// Called when selected prefab changes.
        /// </summary>
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

        /// <summary>
        /// Called when pippet tool selects a new segment.
        /// </summary>
        public void OnPrefabWithModifiersSelected(NetInfo prefab, List<NetworkSkinModifier> modifiers)
        {
            Prefab = prefab;

            if (Prefab != null)
            {
                BuildWithModifiers(modifiers);
            }

            OnChanged();
        }

        /// <summary>
        /// Called when user presses reset the reset button
        /// </summary>
        public abstract void Reset();

        /// <summary>
        /// Called when usign the NetTool to select a new segment. Use ActiveSelectionData to access local/global data.
        /// </summary>
        protected abstract void Build();

        /// <summary>
        /// Called when using the PippetTool to select a new segment. The modifiers come from the skin.
        /// </summary>
        /// <param name="modifiers"></param>
        protected abstract void BuildWithModifiers(List<NetworkSkinModifier> modifiers);

        /// <summary>
        /// Called when a new prefab/skin has been selected
        /// or after any change to the modifiers.
        /// </summary>
        protected virtual void OnChanged()
        {
            EventModifiersChanged?.Invoke();
        }
    }
}

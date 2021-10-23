using System.Collections.Generic;
using NetworkSkins.Skins;

namespace NetworkSkins.GUI.Abstraction
{
    public abstract class FeaturePanelController : FeaturePanelControllerBase
    {
        public Dictionary<NetInfo, List<NetworkSkinModifier>> Modifiers { get; private set; } = new Dictionary<NetInfo, List<NetworkSkinModifier>>();

        protected override void OnChanged()
        {
            if (Enabled && Prefab != null)
            {
                Modifiers = BuildModifiers();
            }
            else
            {
                Modifiers = new Dictionary<NetInfo, List<NetworkSkinModifier>>();
            }

            base.OnChanged();
        }

        /// <summary>
        /// called from <see cref="OnChanged()"/>
        /// </summary>
        /// <returns>modifiere[s] controled by the panel for the current prefab and its variations</returns>
        protected abstract Dictionary<NetInfo, List<NetworkSkinModifier>> BuildModifiers();
    }
}

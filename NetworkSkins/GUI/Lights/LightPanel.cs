using ColossalFramework.UI;
namespace NetworkSkins.GUI
{
    public class LightPanel : ListPanelBase<LightList, PropInfo>
    {
        protected override void RefreshUI(NetInfo netInfo) {
            base.RefreshUI(netInfo);
        }

        protected override void OnSearchLostFocus() {
        }

        protected override void OnSearchTextChanged(string text) {
        }

        protected override void OnPrefabChanged(NetInfo netInfo) {
            RefreshUI(netInfo);
        }

        protected override void OnPanelCreated() {
            tabStrip.isVisible = false;
        }
    }
}

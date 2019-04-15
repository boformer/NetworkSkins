using NetworkSkins.GUI.Abstraction;

namespace NetworkSkins.GUI.Catenaries
{
    public class CatenaryPanel : ListPanelBase<CatenaryList, PropInfo>
    {
        protected override void RefreshUI(NetInfo netInfo) {
            if (NetworkSkinPanelController.Catenary.Items.Count < 10)
                width -= 12.0f;
        }

        protected override void OnPanelBuilt() {
            pillarTabStrip.isVisible = false;
            laneTabStrip.isVisible = false;
            Refresh();
        }

        protected override void OnSelectedChanged(string itemID, bool selected) {
            if (!selected) return;
            NetworkSkinPanelController.Catenary.SetSelectedItem(itemID);
        }
    }
}

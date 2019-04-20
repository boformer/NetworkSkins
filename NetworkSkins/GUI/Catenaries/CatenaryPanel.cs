using NetworkSkins.GUI.Abstraction;
using UnityEngine;

namespace NetworkSkins.GUI.Catenaries
{
    public class CatenaryPanel : ListPanelBase<CatenaryList, PropInfo>
    {
        protected override void RefreshUI(NetInfo netInfo) {
            if (NetworkSkinPanelController.Catenary.Items.Count < 10)
                width -= 12.0f;
        }

        protected override void OnPanelBuilt() {
            pillarTabstrip.isVisible = false;
            laneTabstripContainer.isVisible = false;
            Refresh();
        }

        protected override void OnSelectedChanged(string itemID, bool selected) {
            if (!selected) return;
            NetworkSkinPanelController.Catenary.SetSelectedItem(itemID);
        }
    }
    public class CatenaryList : ListBase<PropInfo>
    {
        protected override Vector2 ListSize => 
            NetworkSkinPanelController.Catenary.Items.Count < 10 ? 
            new Vector2(base.ListSize.x - 12.0f, RowHeight * NetworkSkinPanelController.Catenary.Items.Count) : 
            base.ListSize;
    }
}

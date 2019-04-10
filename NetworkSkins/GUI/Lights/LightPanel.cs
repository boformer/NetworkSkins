using UnityEngine;

namespace NetworkSkins.GUI
{
    public class LightPanel : ListPanelBase<LightList, PropInfo>
    {
        private DistancePanel distancePanel;

        public override void Build(PanelType panelType, Layout layout) {
            base.Build(panelType, layout);
            distancePanel = AddUIComponent<DistancePanel>();
            distancePanel.Build(panelType, new Layout(new Vector2(390.0f, 100.0f), true, ColossalFramework.UI.LayoutDirection.Vertical, ColossalFramework.UI.LayoutStart.TopLeft, 5));
        }

        protected override void RefreshUI(NetInfo netInfo) {
            list.RefreshRowsData();
        }

        protected override void OnSearchLostFocus() {
        }

        protected override void OnSearchTextChanged(string text) {
        }

        protected override void OnPanelBuilt() {
            laneTabStrip.isVisible = false;
            pillarTabStrip.isVisible = false;
            RefreshAfterBuild();
        }

        protected override void OnFavouriteChanged(string itemID, bool favourite) {
        }

        protected override void OnSelectedChanged(string itemID, bool selected) {
            if (!selected) return;
            SkinController.StreetLight.SetSelectedItem(itemID);
            list.Select(itemID);
        }
    }
}

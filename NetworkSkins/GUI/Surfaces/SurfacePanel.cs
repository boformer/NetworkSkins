using ColossalFramework.UI;
using NetworkSkins.Net;
using UnityEngine;

namespace NetworkSkins.GUI
{
    public class SurfacePanel : ListPanelBase
    {
        private SurfaceList list;

        protected override void RefreshUI(NetInfo netInfo) {
            list.RefreshRowsData();
        }

        protected override void CreateList() {
            list = AddUIComponent<SurfaceList>();
            list.Build(PanelType, new Layout(new Vector2(378.0f, 0.0f), true, LayoutDirection.Vertical, LayoutStart.TopLeft, 0));
            list.EventFavouriteChanged += OnFavouriteChanged;
            list.EventSelectedChanged += OnSelectedChanged;
        }

        protected override void OnPanelBuilt() {
            laneTabStrip.isVisible = false;
            pillarTabStrip.isVisible = false;
            RefreshAfterBuild();
        }

        protected override void OnSearchTextChanged(string text) {

        }

        protected override void OnSearchLostFocus() {

        }

        protected override void OnFavouriteChanged(string itemID, bool favourite) {

        }

        protected override void OnSelectedChanged(string itemID, bool selected) {
            if (!selected) return;
            SkinController.TerrainSurface.SetSelectedItem(itemID);
            list.Select(itemID);
        }
    }
}

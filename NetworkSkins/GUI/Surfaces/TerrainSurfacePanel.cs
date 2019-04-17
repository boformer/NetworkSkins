using ColossalFramework.UI;
using NetworkSkins.GUI.Abstraction;
using UnityEngine;

namespace NetworkSkins.GUI.Surfaces
{
    public class TerrainSurfacePanel : ListPanelBase
    {
        private TerrainSurfaceList list;

        protected override void CreateList() {
            list = AddUIComponent<TerrainSurfaceList>();
            list.Build(PanelType, new Layout(new Vector2(378.0f, 0.0f), true, LayoutDirection.Vertical, LayoutStart.TopLeft, 0));
            list.EventFavouriteChanged += OnFavouriteChanged;
            list.EventSelectedChanged += OnSelectedChanged;
        }

        protected override void OnPanelBuilt() {
            laneTabStrip.isVisible = false;
            pillarTabStrip.isVisible = false;
            Refresh();
        }

        protected override void OnSelectedChanged(string itemID, bool selected) {
            if (!selected) return;
            NetworkSkinPanelController.TerrainSurface.SetSelectedItem(itemID);
        }
    }
}

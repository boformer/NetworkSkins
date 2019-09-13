using ColossalFramework.UI;
using NetworkSkins.GUI.Abstraction;
using UnityEngine;

namespace NetworkSkins.GUI.Surfaces
{
    public class TerrainSurfacePanel : ListPanelBase
    {
        private TerrainSurfaceList list;
        public override void OnDestroy() {
            list.EventItemClick -= OnItemClick;
            base.OnDestroy();
        }
        protected override void CreateList() {
            list = AddUIComponent<TerrainSurfaceList>();
            list.Build(PanelType, new Layout(new Vector2(378.0f, 0.0f), true, LayoutDirection.Vertical, LayoutStart.TopLeft, 0));
            list.EventItemClick += OnItemClick;
        }

        protected override void OnPanelBuilt() {
            laneTabstripContainer.isVisible = false;
            pillarTabstrip.isVisible = false;
            Refresh();
        }

        protected void OnItemClick(string itemID)
        {
            NetworkSkinPanelController.TerrainSurface.SetSelectedItem(itemID);
        }
    }
}

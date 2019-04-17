using NetworkSkins.GUI.Abstraction;
using UnityEngine;

namespace NetworkSkins.GUI.Lights
{
    public class StreetLightPanel : ListPanelBase<StreetLightList, PropInfo>
    {
        private DistancePanel distancePanel;

        public override void Build(PanelType panelType, Layout layout) {
            base.Build(panelType, layout);
            distancePanel = AddUIComponent<DistancePanel>();
            distancePanel.Build(panelType, new Layout(new Vector2(390.0f, 100.0f), true, ColossalFramework.UI.LayoutDirection.Vertical, ColossalFramework.UI.LayoutStart.TopLeft, 5));
        }

        protected override void OnPanelBuilt() {
            laneTabStrip.isVisible = false;
            pillarTabStrip.isVisible = false;
            Refresh();
        }

        protected override void OnSelectedChanged(string itemID, bool selected) {
            if (!selected) return;
            NetworkSkinPanelController.StreetLight.SetSelectedItem(itemID);
        }
    }
}

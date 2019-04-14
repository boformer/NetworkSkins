using NetworkSkins.Net;
using System.Linq;

namespace NetworkSkins.GUI
{
    public class PillarPanel : ListPanelBase<PillarList, BuildingInfo>
    {
        protected override void RefreshUI(NetInfo netInfo) {
            pillarTabStrip.isVisible = true;
            SetTabEnabled(Pillar.Bridge, NetworkSkinPanelController.BridgeBridgePillar.Enabled);
            SetTabEnabled(Pillar.BridgeMiddle, NetworkSkinPanelController.BridgeMiddlePillar.Enabled);
            SetTabEnabled(Pillar.Elevated, NetworkSkinPanelController.ElevatedBridgePillar.Enabled);
            SetTabEnabled(Pillar.ElevatedMiddle, NetworkSkinPanelController.ElevatedMiddlePillar.Enabled);
            int tabCount = pillarTabs.Count(tab => tab.isVisible);
            if (tabCount != 0) {
                for (int i = (int)Pillar.Count - 1; i >= 0 ; i--) {
                    pillarTabs[i].width = pillarTabStrip.width / tabCount;
                    if (!NetworkSkinPanelController.TabClicked) pillarTabStrip.selectedIndex = i;
                }
            }
            if (tabCount == 1) {
                pillarTabStrip.isVisible = false;
            }
            NetworkSkinPanelController.TabClicked = false;
        }

        private void SetTabEnabled(Pillar pillar, bool enabled) {
            pillarTabs[(int)pillar].isVisible = enabled;
        }

        protected override void OnSearchLostFocus() {
        }

        protected override void OnSearchTextChanged(string text) {
        }

        protected override void OnPanelBuilt() {
            laneTabStrip.isVisible = false;
            Refresh();
        }

        protected override void OnFavouriteChanged(string itemID, bool favourite) {

        }

        protected override void OnSelectedChanged(string itemID, bool selected) {
            if (!selected) return;
            switch (NetworkSkinPanelController.PillarElevationCombination) {
                case Pillar.Elevated: NetworkSkinPanelController.ElevatedBridgePillar.SetSelectedItem(itemID); break;
                case Pillar.ElevatedMiddle: NetworkSkinPanelController.ElevatedMiddlePillar.SetSelectedItem(itemID); break;
                case Pillar.Bridge: NetworkSkinPanelController.BridgeBridgePillar.SetSelectedItem(itemID); break;
                case Pillar.BridgeMiddle: NetworkSkinPanelController.BridgeMiddlePillar.SetSelectedItem(itemID); break;
                default: break;
            }
        }
    }
}

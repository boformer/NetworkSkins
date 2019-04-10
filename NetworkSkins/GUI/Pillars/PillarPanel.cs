using NetworkSkins.Net;
using System.Linq;

namespace NetworkSkins.GUI
{
    public class PillarPanel : ListPanelBase<PillarList, BuildingInfo>
    {
        protected override void RefreshUI(NetInfo netInfo) {
            SetTabEnabled(Pillar.Bridge, SkinController.BridgeBridgePillar.Enabled);
            SetTabEnabled(Pillar.BridgeMiddle, SkinController.BridgeMiddlePillar.Enabled);
            SetTabEnabled(Pillar.Elevated, SkinController.ElevatedBridgePillar.Enabled);
            SetTabEnabled(Pillar.ElevatedMiddle, SkinController.ElevatedMiddlePillar.Enabled);
            int tabCount = pillarTabs.Count(tab => tab.isVisible);
            if (tabCount != 0) {
                for (int i = 0; i < (int)Pillar.Count; i++) {
                    pillarTabs[i].width = pillarTabStrip.width / tabCount;
                }
            }
            list.RefreshRowsData();
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
            RefreshAfterBuild();
        }

        protected override void OnFavouriteChanged(string itemID, bool favourite) {

        }

        protected override void OnSelectedChanged(string itemID, bool selected) {
            if (!selected) return;
            switch (SkinController.PillarElevationCombination) {
                case Pillar.Elevated: SkinController.ElevatedBridgePillar.SetSelectedItem(itemID); break;
                case Pillar.ElevatedMiddle: SkinController.ElevatedMiddlePillar.SetSelectedItem(itemID); break;
                case Pillar.Bridge: SkinController.BridgeBridgePillar.SetSelectedItem(itemID); break;
                case Pillar.BridgeMiddle: SkinController.BridgeMiddlePillar.SetSelectedItem(itemID); break;
                default: break;
            }
            list.Select(itemID);
        }
    }
}

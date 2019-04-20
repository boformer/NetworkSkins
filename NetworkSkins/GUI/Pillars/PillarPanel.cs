using System.Linq;
using NetworkSkins.GUI.Abstraction;
using NetworkSkins.Net;

namespace NetworkSkins.GUI.Pillars
{
    public class PillarPanel : ListPanelBase<PillarList, BuildingInfo>
    {
        protected override void RefreshUI(NetInfo netInfo) {
            base.RefreshUI(netInfo);
            pillarTabstrip.isVisible = true;
            SetTabEnabled(Pillar.Bridge, NetworkSkinPanelController.BridgeBridgePillar.Enabled);
            SetTabEnabled(Pillar.BridgeMiddle, NetworkSkinPanelController.BridgeMiddlePillar.Enabled);
            SetTabEnabled(Pillar.Elevated, NetworkSkinPanelController.ElevatedBridgePillar.Enabled);
            SetTabEnabled(Pillar.ElevatedMiddle, NetworkSkinPanelController.ElevatedMiddlePillar.Enabled);
            int tabCount = pillarTabs.Count(tab => tab.isVisible);
            if (tabCount != 0) {
                for (int i = 0; i < (int)Pillar.Count; i++) {
                    pillarTabs[i].width = pillarTabstrip.width / tabCount;
                }
            }
            if (tabCount == 1) {
                pillarTabstrip.isVisible = false;
            }
            RefreshTabstrip();
        }

        private void RefreshTabstrip() {
            _ignoreEvents = true;
            pillarTabstrip.selectedIndex = (int)NetworkSkinPanelController.Pillar;
            _ignoreEvents = false;
        }

        private void SetTabEnabled(Pillar pillar, bool enabled) {
            pillarTabs[(int)pillar].isVisible = enabled;
        }

        protected override void OnPanelBuilt() {
            laneTabstripContainer.isVisible = false;
            Refresh();
        }

        protected override void OnSelectedChanged(string itemID, bool selected) {
            if (!selected) return;
            switch (NetworkSkinPanelController.Pillar) {
                case Pillar.Elevated: NetworkSkinPanelController.ElevatedBridgePillar.SetSelectedItem(itemID); break;
                case Pillar.ElevatedMiddle: NetworkSkinPanelController.ElevatedMiddlePillar.SetSelectedItem(itemID); break;
                case Pillar.Bridge: NetworkSkinPanelController.BridgeBridgePillar.SetSelectedItem(itemID); break;
                case Pillar.BridgeMiddle: NetworkSkinPanelController.BridgeMiddlePillar.SetSelectedItem(itemID); break;
                default: break;
            }
        }
    }

    public class PillarList : ListBase<BuildingInfo> { }
}

using ColossalFramework.UI;
using NetworkSkins.Net;
using UnityEngine;

namespace NetworkSkins.GUI
{
    public class PillarPanel : ListPanelBase<PillarList, BuildingInfo>
    {
        protected override void RefreshUI(NetInfo netInfo) {

        }

        protected override void OnSearchLostFocus() {
        }

        protected override void OnSearchTextChanged(string text) {
        }

        protected override void OnPanelBuilt() {
            tabStrip.isVisible = false;
            RefreshAfterBuild();
        }
    }
}

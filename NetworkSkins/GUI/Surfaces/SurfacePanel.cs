using ColossalFramework.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace NetworkSkins.GUI
{
    public class SurfacePanel : ListPanelBase
    {
        private SurfaceList list;

        protected override void RefreshUI(NetInfo netInfo) {

        }

        protected override void CreateList() {
            list = AddUIComponent<SurfaceList>();
            list.Build(new Layout(new Vector2(378.0f, 0.0f), true, LayoutDirection.Vertical, LayoutStart.TopLeft, 0));
        }

        protected override void OnPanelBuilt() {
            tabStrip.isVisible = false;
            RefreshAfterBuild();
        }

        protected override void OnSearchTextChanged(string text) {

        }

        protected override void OnSearchLostFocus() {

        }
    }
}

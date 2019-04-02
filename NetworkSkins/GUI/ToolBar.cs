using ColossalFramework.UI;
using UnityEngine;

namespace NetworkSkins.GUI
{
    public class ToolBar : PanelBase
    {
        private DragBar dragBar;
        public ButtonBar ButtonBar { get; private set; }

        public override void Build(Layout configuration) {
            base.Build(configuration);
            color = MainPanel.GUIColor;
            dragBar = AddUIComponent<DragBar>();
            dragBar.Build(new Layout(new Vector2(size.x, 18.0f), false, LayoutDirection.Horizontal, LayoutStart.TopLeft, 0));
            ButtonBar = AddUIComponent<ButtonBar>();
            ButtonBar.Build(new Layout(new Vector2(size.x, 0.0f), true, LayoutDirection.Vertical, LayoutStart.TopLeft, 5));
        }

        protected override void RefreshUI(NetInfo netInfo) {

        }
    }
}

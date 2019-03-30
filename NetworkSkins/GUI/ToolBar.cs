using ColossalFramework.UI;
using UnityEngine;

namespace NetworkSkins.GUI
{
    public class ToolBar : PanelBase
    {
        private DragBar dragBar;
        private ButtonBar buttonBar;

        public override void Build(Layout configuration) {
            base.Build(configuration);
            dragBar = AddUIComponent<DragBar>();
            dragBar.Build(new Layout(new Vector2(size.x, 18.0f), false, LayoutDirection.Horizontal, LayoutStart.TopLeft, 0));
            buttonBar = AddUIComponent<ButtonBar>();
            buttonBar.Build(new Layout(new Vector2(size.x, 0.0f), true, LayoutDirection.Vertical, LayoutStart.TopLeft, 5));
        }
    }
}

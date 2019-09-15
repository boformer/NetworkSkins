using ColossalFramework.UI;
using NetworkSkins.GUI.Abstraction;
using UnityEngine;

namespace NetworkSkins.GUI
{
    public class ToolBar : PanelBase
    {
        private DragBar dragBar;
        public ButtonBar ButtonBar { get; private set; }
        public event DragEndEventHandler EventDragEnd;

        public override void OnDestroy() {
            dragBar.EventDragEnd -= OnDragBarDragEnd;
            base.OnDestroy();
        }
        public override void Build(PanelType panelType, Layout layout) {
            base.Build(panelType, layout);
            color = GUIColor;
            dragBar = AddUIComponent<DragBar>();
            dragBar.Build(PanelType.None, new Layout(new Vector2(size.x, 18.0f), false, LayoutDirection.Horizontal, LayoutStart.TopLeft, 0));
            dragBar.EventDragEnd += OnDragBarDragEnd;
            ButtonBar = AddUIComponent<ButtonBar>();
            ButtonBar.Build(PanelType.None, new Layout(new Vector2(size.x, 0.0f), true, LayoutDirection.Vertical, LayoutStart.TopLeft, 5));
        }

        private void OnDragBarDragEnd() {
            EventDragEnd?.Invoke();
        }
    }
}

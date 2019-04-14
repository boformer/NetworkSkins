using ColossalFramework.UI;
using NetworkSkins.GUI.Abstraction;
using UnityEngine;

namespace NetworkSkins.GUI
{
    public class DragBar : PanelBase
    {
        private UIDragHandle dragHandle;
        private UIPanel panel;
        public event DragEndEventHandler EventDragEnd;

        public override void Build(PanelType panelType, Layout layout) {
            base.Build(panelType, layout);
            dragHandle = AddUIComponent<UIDragHandle>();
            dragHandle.target = parent.parent;
            dragHandle.isInteractive = true;
            dragHandle.size = new Vector2(parent.width, height);
            dragHandle.relativePosition = new Vector2(0.0f, 0.0f);
            dragHandle.eventMouseUp += OnDragEnd;
            panel = AddUIComponent<UIPanel>();
            panel.size = new Vector2(parent.width, height);
            panel.relativePosition = new Vector2(0.0f, 0.0f);
            panel.atlas = Resources.Atlas;
            panel.backgroundSprite = Resources.DragHandle;
            panel.isInteractive = false;
        }

        private void OnDragEnd(UIComponent component, UIMouseEventParameter eventParam) {

            EventDragEnd?.Invoke();
        }

        protected override void RefreshUI(NetInfo netInfo) {

        }
    }
}

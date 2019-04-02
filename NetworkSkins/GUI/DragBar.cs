using ColossalFramework.UI;
using UnityEngine;

namespace NetworkSkins.GUI
{
    public class DragBar : PanelBase
    {
        private UIDragHandle dragHandle;
        private UIPanel panel;

        public override void Build(Layout layout) {
            base.Build(layout);
            dragHandle = AddUIComponent<UIDragHandle>();
            dragHandle.target = parent.parent;
            dragHandle.isInteractive = true;
            dragHandle.size = new Vector2(parent.width, height);
            dragHandle.relativePosition = new Vector2(0.0f, 0.0f);
            panel = AddUIComponent<UIPanel>();
            panel.size = new Vector2(parent.width, height);
            panel.relativePosition = new Vector2(0.0f, 0.0f);
            panel.atlas = Resources.Atlas;
            panel.backgroundSprite = Resources.DragHandle;
            panel.isInteractive = false;
        }

        protected override void RefreshUI(NetInfo netInfo) {

        }
    }
}

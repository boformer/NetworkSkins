using NetworkSkins.API;
using NetworkSkins.GUI.Abstraction;

namespace NetworkSkins.GUI.Custom {
    public class CustomPanel : PanelBase {
        public NSImplementationWrapper Implementation;
        public override void Build(PanelType panelType, Layout layout) {
            Implementation.BuildPanel(this);
            color = GUIColor;
            base.Build(panelType, layout);
            autoFitChildrenHorizontally = true;
            UIUtil.CreateSpace(width + 5, 5, this);
        }

        protected override void RefreshUI(NetInfo netInfo) => Implementation.RefreshUI();
    }
}
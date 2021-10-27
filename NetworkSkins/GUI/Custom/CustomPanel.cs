using NetworkSkins.API;
using NetworkSkins.GUI.Abstraction;
using UnityEngine;

namespace NetworkSkins.GUI.Custom {
    public class CustomPanel : PanelBase {
        public NSImplementationWrapper Implementation;
        public override void Build(PanelType panelType, Layout layout) {
            Implementation.BuildPanel(this);
            color = GUIColor;
            base.Build(panelType, layout);
            autoFitChildrenHorizontally = autoFitChildrenVertically = true;
            eventFitChildren += OnAutoFit;
        }

        protected override void RefreshUI(NetInfo netInfo) {
            if(Implementation.Enabled)
                Implementation.RefreshUI();
        }

        /// <summary>
        /// workaround: fix auto fit bug in CO code.
        /// </summary>
        private void OnAutoFit() {
            size += new Vector2(Spacing, Spacing);
        }

    }
}
using ColossalFramework.UI;
using NetworkSkins.GUI.Abstraction;
using NetworkSkins.Locale;
using NetworkSkins.TranslationFramework;
using UnityEngine;

namespace NetworkSkins.GUI.RoadDecoration
{
    public class RoadDecorationPanel : PanelBase
    {
        private CheckboxPanel nodeMarkingsHiddenCheckbox;

        public override void OnDestroy()
        {
            nodeMarkingsHiddenCheckbox.EventCheckboxStateChanged -= NetworkSkinPanelController.RoadDecoration.SetNodeMarkingsHidden;
            base.OnDestroy();
        }

        public override void Build(PanelType panelType, Layout layout)
        {
            base.Build(panelType, layout);
            color = GUIColor;
            UIUtil.CreateSpace(1.0f, 3.0f, this);
            CreateNodeMarkingsHiddenCheckbox();
            UIUtil.CreateSpace(1.0f, 10.0f, this);
            autoFitChildrenHorizontally = true;
        }

        protected override void RefreshUI(NetInfo netInfo)
        {
            nodeMarkingsHiddenCheckbox.SetChecked(NetworkSkinPanelController.RoadDecoration.NodeMarkingsHidden);
        }

        private void CreateNodeMarkingsHiddenCheckbox()
        {
            nodeMarkingsHiddenCheckbox = AddUIComponent<CheckboxPanel>();
            nodeMarkingsHiddenCheckbox.Build(PanelType.None, new Layout(new Vector2(0.0f, 28.0f), true, LayoutDirection.Horizontal, LayoutStart.TopLeft, 10));
            nodeMarkingsHiddenCheckbox.Initialize(
                NetworkSkinPanelController.RoadDecoration.NodeMarkingsHidden,
                Translation.Instance.GetTranslation(TranslationID.LABEL_HIDE_NODE_MARKINGS),
                Translation.Instance.GetTranslation(TranslationID.TOOLTIP_HIDE_NODE_MARKINGS));
            nodeMarkingsHiddenCheckbox.EventCheckboxStateChanged += NetworkSkinPanelController.RoadDecoration.SetNodeMarkingsHidden;
        }
    }
}

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
        private CheckboxPanel arrowsHiddenCheckbox;
        private CheckboxPanel signsHiddenCheckbox;
        private CheckboxPanel decorationHiddenCheckbox;
        private CheckboxPanel transportStopsHiddenCheckbox;
        private CheckboxPanel trafficLightsHiddenCheckbox;

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

        protected override void RefreshUI(NetInfo netInfo) {
            nodeMarkingsHiddenCheckbox.isVisible = NetworkSkinPanelController.RoadDecoration.CanHideNodeMarkings;
            nodeMarkingsHiddenCheckbox.SetChecked(NetworkSkinPanelController.RoadDecoration.NodeMarkingsHidden);

            arrowsHiddenCheckbox.isVisible = NetworkSkinPanelController.RoadDecoration.HasArrows;
            arrowsHiddenCheckbox.SetChecked(NetworkSkinPanelController.RoadDecoration.ArrowsHidden);

            signsHiddenCheckbox.isVisible = NetworkSkinPanelController.RoadDecoration.HasSigns;
            signsHiddenCheckbox.SetChecked(NetworkSkinPanelController.RoadDecoration.SignsHidden);

            decorationHiddenCheckbox.isVisible = NetworkSkinPanelController.RoadDecoration.HasDecoration;
            decorationHiddenCheckbox.SetChecked(NetworkSkinPanelController.RoadDecoration.DecorationHidden);

            transportStopsHiddenCheckbox.isVisible = NetworkSkinPanelController.RoadDecoration.HasTransportStops;
            transportStopsHiddenCheckbox.SetChecked(NetworkSkinPanelController.RoadDecoration.TransportStopsHidden);

            trafficLightsHiddenCheckbox.isVisible = NetworkSkinPanelController.RoadDecoration.HasTrafficLights;
            trafficLightsHiddenCheckbox.SetChecked(NetworkSkinPanelController.RoadDecoration.TrafficLightsHidden);
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
            nodeMarkingsHiddenCheckbox.isVisible = NetworkSkinPanelController.RoadDecoration.CanHideNodeMarkings;

            arrowsHiddenCheckbox = AddUIComponent<CheckboxPanel>();
            arrowsHiddenCheckbox.Build(PanelType.None, new Layout(new Vector2(0.0f, 28.0f), true, LayoutDirection.Horizontal, LayoutStart.TopLeft, 10));
            arrowsHiddenCheckbox.Initialize(
                NetworkSkinPanelController.RoadDecoration.ArrowsHidden,
                Translation.Instance.GetTranslation(TranslationID.LABEL_HIDE_ROAD_ARROWS),
                Translation.Instance.GetTranslation(TranslationID.TOOLTIP_HIDE_ROAD_ARROWS));
            arrowsHiddenCheckbox.EventCheckboxStateChanged += NetworkSkinPanelController.RoadDecoration.SetArrowsHidden;
            arrowsHiddenCheckbox.isVisible = NetworkSkinPanelController.RoadDecoration.HasArrows;

            signsHiddenCheckbox = AddUIComponent<CheckboxPanel>();
            signsHiddenCheckbox.Build(PanelType.None, new Layout(new Vector2(0.0f, 28.0f), true, LayoutDirection.Horizontal, LayoutStart.TopLeft, 10));
            signsHiddenCheckbox.Initialize(
                NetworkSkinPanelController.RoadDecoration.SignsHidden,
                Translation.Instance.GetTranslation(TranslationID.LABEL_HIDE_ROAD_SIGNS),
                Translation.Instance.GetTranslation(TranslationID.TOOLTIP_HIDE_ROAD_SIGNS));
            signsHiddenCheckbox.EventCheckboxStateChanged += NetworkSkinPanelController.RoadDecoration.SetSignsHidden;
            signsHiddenCheckbox.isVisible = NetworkSkinPanelController.RoadDecoration.HasSigns;

            decorationHiddenCheckbox = AddUIComponent<CheckboxPanel>();
            decorationHiddenCheckbox.Build(PanelType.None, new Layout(new Vector2(0.0f, 28.0f), true, LayoutDirection.Horizontal, LayoutStart.TopLeft, 10));
            decorationHiddenCheckbox.Initialize(
                NetworkSkinPanelController.RoadDecoration.SignsHidden,
                Translation.Instance.GetTranslation(TranslationID.LABEL_HIDE_ROAD_DECORATION),
                Translation.Instance.GetTranslation(TranslationID.TOOLTIP_HIDE_ROAD_DECORATION));
            decorationHiddenCheckbox.EventCheckboxStateChanged += NetworkSkinPanelController.RoadDecoration.SetDecorationHidden;
            decorationHiddenCheckbox.isVisible = NetworkSkinPanelController.RoadDecoration.HasDecoration;

            transportStopsHiddenCheckbox = AddUIComponent<CheckboxPanel>();
            transportStopsHiddenCheckbox.Build(PanelType.None, new Layout(new Vector2(0.0f, 28.0f), true, LayoutDirection.Horizontal, LayoutStart.TopLeft, 10));
            transportStopsHiddenCheckbox.Initialize(
                NetworkSkinPanelController.RoadDecoration.TransportStopsHidden,
                Translation.Instance.GetTranslation(TranslationID.LABEL_HIDE_TRANSPORT_STOPS),
                Translation.Instance.GetTranslation(TranslationID.TOOLTIP_HIDE_TRANSPORT_STOPS));
            transportStopsHiddenCheckbox.EventCheckboxStateChanged += NetworkSkinPanelController.RoadDecoration.SetTransportStopsHidden;
            transportStopsHiddenCheckbox.isVisible = NetworkSkinPanelController.RoadDecoration.HasTransportStops;

            trafficLightsHiddenCheckbox = AddUIComponent<CheckboxPanel>();
            trafficLightsHiddenCheckbox.Build(PanelType.None, new Layout(new Vector2(0.0f, 28.0f), true, LayoutDirection.Horizontal, LayoutStart.TopLeft, 10));
            trafficLightsHiddenCheckbox.Initialize(
                NetworkSkinPanelController.RoadDecoration.TransportStopsHidden,
                Translation.Instance.GetTranslation(TranslationID.LABEL_HIDE_TRAFFIC_LIGHTS),
                Translation.Instance.GetTranslation(TranslationID.TOOLTIP_HIDE_TRAFFIC_LIGHTS));
            trafficLightsHiddenCheckbox.EventCheckboxStateChanged += NetworkSkinPanelController.RoadDecoration.SetTrafficLightsHidden;
            trafficLightsHiddenCheckbox.isVisible = NetworkSkinPanelController.RoadDecoration.HasTrafficLights;
        }
    }
}

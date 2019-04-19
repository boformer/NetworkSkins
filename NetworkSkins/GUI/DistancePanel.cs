using ColossalFramework.UI;
using NetworkSkins.GUI.Abstraction;
using NetworkSkins.Locale;
using NetworkSkins.Net;
using NetworkSkins.TranslationFramework;
using UnityEngine;

namespace NetworkSkins.GUI
{
    public class DistancePanel : PanelBase
    {
        private PanelBase labelPanel;
        private UILabel labelDefault;
        private UILabel labelValue;
        private UISlider slider;

        public override void OnDestroy() {
            slider.eventMouseUp -= OnMouseUp;
            slider.eventValueChanged -= OnValueChanged;
            base.OnDestroy();
        }
        public override void Build(PanelType panelType, Layout layout) {
            base.Build(panelType, layout);
            labelPanel = AddUIComponent<PanelBase>();
            labelPanel.Build(PanelType.None, new Layout(new Vector2(0.0f, 18.0f), true, LayoutDirection.Horizontal, LayoutStart.TopLeft, 0));
            CreateLabels();
            UIUtil.CreateSpace(390.0f, 1.0f, this);
            CreateSlider();
            UIUtil.CreateSpace(390.0f, 15.0f, this);
            Refresh();
        }

        private void CreateLabels() {
            labelDefault = labelPanel.AddUIComponent<UILabel>();
            labelDefault.autoSize = false;
            labelDefault.autoHeight = true;
            labelDefault.width = 190.0f;
            labelDefault.textAlignment = UIHorizontalAlignment.Left;
            labelDefault.verticalAlignment = UIVerticalAlignment.Middle;
            labelDefault.text = Translation.Instance.GetTranslation(TranslationID.LABEL_DISTANCE);
            labelDefault.textColor = UIUtil.TextColor;
            labelDefault.font = UIUtil.Font;

            labelValue = labelPanel.AddUIComponent<UILabel>();
            labelValue.autoSize = false;
            labelValue.autoHeight = true;
            labelValue.width = 190.0f;
            labelValue.textAlignment = UIHorizontalAlignment.Right;
            labelValue.verticalAlignment = UIVerticalAlignment.Middle;
            labelValue.textColor = UIUtil.TextColor;
            labelValue.font = UIUtil.Font;
        }

        private void CreateSlider() {
            slider = AddUIComponent<UISlider>();
            slider.size = new Vector2(width - 10.0f, 5.0f);
            slider.backgroundSprite = "WhiteRect";
            slider.color = Color.black;
            slider.scrollWheelAmount = 1.0f;
            slider.minValue = 2.0f;
            slider.maxValue = 100.0f;
            slider.stepSize = 1.0f;
            slider.eventMouseUp += OnMouseUp;
            slider.eventValueChanged += OnValueChanged;

            UISprite thumb = slider.AddUIComponent<UISprite>();
            thumb.size = new Vector2(8.0f, 12.0f);
            thumb.spriteName = "WhiteRect";

            slider.thumbObject = thumb;
        }

        private void OnValueChanged(UIComponent component, float value) {
            float defaultDistance = 0.0f;
            switch (PanelType) {
                case PanelType.Trees: {
                    switch (NetworkSkinPanelController.LanePosition) {
                        case LanePosition.Left:
                            defaultDistance = NetworkSkinPanelController.LeftTree.DefaultRepeatDistance;
                            break;
                        case LanePosition.Middle:
                            defaultDistance = NetworkSkinPanelController.MiddleTree.DefaultRepeatDistance;
                            break;
                        case LanePosition.Right:
                            defaultDistance = NetworkSkinPanelController.RighTree.DefaultRepeatDistance;
                            break;
                        default: break;
                    }
                }
                break;
                case PanelType.Lights:
                    defaultDistance = NetworkSkinPanelController.StreetLight.DefaultRepeatDistance;
                    break;
                default: break;
            }
            UpdateText(defaultDistance);
        }

        private void OnMouseUp(UIComponent component, UIMouseEventParameter eventParam) {
            switch (PanelType) {
                case PanelType.Trees: {
                    switch (NetworkSkinPanelController.LanePosition) {
                        case LanePosition.Left:
                            NetworkSkinPanelController.LeftTree.SetRepeatDistance(slider.value);
                            if(Persistence.LanePositionLocked)
                                NetworkSkinPanelController.RighTree.SetRepeatDistance(slider.value);
                            break;
                        case LanePosition.Middle:
                            NetworkSkinPanelController.MiddleTree.SetRepeatDistance(slider.value);
                            break;
                        case LanePosition.Right:
                            NetworkSkinPanelController.RighTree.SetRepeatDistance(slider.value);
                            if (Persistence.LanePositionLocked)
                                NetworkSkinPanelController.LeftTree.SetRepeatDistance(slider.value);
                            break;
                        default: break;
                    }
                }
                break;
                case PanelType.Lights:
                    NetworkSkinPanelController.StreetLight.SetRepeatDistance(slider.value);
                    break;
                default: break;
            }
        }

        protected override void RefreshUI(NetInfo netInfo) {
            float defaultDistance = 0.0f;
            switch (PanelType) {
                case PanelType.Trees: {
                    switch (NetworkSkinPanelController.LanePosition) {
                        case LanePosition.Left:
                            slider.value = NetworkSkinPanelController.LeftTree.SelectedRepeatDistance;
                            defaultDistance = NetworkSkinPanelController.LeftTree.DefaultRepeatDistance;
                            break;
                        case LanePosition.Middle:
                            slider.value = NetworkSkinPanelController.MiddleTree.SelectedRepeatDistance;
                            defaultDistance = NetworkSkinPanelController.MiddleTree.DefaultRepeatDistance;
                            break;
                        case LanePosition.Right:
                            slider.value = NetworkSkinPanelController.RighTree.SelectedRepeatDistance;
                            defaultDistance = NetworkSkinPanelController.RighTree.DefaultRepeatDistance;
                            break;
                        default: break;
                    }
                } break;
                case PanelType.Lights:
                    slider.value = NetworkSkinPanelController.StreetLight.SelectedRepeatDistance;
                    defaultDistance = NetworkSkinPanelController.StreetLight.DefaultRepeatDistance;
                    isVisible = NetworkSkinPanelController.StreetLight.CanEditRepeatDistance;
                    break;
                default: break;
            }
            UpdateText(defaultDistance);
        }

        private void UpdateText(float defaultDistance) {
            labelValue.text = string.Concat(Mathf.RoundToInt(slider.value));
            labelDefault.tooltip = string.Concat(Translation.Instance.GetTranslation(TranslationID.LABEL_DEFAULT), ": ", Mathf.RoundToInt(defaultDistance));
            labelValue.tooltip = string.Concat(Translation.Instance.GetTranslation(TranslationID.LABEL_DEFAULT), ": ", Mathf.RoundToInt(defaultDistance));
        }
    }
}

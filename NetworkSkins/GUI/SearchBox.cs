using ColossalFramework.UI;
using NetworkSkins.Locale;
using NetworkSkins.TranslationFramework;
using UnityEngine;

namespace NetworkSkins.GUI
{
    public class SearchBox : PanelBase
    {
        public delegate void SearchTextChangedEventHandler(string text);
        public event SearchTextChangedEventHandler EventSearchTextChanged;

        public delegate void SearchLostFocusEventHandler();
        public event SearchLostFocusEventHandler EventLostFocus;

        private UILabel label;
        private UITextField textField;

        public override void Build(PanelType panelType, Layout layout) {
            base.Build(panelType, layout);
            textField = AddUIComponent<UITextField>();
            textField.size = new Vector2(230f, 30f);
            textField.relativePosition = new Vector3(160f, 0f);
            textField.padding = new RectOffset(6, 6, 6, 6);
            textField.builtinKeyNavigation = true;
            textField.isInteractive = true;
            textField.readOnly = false;
            textField.horizontalAlignment = UIHorizontalAlignment.Center;
            textField.selectionSprite = "EmptySprite";
            textField.selectionBackgroundColor = new Color32(0, 172, 234, 255);
            textField.normalBgSprite = "TextFieldPanelHovered";
            textField.textColor = new Color32(0, 0, 0, 255);
            textField.color = new Color32(255, 255, 255, 255);
            textField.eventTextChanged += OnSearchTextChanged;
            textField.eventLostFocus += OnSearchLostFocus;

            label = AddUIComponent<UILabel>();
            label.autoSize = true;
            label.pivot = UIPivotPoint.TopRight;
            label.anchor = UIAnchorStyle.Right;
            label.padding = new RectOffset(0, 0, 8, 0);
            label.text = Translation.Instance.GetTranslation(TranslationID.LABEL_SEARCH);
        }

        protected override void RefreshUI(NetInfo netInfo) {

        }

        private void OnSearchLostFocus(UIComponent component, UIFocusEventParameter eventParam) {
            EventLostFocus?.Invoke();
        }

        private void OnSearchTextChanged(UIComponent component, string value) {
            EventSearchTextChanged?.Invoke(value);
        }
    }
}

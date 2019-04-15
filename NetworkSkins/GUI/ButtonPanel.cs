using ColossalFramework.UI;
using NetworkSkins.GUI.Abstraction;
using UnityEngine;

namespace NetworkSkins.GUI
{
    public class ButtonPanel : PanelBase
    {
        public delegate void ButtonClickedEventHandler();
        public event ButtonClickedEventHandler EventButtonClicked;

        private UIButton button;

        public override void Build(PanelType panelType, Layout layout) {
            base.Build(panelType, layout);
            CreateButton();
            UIUtil.CreateSpace(0.0f, 10.0f, this);
        }
        private void CreateButton() {
            button = UIUtil.CreateButton(new Vector2(117.0f, 30.0f), parentComponent: this);
            button.eventClicked += OnButtonClicked;
        }

        public void SetAnchor(UIAnchorStyle anchors) {
            button.anchor = anchors;
        }

        public void SetText(string text, string tooltip = "") {
            button.text = text;
            button.tooltip = tooltip;
        }

        private void OnButtonClicked(UIComponent component, UIMouseEventParameter eventParam) {
            EventButtonClicked?.Invoke();
        }
    }
}

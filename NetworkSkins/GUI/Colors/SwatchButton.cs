using ColossalFramework.UI;
using UnityEngine;

namespace NetworkSkins.GUI.Colors
{
    public class SwatchButton : UIButton
    {
        public Color32 swatch;
        public delegate void SwatchClickedEventHandler(Color32 color, UIMouseEventParameter eventParam, UIComponent component);
        public event SwatchClickedEventHandler EventSwatchClicked;

        public override void OnDestroy() {
            EventSwatchClicked = null;
            eventClicked -= OnSwatchClicked;
            base.OnDestroy();
        }
        public override void Start() {
            eventClicked += OnSwatchClicked;
        }
        private void OnSwatchClicked(UIComponent component, UIMouseEventParameter eventParam) {
            EventSwatchClicked?.Invoke(swatch, eventParam, component);
        }
    }
}

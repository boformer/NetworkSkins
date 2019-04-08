using ColossalFramework.UI;
using NetworkSkins.Persistence;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NetworkSkins.GUI
{
    public class Swatches : IEnumerable<Swatches.Swatch>
    {
        private PersistenceService persistence;
        private Queue<Swatch> SwatchList { get; set; }
        private Queue<SwatchButton> SwatchButtonsList { get; set; }

        public int Count;

        public Swatches(int capacity, PersistenceService persistence) {
            SwatchList = new Queue<Swatch>();
            SwatchButtonsList = new Queue<SwatchButton>();
            this.Count = capacity;
            this.persistence = persistence;
        }

        public void AddSwatch(ColorPanel owner, PanelBase parent, Color32 color) {
            Ensure();
            if (!SwatchList.Contains(color)) {
                if (SwatchList.Count < Count) {
                    SwatchList.Enqueue(color);
                    SwatchButtonsList.Enqueue(((Swatch)color).MakeSwatchButton(owner, parent));
                    persistence.AddSwatch(color);
                }
            }
        }

        private void Ensure() {
            if (SwatchList == null || SwatchButtonsList == null) {
                SwatchList = new Queue<Swatch>();
                SwatchButtonsList = new Queue<SwatchButton>();
            }
        }

        IEnumerator<Swatch> IEnumerable<Swatch>.GetEnumerator() {
            return SwatchList.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return SwatchList.GetEnumerator();
        }

        public class Swatch
        {
            public byte r;
            public byte g;
            public byte b;

            public Swatch(Color32 color) {
                this.r = color.r;
                this.g = color.g;
                this.b = color.b;
            }

            public static implicit operator Color32(Swatch swatch) {
                return new Color32(swatch.r, swatch.g, swatch.b, 255);
            }

            public static implicit operator Swatch(Color32 color) {
                return new Swatch(color);
            }

            public SwatchButton MakeSwatchButton(ColorPanel owner, PanelBase parent) {
                SwatchButton button = parent.AddUIComponent<SwatchButton>();
                button.size = new Vector2(20.0f, 20.0f);
                button.atlas = Resources.Atlas;
                button.normalBgSprite = Resources.Swatch;
                button.hoveredColor = new Color32(this.r, this.g, this.b, 64);
                button.focusedColor = this;
                button.color = this;
                button.swatch = this;
                button.EventSwatchClicked += owner.OnSwatchClicked;
                return button;
            }
        }

        public class SwatchButton : UIButton
        {
            public Swatch swatch;
            public delegate void SwatchClickedEventHandler(Color32 color, UIMouseEventParameter eventParam, UIComponent component);
            public event SwatchClickedEventHandler EventSwatchClicked;

            public override void Awake() {
                eventClicked += OnSwatchClicked;
            }
            private void OnSwatchClicked(UIComponent component, UIMouseEventParameter eventParam) {
                EventSwatchClicked?.Invoke(swatch, eventParam, component);
            }
        }
    }
}

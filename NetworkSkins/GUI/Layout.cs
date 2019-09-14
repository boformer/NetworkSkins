using ColossalFramework.UI;
using NetworkSkins.GUI.Abstraction;
using UnityEngine;

namespace NetworkSkins.GUI
{
    public struct Layout {
        public Vector2 Size { get; set; }
        public bool AutoLayout { get; set; }
        public LayoutDirection AutoLayoutDirection { get; set; }
        public LayoutStart AutoLayoutStart { get; set; }
        public int Spacing { get; set; }
        public string BackgroundSprite { get; set; }

        public Layout(Vector2 size, bool autoLayout, LayoutDirection autoLayoutDirection, LayoutStart autoLayoutStart, int spacing, string backgroundSprite = "") {
            Size = size;
            AutoLayout = autoLayout;
            AutoLayoutDirection = autoLayoutDirection;
            AutoLayoutStart = autoLayoutStart;
            Spacing = spacing;
            BackgroundSprite = backgroundSprite;
        }

        public void Apply(PanelBase panel) {
            panel.size = Size;
            panel.autoLayout = AutoLayout;
            panel.autoLayoutDirection = AutoLayoutDirection;
            switch (AutoLayoutDirection) {
                case LayoutDirection.Horizontal: panel.autoFitChildrenHorizontally = true; break;
                case LayoutDirection.Vertical: panel.autoFitChildrenVertically = true; break;
                default: break;
            }
            panel.autoLayoutStart = AutoLayoutStart;
            panel.Spacing = Spacing;
            panel.backgroundSprite = BackgroundSprite;
            switch (AutoLayoutStart) {
                case LayoutStart.TopLeft:
                    panel.padding = new RectOffset(Spacing, 0, Spacing, 0);
                    panel.autoLayoutPadding = new RectOffset(0, Spacing, 0, Spacing);
                    break;

                case LayoutStart.BottomLeft:
                    panel.padding = new RectOffset(Spacing, 0, 0, Spacing);
                    panel.autoLayoutPadding = new RectOffset(0, Spacing, Spacing, 0);
                    break;

                case LayoutStart.TopRight:
                    panel.padding = new RectOffset(0, Spacing, 0, Spacing);
                    panel.autoLayoutPadding = new RectOffset(Spacing, 0, Spacing, 0);
                    break;

                case LayoutStart.BottomRight:
                    panel.padding = new RectOffset(Spacing, 0, 0, Spacing);
                    panel.autoLayoutPadding = new RectOffset(0, Spacing, Spacing, 0);
                    break;
            }
        }
    }
}
using System;
using ColossalFramework.UI;
using UnityEngine;

namespace NetworkSkins.GUI
{
    public class MainPanel : PanelBase
    {
        private ToolBar toolBar;
        public static NetInfo Prefab { get; set; }

        public override void Start() {
            Build(new Layout(new Vector2(0.0f, 234.0f), true, LayoutDirection.Horizontal, LayoutStart.TopLeft, 0, "GenericPanel"));
            relativePosition = new Vector3(100.0f, 100.0f);
            color = new Color32(128, 128, 128, 255);
            autoFitChildrenVertically = true;
            MakeToolbar();
        }

        private void MakeToolbar() {
            toolBar = AddUIComponent<ToolBar>();
            toolBar.Build(new Layout(new Vector2(40.0f, 0.0f), true, LayoutDirection.Vertical, LayoutStart.TopLeft, 0));
        }
    }
}

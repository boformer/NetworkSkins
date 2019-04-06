using System;
using ColossalFramework.UI;
using NetworkSkins.Locale;
using NetworkSkins.Net;
using NetworkSkins.TranslationFramework;
using UnityEngine;

namespace NetworkSkins.GUI
{
    public abstract class ListPanelBase : PanelBase
    {
        protected UITabstrip tabStrip;
        protected UIButton[] tabs;
        protected SearchBox searchBox;

        public override void Build(PanelType panelType, Layout layout) {
            base.Build(panelType, layout);
            color = MainPanel.GUIColor;

            CreateTabstrip();
            SetupTabs();
            CreateList();
            CreateSearchBox();
            CreateSpace(width - (Spacing * 2), 0.1f);
            OnPanelBuilt();
        }

        protected abstract void CreateList();

        protected abstract void OnPanelBuilt();

        protected abstract void OnSearchTextChanged(string text);

        protected abstract void OnSearchLostFocus();

        protected abstract void OnFavouriteChanged(string itemID, bool favourite);

        protected abstract void OnSelectedChanged(string itemID, bool selected);

        private void CreateTabstrip() {
            tabStrip = AddUIComponent<UITabstrip>();
            tabStrip.builtinKeyNavigation = true;
            tabStrip.size = new Vector2(width - (Spacing * 2.0f), 30.0f);
        }

        private void SetupTabs() {
            tabs = new UIButton[(int)LanePosition.Count];
            for (int i = 0; i < tabs.Length; i++) {
                string lane = string.Empty;
                switch ((LanePosition)i) {
                    case LanePosition.Left: lane = Translation.Instance.GetTranslation(TranslationID.LABEL_LEFTLANE); break;
                    case LanePosition.Middle: lane = Translation.Instance.GetTranslation(TranslationID.LABEL_MIDDLELANE); break;
                    case LanePosition.Right: lane = Translation.Instance.GetTranslation(TranslationID.LABEL_RIGHTLANE); break;
                    default: break;
                }
                tabs[i] = UIUtil.CreateButton(Vector2.zero, lane, backgroundSprite: "GenericTab", parentComponent: tabStrip, isFocusable: true);
                tabs[i].color = tabs[i].focusedColor = new Color32(210, 210, 210, 255);
                tabs[i].size = new Vector2(tabStrip.width / 3.0f, tabStrip.height);
            }
        }

        private void CreateSearchBox() {
            //searchBox = AddUIComponent<SearchBox>();
            //searchBox.Build(new Layout(new Vector2(390.0f, 50.0f), true, LayoutDirection.Horizontal, LayoutStart.TopRight, Spacing));
            //searchBox.EventSearchTextChanged += OnSearchTextChanged;
            //searchBox.EventLostFocus += OnSearchLostFocus;
        }
    }
}

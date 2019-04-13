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
        protected UITabstrip laneTabStrip;
        protected UITabstrip pillarTabStrip;
        protected UIButton[] laneTabs;
        protected UIButton[] pillarTabs;
        protected SearchBox searchBox;

        public override void Build(PanelType panelType, Layout layout) {
            base.Build(panelType, layout);
            color = MainPanel.GUIColor;

            CreatePillarTabstrip();
            SetupPillarTabs();
            CreateLaneTabstrip();
            SetupLaneTabs();
            CreateList();
            CreateSearchBox();
            UIUtil.CreateSpace(width - (Spacing * 2), 0.1f, this);
            OnPanelBuilt();
        }

        protected abstract void CreateList();

        protected abstract void OnPanelBuilt();

        protected abstract void OnSearchTextChanged(string text);

        protected abstract void OnSearchLostFocus();

        protected abstract void OnFavouriteChanged(string itemID, bool favourite);

        protected abstract void OnSelectedChanged(string itemID, bool selected);

        private void CreateLaneTabstrip() {
            laneTabStrip = AddUIComponent<UITabstrip>();
            laneTabStrip.builtinKeyNavigation = true;
            laneTabStrip.size = new Vector2(width - (Spacing * 2.0f), 30.0f);
            laneTabStrip.eventSelectedIndexChanged += OnLaneTabstripSelectedIndexChanged;
        }

        private void CreatePillarTabstrip() {
            pillarTabStrip = AddUIComponent<UITabstrip>();
            pillarTabStrip.builtinKeyNavigation = true;
            pillarTabStrip.size = new Vector2(width - (Spacing * 2.0f), 30.0f);
            pillarTabStrip.eventSelectedIndexChanged += OnPillarTabstripSelectedIndexChanged;
        }

        private void OnPillarTabstripSelectedIndexChanged(UIComponent component, int value) {
            SkinController.PillarElevationCombination = (Pillar)value;
        }

        private void OnLaneTabstripSelectedIndexChanged(UIComponent component, int value) {
            SkinController.LaneTabClicked = true;
            SkinController.SetActiveLane((LanePosition)value);
        }

        private void SetupLaneTabs() {
            laneTabs = new UIButton[LanePositionExtensions.LanePositionCount];
            for (int i = 0; i < laneTabs.Length; i++) {
                string lane = string.Empty;
                switch ((LanePosition)i) {
                    case LanePosition.Left: lane = Translation.Instance.GetTranslation(TranslationID.LABEL_LEFTLANE); break;
                    case LanePosition.Middle: lane = Translation.Instance.GetTranslation(TranslationID.LABEL_MIDDLELANE); break;
                    case LanePosition.Right: lane = Translation.Instance.GetTranslation(TranslationID.LABEL_RIGHTLANE); break;
                    default: break;
                }
                laneTabs[i] = UIUtil.CreateButton(Vector2.zero, lane, backgroundSprite: "GenericTab", parentComponent: laneTabStrip, isFocusable: true);
                laneTabs[i].color = laneTabs[i].focusedColor = new Color32(210, 210, 210, 255);
                laneTabs[i].size = new Vector2(laneTabStrip.width / LanePositionExtensions.LanePositionCount, laneTabStrip.height);
            }
        }

        private void SetupPillarTabs() {
            pillarTabs = new UIButton[(int)Pillar.Count];
            for (int i = 0; i < pillarTabs.Length; i++) {
                string pillarElevationCombination = string.Empty;
                switch ((Pillar)i) {
                    case Pillar.Elevated: pillarElevationCombination = Translation.Instance.GetTranslation(TranslationID.LABEL_ELEVATED); break;
                    case Pillar.ElevatedMiddle: pillarElevationCombination = Translation.Instance.GetTranslation(TranslationID.LABEL_ELEVATEDMIDDLE); break;
                    case Pillar.Bridge: pillarElevationCombination = Translation.Instance.GetTranslation(TranslationID.LABEL_BRIDGE); break;
                    case Pillar.BridgeMiddle: pillarElevationCombination =  Translation.Instance.GetTranslation(TranslationID.LABEL_BRIDGEMIDDLE); break;
                }
                pillarTabs[i] = UIUtil.CreateButton(Vector2.zero, pillarElevationCombination, backgroundSprite: "GenericTab", parentComponent: pillarTabStrip, isFocusable: true);
                pillarTabs[i].color = pillarTabs[i].focusedColor = new Color32(210, 210, 210, 255);
                pillarTabs[i].size = new Vector2(pillarTabStrip.width / (int)Pillar.Count, pillarTabStrip.height);
            }
        }

        private void CreateSearchBox() {
            
        }
    }
}

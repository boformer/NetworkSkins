using ColossalFramework.UI;
using NetworkSkins.Locale;
using NetworkSkins.Net;
using NetworkSkins.TranslationFramework;
using UnityEngine;

namespace NetworkSkins.GUI.Abstraction
{
    public abstract class ListPanelBase : PanelBase
    {
        protected PanelBase laneTabstripContainer;
        protected UIButton lockButton;
        protected UITabstrip laneTabstrip;
        protected UITabstrip pillarTabstrip;
        protected UIButton[] laneTabs;
        protected UIButton[] pillarTabs;
        protected SearchBox searchBox;
        protected bool _ignoreEvents = false;

        public override void Build(PanelType panelType, Layout layout) {
            base.Build(panelType, layout);
            color = GUIColor;
            CreateTabstripContainer();
            CreatePillarTabstrip();
            SetupPillarTabs();
            CreateLaneTabstrip();
            SetupLaneTabs();
            UIUtil.CreateSpace(5.0f, 30.0f, laneTabstripContainer);
            CreateLockButton();
            CreateList();
            UIUtil.CreateSpace(width - (Spacing * 2), 0.1f, this);
            OnPanelBuilt();
            if (Persistence.LanePositionLocked) LockLaneTabs();
        }

        protected abstract void CreateList();

        protected abstract void OnPanelBuilt();

        
        protected void OnFavouriteChanged(string itemID, bool favourite) {
            if (favourite) Persistence.AddFavourite(itemID, UIUtil.PanelToItemType(PanelType));
            else Persistence.RemoveFavourite(itemID, UIUtil.PanelToItemType(PanelType));
        }

        public override void OnDestroy() {
            laneTabstrip.eventSelectedIndexChanged -= OnLaneTabstripSelectedIndexChanged;
            pillarTabstrip.eventSelectedIndexChanged -= OnPillarTabstripSelectedIndexChanged;
            lockButton.eventClicked -= OnLockClicked;
            base.OnDestroy();
        }
        private void CreateTabstripContainer() {
            laneTabstripContainer = AddUIComponent<PanelBase>();
            laneTabstripContainer.Build(PanelType.None, new Layout(new Vector2(width - (Spacing * 2.0f), 30.0f), true, LayoutDirection.Horizontal, LayoutStart.TopLeft, 0));
        }

        private void CreateLaneTabstrip() {
            laneTabstrip = laneTabstripContainer.AddUIComponent<UITabstrip>();
            laneTabstrip.builtinKeyNavigation = true;
            laneTabstrip.size = new Vector2(width - (Spacing * 2.0f) - 35.0f, 30.0f);
            laneTabstrip.eventSelectedIndexChanged += OnLaneTabstripSelectedIndexChanged;
            laneTabstrip.atlas = Sprites.DefaultAtlas;
        }

        private void CreatePillarTabstrip() {
            pillarTabstrip = AddUIComponent<UITabstrip>();
            pillarTabstrip.builtinKeyNavigation = true;
            pillarTabstrip.size = new Vector2(width - (Spacing * 2.0f), 30.0f);
            pillarTabstrip.eventSelectedIndexChanged += OnPillarTabstripSelectedIndexChanged;
            pillarTabstrip.atlas = Sprites.DefaultAtlas;
        }

        private void CreateLockButton() {
            Vector2 buttonSize = new Vector2(30.0f, 30.0f);
            lockButton = UIUtil.CreateButton(buttonSize, parentComponent: laneTabstripContainer, backgroundSprite: Sprites.Unlocked, atlas: Sprites.Atlas, isFocusable: true, tooltip: Translation.Instance.GetTranslation(TranslationID.TOOLTIP_LOCK));
            lockButton.eventClicked += OnLockClicked;
        }

        private void OnLockClicked(UIComponent component, UIMouseEventParameter eventParam) {
            Persistence.LanePositionLocked = !Persistence.LanePositionLocked;
            if (Persistence.LanePositionLocked) LockLaneTabs();
            else UnlockLaneTabs();
            NetworkSkinPanelController.SetLaneAndRefreshUI(NetworkSkinPanelController.LanePosition);
        }

        private void UnlockLaneTabs() {
            lockButton.normalBgSprite = Sprites.Unlocked;
            lockButton.hoveredBgSprite = Sprites.UnlockedHovered;
            lockButton.pressedBgSprite = Sprites.UnlockedPressed;
            for (int i = 0; i < laneTabs.Length; i++) {
                laneTabs[i].Enable();
            }
        }

        private void LockLaneTabs() {
            lockButton.normalBgSprite = Sprites.Locked;
            lockButton.hoveredBgSprite = Sprites.Locked;
            lockButton.pressedBgSprite = Sprites.Locked;
            for (int i = 0; i < laneTabs.Length; i++) {
                laneTabs[i].Disable();
            }
        }

        private void OnPillarTabstripSelectedIndexChanged(UIComponent component, int value) {
            if (_ignoreEvents) return;
            NetworkSkinPanelController.SetPillarAndRefreshUI((Pillar)value);
        }

        private void OnLaneTabstripSelectedIndexChanged(UIComponent component, int value) {
            if (_ignoreEvents) return;
            NetworkSkinPanelController.SetLaneAndRefreshUI((LanePosition)value);
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
                laneTabs[i] = UIUtil.CreateButton(Vector2.zero, lane, backgroundSprite: "GenericTab", parentComponent: laneTabstrip, isFocusable: true);
                laneTabs[i].color = laneTabs[i].focusedColor =  new Color32(210, 210, 210, 255);
                laneTabs[i].disabledBgSprite = laneTabs[i].focusedBgSprite;
                laneTabs[i].disabledTextColor = laneTabs[i].disabledColor = new Color32(165, 165, 165, 255);
                laneTabs[i].size = new Vector2(laneTabstrip.width / LanePositionExtensions.LanePositionCount, laneTabstrip.height);
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
                pillarTabs[i] = UIUtil.CreateButton(Vector2.zero, pillarElevationCombination, backgroundSprite: "GenericTab", parentComponent: pillarTabstrip, isFocusable: true);
                pillarTabs[i].color = pillarTabs[i].focusedColor = new Color32(210, 210, 210, 255);
                pillarTabs[i].size = new Vector2(pillarTabstrip.width / (int)Pillar.Count, pillarTabstrip.height);
            }
        }
    }
}

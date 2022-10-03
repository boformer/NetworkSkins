﻿using ColossalFramework.UI;
using NetworkSkins.GUI.Abstraction;
using NetworkSkins.Locale;
using NetworkSkins.TranslationFramework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace NetworkSkins.GUI
{
    using NetworkSkins.API;
    using System;

    public class ButtonBar : PanelBase
    {
        public delegate void ButtonClickedEventHandler(UIButton focusedButton, UIButton[] buttons);
        public delegate void CustomButtonClickedEventHandler(UIButton focusedButton, NSImplementationWrapper impl, UIButton[] buttons);
        public event ButtonClickedEventHandler EventTreesClicked;
        public event ButtonClickedEventHandler EventLightsClicked;
        public event ButtonClickedEventHandler EventSurfacesClicked;
        public event ButtonClickedEventHandler EventPillarsClicked;
        public event ButtonClickedEventHandler EventCatenaryClicked;
        public event ButtonClickedEventHandler EventColorClicked;
        public event ButtonClickedEventHandler EventExtrasClicked;
        public event ButtonClickedEventHandler EventPipetteClicked;
        public event ButtonClickedEventHandler EventRoadDecorationClicked;
        public event ButtonClickedEventHandler EventResetClicked;
        public event CustomButtonClickedEventHandler EventCustomClicked;

        public delegate void ButtonVisibilityChangedEventHandler(UIButton focusedButton, UIButton[] buttons, bool visible);
        public delegate void CustomButtonVisibilityChangedEventHandler(UIButton focusedButton, NSImplementationWrapper impl, UIButton[] buttons, bool visible);
        public event ButtonVisibilityChangedEventHandler EventTreesVisibilityChanged;
        public event ButtonVisibilityChangedEventHandler EventLightsVisibilityChanged;
        public event ButtonVisibilityChangedEventHandler EventSurfacesVisibilityChanged;
        public event ButtonVisibilityChangedEventHandler EventPillarsVisibilityChanged;
        public event ButtonVisibilityChangedEventHandler EventCatenaryVisibilityChanged;
        public event ButtonVisibilityChangedEventHandler EventColorVisibilityChanged;
        public event ButtonVisibilityChangedEventHandler EventRoadDecorationVisibilityChanged;
        public event ButtonVisibilityChangedEventHandler EventSettingsVisibilityChanged;
        public event CustomButtonVisibilityChangedEventHandler EventCustomVisibilityChanged;

        private UIButton treesButton;
        private UIButton lightsButton;
        private UIButton surfacesButton;
        private UIButton pillarsButton;
        private UIButton catenaryButton;
        private UIButton colorButton;
        private UIButton roadDecorationButton;
        private UIButton settingsButton;
        private UIButton pipetteButton;
        private UIButton resetButton;
        public  List<UIButton> CustomButtons;

        private UIButton[] buttons;

        private Vector2 ButtonSize => new Vector2(Layout.Size.x - Layout.Spacing * 2, Layout.Size.x - Layout.Spacing * 2);

        public override void OnDestroy() {
            // TODO: this code is redundant because all events are automatically cleared by UIComponent.RemoveAllEventHandlers during destruction
            treesButton.eventClicked -= OnTreesButtonClicked;
            lightsButton.eventClicked -= OnLightsButtonClicked;
            surfacesButton.eventClicked -= OnSurfacesButtonClicked;
            pillarsButton.eventClicked -= OnPillarsButtonClicked;
            catenaryButton.eventClicked -= OnCatenaryButtonClicked;
            colorButton.eventClicked -= OnColorButtonClicked;
            roadDecorationButton.eventClicked -= OnRoadDecorationButtonClicked;
            settingsButton.eventClicked -= OnSettingsButtonClicked;
            resetButton.eventClicked -= OnResetButtonClicked;
            treesButton.eventVisibilityChanged -= OnTreesButtonVisibilityChanged;
            lightsButton.eventVisibilityChanged -= OnLightsButtonVisibilityChanged;
            surfacesButton.eventVisibilityChanged -= OnSurfacesButtonVisibilityChanged;
            pillarsButton.eventVisibilityChanged -= OnPillarsButtonVisibilityChanged;
            catenaryButton.eventVisibilityChanged -= OnCatenaryButtonVisibilityChanged;
            colorButton.eventVisibilityChanged -= OnColorButtonVisibilityChanged;
            roadDecorationButton.eventVisibilityChanged -= OnRoadDecorationButtonVisibilityChanged;
            settingsButton.eventVisibilityChanged -= OnSettingsButtonVisibilityChanged;
            base.OnDestroy();
        }

        public override void Build(PanelType panelType, Layout layout) {
            base.Build(panelType, layout);
            CreateButtons();
            UIPanel space = AddUIComponent<UIPanel>();
            space.size = new Vector2(width, 1);
            Refresh();
        }

        public void RemoveCustomButton(string id) {
            var button = CustomButtons.Find(button => button.objectUserData is NSImplementationWrapper wrapper && wrapper.ID == id);
            if (button != null) {
                Destroy(button.gameObject);
                CustomButtons.Remove(button);
            }
        }

        public void AddCustomButton(NSImplementationWrapper impl) {
            var button = UIUtil.CreateButton(
                size: ButtonSize,
                parentComponent: this,
                foregroundSprite: NSImplementationWrapper.ForegroundIconName,
                backgroundSprite: Resources.ButtonSmall,
                atlas: impl.Atlas,
                isFocusable: true,
                tooltip: impl.Tooltip);
            CustomButtons.Add(button);
            button.objectUserData = impl;
            button.eventClicked += OnCustomButtonClicked;
            button.eventVisibilityChanged += OnCustomButtonVisibilityChanged;
        }

        protected override void RefreshUI(NetInfo netInfo) {
            try {
                this.enabled = false; // work around: avoid long layout processing time
                treesButton.isVisible = NetworkSkinPanelController.TreesEnabled;
                lightsButton.isVisible = NetworkSkinPanelController.StreetLight.Enabled;
                surfacesButton.isVisible = NetworkSkinPanelController.TerrainSurface.Enabled;
                pillarsButton.isVisible = NetworkSkinPanelController.PillarsEnabled;
                catenaryButton.isVisible = NetworkSkinPanelController.Catenary.Enabled;
                colorButton.isVisible = NetworkSkinPanelController.Color.Enabled;
                roadDecorationButton.isVisible = NetworkSkinPanelController.RoadDecoration.Enabled;
                foreach(var button in CustomButtons) {
                    var impl = button.objectUserData as NSImplementationWrapper;
                    var controller = NetworkSkinPanelController.CustomPanelControllers[impl.ID];
                    button.isVisible = controller.Enabled;
                }
            } catch(Exception ex) {
                Debug.LogException(ex);
            } finally {
                this.enabled = true;
            }
        }

        private void CreateButtons() {
            Vector2 buttonSize = ButtonSize;

            surfacesButton = UIUtil.CreateButton(buttonSize, parentComponent: this, backgroundSprite: Resources.Surface, atlas: Resources.Atlas, isFocusable: true, tooltip: Translation.Instance.GetTranslation(TranslationID.TOOLTIP_SURFACE));
            surfacesButton.eventClicked += OnSurfacesButtonClicked;
            surfacesButton.eventVisibilityChanged += OnSurfacesButtonVisibilityChanged;

            colorButton = UIUtil.CreateButton(buttonSize, parentComponent: this, backgroundSprite: Resources.Color, atlas: Resources.Atlas, isFocusable: true, tooltip: Translation.Instance.GetTranslation(TranslationID.TOOLTIP_COLOR));
            colorButton.eventClicked += OnColorButtonClicked;
            colorButton.eventVisibilityChanged += OnColorButtonVisibilityChanged;
            
            roadDecorationButton = UIUtil.CreateButton(buttonSize, parentComponent: this, backgroundSprite: Resources.RoadDecoration, atlas: Resources.Atlas, isFocusable: true, tooltip: Translation.Instance.GetTranslation(TranslationID.TOOLTIP_ROAD_DECORATION));
            roadDecorationButton.eventClicked += OnRoadDecorationButtonClicked;
            roadDecorationButton.eventVisibilityChanged += OnRoadDecorationButtonVisibilityChanged;

            lightsButton = UIUtil.CreateButton(buttonSize, parentComponent: this, backgroundSprite: Resources.Light, atlas: Resources.Atlas, isFocusable: true, tooltip: Translation.Instance.GetTranslation(TranslationID.TOOLTIP_LIGHTS));
            lightsButton.eventClicked += OnLightsButtonClicked;
            lightsButton.eventVisibilityChanged += OnLightsButtonVisibilityChanged;

            catenaryButton = UIUtil.CreateButton(buttonSize, parentComponent: this, backgroundSprite: Resources.Catenary, atlas: Resources.Atlas, isFocusable: true, tooltip: Translation.Instance.GetTranslation(TranslationID.TOOLTIP_CATENARY));
            catenaryButton.eventClicked += OnCatenaryButtonClicked;
            catenaryButton.eventVisibilityChanged += OnCatenaryButtonVisibilityChanged;

            treesButton = UIUtil.CreateButton(buttonSize, parentComponent: this, backgroundSprite: Resources.Tree, atlas: Resources.Atlas, isFocusable: true, tooltip: Translation.Instance.GetTranslation(TranslationID.TOOLTIP_TREES));
            treesButton.eventClicked += OnTreesButtonClicked;
            treesButton.eventVisibilityChanged += OnTreesButtonVisibilityChanged;

            pillarsButton = UIUtil.CreateButton(buttonSize, parentComponent: this, backgroundSprite: Resources.Pillar, atlas: Resources.Atlas, isFocusable: true, tooltip: Translation.Instance.GetTranslation(TranslationID.TOOLTIP_PILLARS));
            pillarsButton.eventClicked += OnPillarsButtonClicked;
            pillarsButton.eventVisibilityChanged += OnPillarsButtonVisibilityChanged;

            resetButton = UIUtil.CreateButton(buttonSize, parentComponent: this, backgroundSprite: Resources.Undo, atlas: Resources.Atlas, isFocusable: false, tooltip: Translation.Instance.GetTranslation(TranslationID.TOOLTIP_RESETCURRENT));
            resetButton.eventClicked += OnResetButtonClicked;

            UIPanel panel = AddUIComponent<UIPanel>();
            panel.size = new Vector2(30.0f, 4.0f);
            panel.atlas = Resources.DefaultAtlas;
            panel.backgroundSprite = "WhiteRect";
            panel.color = new Color32(53, 54, 54, 255);

            settingsButton = UIUtil.CreateButton(buttonSize, parentComponent: this, backgroundSprite: Resources.Settings, atlas: Resources.Atlas, isFocusable: true, tooltip: Translation.Instance.GetTranslation(TranslationID.TOOLTIP_SETTINGS));
            settingsButton.eventClicked += OnSettingsButtonClicked;
            settingsButton.eventVisibilityChanged += OnSettingsButtonVisibilityChanged;

            pipetteButton = UIUtil.CreateButton(buttonSize, parentComponent: this, backgroundSprite: Resources.Pipette, atlas: Resources.Atlas, isFocusable: true, tooltip: Translation.Instance.GetTranslation(TranslationID.TOOLTIP_PIPETTE));
            pipetteButton.eventClicked += OnPipetteButtonClicked;

            CustomButtons = new List<UIButton>();
            foreach(var impl in NSAPI.Instance.ActiveImplementationWrappers) {
                AddCustomButton(impl);
            }

            CreateButtonArray();
        }

        private void CreateButtonArray() {
            buttons = new UIButton[] {
                treesButton,
                lightsButton,
                surfacesButton,
                pillarsButton,
                catenaryButton,
                colorButton,
                roadDecorationButton,
                settingsButton,
                pipetteButton,
                resetButton
            };
            buttons = buttons.Concat(CustomButtons).ToArray();
        }

        private void OnTreesButtonVisibilityChanged(UIComponent component, bool value) {
            EventTreesVisibilityChanged?.Invoke(component as UIButton, buttons, value);
        }

        private void OnLightsButtonVisibilityChanged(UIComponent component, bool value) {
            EventLightsVisibilityChanged?.Invoke(component as UIButton, buttons, value);
        }

        private void OnSurfacesButtonVisibilityChanged(UIComponent component, bool value) {
            EventSurfacesVisibilityChanged?.Invoke(component as UIButton, buttons, value);
        }

        private void OnPillarsButtonVisibilityChanged(UIComponent component, bool value) {
            EventPillarsVisibilityChanged?.Invoke(component as UIButton, buttons, value);
        }

        private void OnCatenaryButtonVisibilityChanged(UIComponent component, bool value) {
            EventCatenaryVisibilityChanged?.Invoke(component as UIButton, buttons, value);
        }

        private void OnColorButtonVisibilityChanged(UIComponent component, bool value) {
            EventColorVisibilityChanged?.Invoke(component as UIButton, buttons, value);
        }

        private void OnRoadDecorationButtonVisibilityChanged(UIComponent component, bool value) {
            EventRoadDecorationVisibilityChanged?.Invoke(component as UIButton, buttons, value);
        }

        private void OnSettingsButtonVisibilityChanged(UIComponent component, bool value) {
            EventSettingsVisibilityChanged?.Invoke(component as UIButton, buttons, value);
        }

        private void OnCustomButtonVisibilityChanged(UIComponent component, bool value) {
            EventCustomVisibilityChanged?.Invoke(component as UIButton, component.objectUserData as NSImplementationWrapper, buttons, value);
        }

        private void OnTreesButtonClicked(UIComponent component, UIMouseEventParameter eventParam) {
            EventTreesClicked?.Invoke(component as UIButton, buttons);
            treesButton.RefreshTooltip();
        }

        private void OnLightsButtonClicked(UIComponent component, UIMouseEventParameter eventParam) {
            EventLightsClicked?.Invoke(component as UIButton, buttons);
            lightsButton.RefreshTooltip();
        }

        private void OnSurfacesButtonClicked(UIComponent component, UIMouseEventParameter eventParam) {
            EventSurfacesClicked?.Invoke(component as UIButton, buttons);
            surfacesButton.RefreshTooltip();
        }

        private void OnPillarsButtonClicked(UIComponent component, UIMouseEventParameter eventParam) {
            EventPillarsClicked?.Invoke(component as UIButton, buttons);
            pillarsButton.RefreshTooltip();
        }

        private void OnCatenaryButtonClicked(UIComponent component, UIMouseEventParameter eventParam) {
            EventCatenaryClicked?.Invoke(component as UIButton, buttons);
            catenaryButton.RefreshTooltip();
        }

        private void OnColorButtonClicked(UIComponent component, UIMouseEventParameter eventParam) {
            EventColorClicked?.Invoke(component as UIButton, buttons);
            colorButton.RefreshTooltip();
        }

        private void OnRoadDecorationButtonClicked(UIComponent component, UIMouseEventParameter eventParam) {
            EventRoadDecorationClicked?.Invoke(component as UIButton, buttons);
            roadDecorationButton.RefreshTooltip();
        }

        private void OnSettingsButtonClicked(UIComponent component, UIMouseEventParameter eventParam) {
            EventExtrasClicked?.Invoke(component as UIButton, buttons);
            settingsButton.RefreshTooltip();
        }

        private void OnPipetteButtonClicked(UIComponent component, UIMouseEventParameter eventParam) {
            EventPipetteClicked?.Invoke(component as UIButton, buttons);
            UIView.Find("DefaultTooltip")?.Hide();
        }
        private void OnResetButtonClicked(UIComponent component, UIMouseEventParameter eventParam) {
            EventResetClicked?.Invoke(component as UIButton, buttons);
            resetButton.RefreshTooltip();
        }

        private void OnCustomButtonClicked(UIComponent component, UIMouseEventParameter eventParam) {
            EventCustomClicked?.Invoke(component as UIButton, component.objectUserData as NSImplementationWrapper, buttons);
            component.RefreshTooltip();
        }
    }
}
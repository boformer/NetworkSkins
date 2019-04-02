using ColossalFramework.UI;
using NetworkSkins.Locale;
using NetworkSkins.TranslationFramework;
using UnityEngine;

namespace NetworkSkins.GUI
{
    public class ButtonBar : PanelBase
    {
        public delegate void TreesButtonClickedEventHandler();
        public event TreesButtonClickedEventHandler EventTreesClicked;

        public delegate void LightsButtonClickedEventHandler();
        public event LightsButtonClickedEventHandler EventLightsClicked;

        public delegate void SurfacesButtonClickedEventHandler();
        public event SurfacesButtonClickedEventHandler EventSurfacesClicked;

        public delegate void PillarsButtonClickedEventHandler();
        public event PillarsButtonClickedEventHandler EventPillarsClicked;

        public delegate void CatenaryButtonClickedEventHandler();
        public event CatenaryButtonClickedEventHandler EventCatenaryClicked;

        public delegate void ColorButtonClickedEventHandler();
        public event ColorButtonClickedEventHandler EventColorClicked;

        public delegate void ExtrasButtonClickedEventHandler();
        public event ExtrasButtonClickedEventHandler EventExtrasClicked;

        public delegate void TreesButtonVisibilityChangedEventHandler(bool visible);
        public event TreesButtonVisibilityChangedEventHandler EventTreesVisibilityChanged;

        public delegate void LightsButtonVisibilityChangedEventHandler(bool visible);
        public event LightsButtonVisibilityChangedEventHandler EventLightsVisibilityChanged;

        public delegate void SurfacesButtonVisibilityChangedEventHandler(bool visible);
        public event SurfacesButtonVisibilityChangedEventHandler EventSurfacesVisibilityChanged;

        public delegate void PillarsButtonVisibilityChangedEventHandler(bool visible);
        public event PillarsButtonVisibilityChangedEventHandler EventPillarsVisibilityChanged;

        public delegate void CatenaryButtonVisibilityChangedEventHandler(bool visible);
        public event CatenaryButtonVisibilityChangedEventHandler EventCatenaryVisibilityChanged;

        public delegate void ColorButtonVisibilityChangedEventHandler(bool visible);
        public event ColorButtonVisibilityChangedEventHandler EventColorVisibilityChanged;

        public delegate void ExtrasButtonVisibilityChangedEventHandler(bool visible);
        public event ExtrasButtonVisibilityChangedEventHandler EventExtrasVisibilityChanged;

        private NetToolMonitor Monitor => NetToolMonitor.Instance;
        private UIButton treesButton;
        private UIButton lightsButton;
        private UIButton surfacesButton;
        private UIButton pillarsButton;
        private UIButton catenaryButton;
        private UIButton colorButton;
        private UIButton extrasButton;

        public override void Awake() {
            base.Awake();
            Monitor.EventPrefabChanged += OnPrefabChanged;
        }

        public override void OnDestroy() {
            base.OnDestroy();
            Monitor.EventPrefabChanged -= OnPrefabChanged;

            treesButton.eventClicked -= OnTreesButtonClicked;
            lightsButton.eventClicked -= OnLightsButtonClicked;
            surfacesButton.eventClicked -= OnSurfacesButtonClicked;
            pillarsButton.eventClicked -= OnPillarsButtonClicked;
            catenaryButton.eventClicked -= OnCatenaryButtonClicked;
            colorButton.eventClicked -= OnColorButtonClicked;
            extrasButton.eventClicked -= OnExtrasButtonClicked;
        }

        public override void Build(Layout layout) {
            base.Build(layout);
            CreateButtons();
            UIPanel space = AddUIComponent<UIPanel>();
            space.size = new Vector2(width, 0.1f);
            RefreshUI(null);
        }

        protected override void RefreshUI(NetInfo netInfo) {
            treesButton.isVisible = Monitor.NetInfoHasTrees;
            lightsButton.isVisible = Monitor.NetInfoHasStreetLights;
            surfacesButton.isVisible = Monitor.NetInfoHasSurfaces;
            pillarsButton.isVisible = Monitor.NetInfoHasPillars;
            catenaryButton.isVisible = Monitor.NetInfoHasCatenaries;
            colorButton.isVisible = Monitor.NetInfoIsColorable;
        }

        private void OnPrefabChanged(NetInfo netInfo) {
            RefreshUI(netInfo);
        }

        private void CreateButtons() {
            Vector2 buttonSize = new Vector2(Layout.Size.x - Layout.Spacing * 2, size.x - Layout.Spacing * 2);

            treesButton = CreateButton(buttonSize, "T", Translation.Instance.GetTranslation(TranslationID.TOOLTIP_TREES));
            treesButton.eventClicked += OnTreesButtonClicked;
            treesButton.eventVisibilityChanged += OnTreesButtonVisibilityChanged;

            lightsButton = CreateButton(buttonSize, "L", Translation.Instance.GetTranslation(TranslationID.TOOLTIP_LIGHTS));
            lightsButton.eventClicked += OnLightsButtonClicked;
            lightsButton.eventVisibilityChanged += OnLightsButtonVisibilityChanged;

            surfacesButton = CreateButton(buttonSize, "S", Translation.Instance.GetTranslation(TranslationID.TOOLTIP_SIDEWALKS));
            surfacesButton.eventClicked += OnSurfacesButtonClicked;
            surfacesButton.eventVisibilityChanged += OnSurfacesButtonVisibilityChanged;

            pillarsButton = CreateButton(buttonSize, "P", Translation.Instance.GetTranslation(TranslationID.TOOLTIP_PILLARS));
            pillarsButton.eventClicked += OnPillarsButtonClicked;
            pillarsButton.eventVisibilityChanged += OnPillarsButtonVisibilityChanged;

            catenaryButton = CreateButton(buttonSize, "C", Translation.Instance.GetTranslation(TranslationID.TOOLTIP_CATENARY));
            catenaryButton.eventClicked += OnCatenaryButtonClicked;
            catenaryButton.eventVisibilityChanged += OnCatenaryButtonVisibilityChanged;

            colorButton = CreateButton(buttonSize, "C", Translation.Instance.GetTranslation(TranslationID.TOOLTIP_COLOR));
            colorButton.eventClicked += OnColorButtonClicked;
            colorButton.eventVisibilityChanged += OnColorButtonVisibilityChanged;

            extrasButton = CreateButton(buttonSize, "E", Translation.Instance.GetTranslation(TranslationID.TOOLTIP_EXTRAS));
            extrasButton.eventClicked += OnExtrasButtonClicked;
            extrasButton.eventVisibilityChanged += OnExtraButtonVisibilityChanged;
        }

        private void OnTreesButtonVisibilityChanged(UIComponent component, bool value) {
            EventTreesVisibilityChanged?.Invoke(value);
        }

        private void OnLightsButtonVisibilityChanged(UIComponent component, bool value) {
            EventLightsVisibilityChanged?.Invoke(value);
        }

        private void OnSurfacesButtonVisibilityChanged(UIComponent component, bool value) {
            EventSurfacesVisibilityChanged?.Invoke(value);
        }

        private void OnPillarsButtonVisibilityChanged(UIComponent component, bool value) {
            EventPillarsVisibilityChanged?.Invoke(value);
        }

        private void OnCatenaryButtonVisibilityChanged(UIComponent component, bool value) {
            EventCatenaryVisibilityChanged?.Invoke(value);
        }

        private void OnColorButtonVisibilityChanged(UIComponent component, bool value) {
            EventColorVisibilityChanged?.Invoke(value);
        }

        private void OnExtraButtonVisibilityChanged(UIComponent component, bool value) {
            EventExtrasVisibilityChanged?.Invoke(value);
        }

        private void OnTreesButtonClicked(UIComponent component, UIMouseEventParameter eventParam) {
            EventTreesClicked?.Invoke();
        }

        private void OnLightsButtonClicked(UIComponent component, UIMouseEventParameter eventParam) {
            EventLightsClicked?.Invoke();
        }

        private void OnSurfacesButtonClicked(UIComponent component, UIMouseEventParameter eventParam) {
            EventSurfacesClicked?.Invoke();
        }

        private void OnPillarsButtonClicked(UIComponent component, UIMouseEventParameter eventParam) {
            EventPillarsClicked?.Invoke();
        }

        private void OnCatenaryButtonClicked(UIComponent component, UIMouseEventParameter eventParam) {
            EventCatenaryClicked?.Invoke();
        }

        private void OnColorButtonClicked(UIComponent component, UIMouseEventParameter eventParam) {
            EventColorClicked?.Invoke();
        }

        private void OnExtrasButtonClicked(UIComponent component, UIMouseEventParameter eventParam) {
            EventExtrasClicked?.Invoke();
        }
    }
}
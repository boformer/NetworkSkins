using System.Collections.Generic;
using NetworkSkins.GUI.Abstraction;
using NetworkSkins.Net;
using NetworkSkins.Persistence;
using NetworkSkins.Skins;
using NetworkSkins.Skins.Modifiers;
using UnityEngine;

namespace NetworkSkins.GUI.Colors
{
    public class ColorPanelController : FeaturePanelController
    {
        public const int MaxSwatchesCount = 10;

        public override bool Enabled => base.Enabled && _colorable;

        public Color SelectedColor { get; private set; }

        public IEnumerable<Color32> Swatches => _swatches;
        private readonly List<Color32> _swatches;

        public event ColorUsedInSegmentEventHandler EventColorUsedInSegment;
        public delegate void ColorUsedInSegmentEventHandler();

        private bool _colorable = false;

        private bool _default = true;
        
        public ColorPanelController()
        {
            _swatches = PersistenceService.Instance.GetSwatches();
        }

        public void SetColor(Color selected)
        {
            if (SelectedColor == selected) return;

            SelectedColor = selected;
            _default = (selected == Prefab.m_color);

            SaveSelectedColor();

            OnChanged();
        }
        
        public override void Reset()
        {
            if (!Enabled || _default) return;

            SelectedColor = Prefab.m_color;
            _default = true;

            SaveSelectedColor();

            OnChanged();
        }

        public void OnSegmentPlaced(NetworkSkin skin)
        {
            foreach (var modifier in skin.Modifiers)
            {
                if (modifier is ColorModifier colorModifier)
                {
                        (colorModifier.Color);
                    break;
                }
            }
        }

        private void OnColorUsed(Color color)
        {
            var colorIndex = _swatches.IndexOf(color);
            if (colorIndex == 0)
            {
                // color is the last used color
                return;
            }

            if (colorIndex != -1)
            {
                _swatches.RemoveAt(colorIndex);
                _swatches.Insert(0, color);
            }
            else
            {
                _swatches.Insert(0, color);
                if (_swatches.Count > MaxSwatchesCount)
                {
                    _swatches.RemoveRange(MaxSwatchesCount, _swatches.Count - MaxSwatchesCount);
                }
            }

            EventColorUsedInSegment?.Invoke();

            PersistenceService.Instance.UpdateSwatches(_swatches);
        }

        protected override void Build()
        {
            RefreshColorable();

            var savedColor = LoadSelectedColor();
            SelectedColor = savedColor ?? Prefab.m_color;
            _default = !savedColor.HasValue;
        }

        protected override void BuildWithModifiers(List<NetworkSkinModifier> modifiers)
        {
            RefreshColorable();

            SelectedColor = Prefab.m_color;
            _default = true;

            foreach (var modifier in modifiers)
            {
                if (modifier is ColorModifier colorModifier)
                {
                    SelectedColor = colorModifier.Color;
                    _default = false;
                    break;
                }   
            }

            SaveSelectedColor();
        }

        private void RefreshColorable()
        {
            var subPrefabs = NetUtils.GetPrefabVariations(Prefab);

            _colorable = false;
            foreach (var subPrefab in subPrefabs)
            {
                _colorable = _colorable || NetTextureUtils.HasRoadTexture(subPrefab);
            }
        }

        protected override Dictionary<NetInfo, List<NetworkSkinModifier>> BuildModifiers()
        {
            var modifiers = new Dictionary<NetInfo, List<NetworkSkinModifier>>();

            if (!_default)
            {
                var prefabModifiers = new List<NetworkSkinModifier>
                {
                    new ColorModifier(SelectedColor)
                };

                var subPrefabs = NetUtils.GetPrefabVariations(Prefab);
                foreach (var subPrefab in subPrefabs)
                {
                    if (NetTextureUtils.HasRoadTexture(subPrefab))
                    {
                        modifiers[subPrefab] = prefabModifiers;
                    }
                }
            }

            return modifiers;
        }

        #region Active Selection Data
        private const string ColorKey = "Color";

        private Color? LoadSelectedColor()
        {
            return ActiveSelectionData.Instance.GetColorValue(Prefab, ColorKey);
        }

        private void SaveSelectedColor()
        {
            if (!_default)
            {
                ActiveSelectionData.Instance.SetColorValue(Prefab, ColorKey, SelectedColor);
            }
            else
            {
                ActiveSelectionData.Instance.ClearValue(Prefab, ColorKey);
            }
        }
        #endregion
    }
}

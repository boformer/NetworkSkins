using System.Collections.Generic;
using NetworkSkins.Net;
using NetworkSkins.Skins;
using NetworkSkins.Skins.Modifiers;
using UnityEngine;

namespace NetworkSkins.Controller
{
    public class ColorFeatureController : FeatureController
    {
        public override bool Enabled => base.Enabled && _colorable;

        public Color SelectedColor { get; private set; }

        private bool _colorable = false;

        private bool _default = true;

        public void OnSelectedColorChanged(Color selected)
        {
            if (SelectedColor == selected) return;

            SelectedColor = selected;
            _default = false;

            SaveSelectedColor();

            OnChanged();
        }

        public void OnColorReset()
        {
            if (_default) return;

            SelectedColor = Prefab.m_color;
            _default = true;

            SaveSelectedColor();

            OnChanged();
        }

        protected override void Build()
        {
            var subPrefabs = NetUtils.GetPrefabVariations(Prefab);

            _colorable = false;
            foreach (var subPrefab in subPrefabs)
            {
                _colorable = _colorable || NetTextureUtils.HasRoadTexture(subPrefab);
            }

            var savedColor = LoadSelectedColor();
            SelectedColor = savedColor ?? Prefab.m_color;
            _default = !savedColor.HasValue;
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

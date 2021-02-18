using System.Collections.Generic;
using System.Linq;
using NetworkSkins.GUI.Abstraction;
using NetworkSkins.Net;
using NetworkSkins.Persistence;
using NetworkSkins.Skins;
using NetworkSkins.Skins.Modifiers;
using UnityEngine;

namespace NetworkSkins.GUI.Lights
{
    public class StreetLightPanelController : ListPanelController<PropInfo>
    {
        public float DefaultRepeatDistance { get; private set; }

        public float SelectedRepeatDistance { get; private set; }

        public bool CanEditRepeatDistance { get; private set; }

        public void SetRepeatDistance(float repeatDistance)
        {
            SelectedRepeatDistance = repeatDistance;

            SaveSelectedRepeatDistance();

            OnChanged();
        }

        public override void Reset()
        {
            base.Reset();

            if (Enabled)
            {
                SetRepeatDistance(DefaultRepeatDistance);
            }
        }

        protected override void Build()
        {
            base.Build();

            // Use repeat distance of the ground version of the road
            DefaultRepeatDistance = StreetLightUtils.GetDefaultRepeatDistance(Prefab);
            SelectedRepeatDistance = LoadSelectedRepeatDistance() ?? DefaultRepeatDistance;
        }

        protected override void BuildWithModifiers(List<NetworkSkinModifier> modifiers)
        {
            base.BuildWithModifiers(modifiers);

            DefaultRepeatDistance = StreetLightUtils.GetDefaultRepeatDistance(Prefab);
            SelectedRepeatDistance = GetSelectedRepeatDistanceFromModifiers(modifiers) ?? DefaultRepeatDistance;
            SaveSelectedRepeatDistance();
        }

        protected override Item GetSelectedItemFromModifiers(List<NetworkSkinModifier> modifiers)
        {
            foreach (var modifier in modifiers)
            {
                if (modifier is StreetLightModifier streetLightModifier)
                {
                    return FindItemByName(streetLightModifier.StreetLight?.name ?? "#NONE#");
                }
            }

            return null;
        }

        private float? GetSelectedRepeatDistanceFromModifiers(List<NetworkSkinModifier> modifiers)
        {
            foreach (var modifier in modifiers)
            {
                if (modifier is StreetLightModifier streetLightModifier)
                {
                    return streetLightModifier.RepeatDistance;
                }
            }

            return null;
        }

        protected override void OnChanged()
        {
            // fix for nature trail with repeat distance 0f!
            CanEditRepeatDistance = DefaultRepeatDistance >= 2.0f && (SelectedItem != DefaultItem || !(SelectedItem is DefaultVariantItem));
            base.OnChanged();
        }

        protected override List<Item> BuildItems(ref Item defaultItem)
        {
            var defaultStreetLights = GetDefaultStreetLights();
            if (defaultStreetLights.Count == 0)
            {
                return new List<Item>();
            }

            var items = new List<Item>
            {
                new SimpleItem("#NONE#", null)
            };

            PropInfo singleDefaultStreetLight = null;
            var uniqueDefaultStreetLights = new HashSet<PropInfo>(defaultStreetLights.Values);
            if(uniqueDefaultStreetLights.Count == 1)
            {
                singleDefaultStreetLight = uniqueDefaultStreetLights.First();
            }
            else
            {
                defaultItem = new DefaultVariantItem();
                items.Add(defaultItem);
            }

            var streetLights = StreetLightUtils.GetAvailableStreetLights();
            
            foreach (var streetLight in streetLights)
            {
                var item = new SimpleItem(streetLight.name, streetLight);
                items.Add(item);

                if (streetLight == singleDefaultStreetLight)
                {
                    defaultItem = item;
                }
            }

            //Debug.Log($"Built {items.Count} street light items with default {singleDefaultStreetLight}");

            return items;
        }

        private Dictionary<NetInfo, PropInfo> GetDefaultStreetLights()
        {
            var defaultStreetLights = new Dictionary<NetInfo, PropInfo>();

            var groundStreetLight = StreetLightUtils.GetDefaultStreetLight(Prefab);
            if (groundStreetLight != null) {
                if (StreetLightUtils.HasSingularStreetLight(Prefab)) {
                    defaultStreetLights[Prefab] = groundStreetLight;
                }
            }


            var elevatedPrefab = NetUtils.GetElevatedPrefab(Prefab);
            if (elevatedPrefab != null)
            {
                var elevatedStreetLight = StreetLightUtils.GetDefaultStreetLight(elevatedPrefab);
                if (elevatedStreetLight != null) {
                    if (StreetLightUtils.HasSingularStreetLight(elevatedPrefab)) {
                        defaultStreetLights[elevatedPrefab] = elevatedStreetLight;
                    }
                }
            }

            var bridgePrefab = NetUtils.GetBridgePrefab(Prefab);
            if (bridgePrefab != null)
            {
                var bridgeStreetLight = StreetLightUtils.GetDefaultStreetLight(bridgePrefab);
                if (bridgeStreetLight != null) {
                    if (StreetLightUtils.HasSingularStreetLight(bridgePrefab)) {
                        defaultStreetLights[bridgePrefab] = bridgeStreetLight;
                    }
                }
            }

            return defaultStreetLights;
        }

        protected override Dictionary<NetInfo, List<NetworkSkinModifier>> BuildModifiers()
        {
            var modifiers = new Dictionary<NetInfo, List<NetworkSkinModifier>>();
            
            if (SelectedItem != null && SelectedItem is SimpleItem item)
            {
                // fix for nature trail with repeat distance 0f!
                var repeatDistance = DefaultRepeatDistance >= 2.0f ? SelectedRepeatDistance : DefaultRepeatDistance;
                if (item != DefaultItem || repeatDistance != DefaultRepeatDistance)
                {
                    var prefabModifiers = new List<NetworkSkinModifier>
                    {
                        new StreetLightModifier(item.Value, repeatDistance)
                    };

                    if (StreetLightUtils.HasSingularStreetLight(Prefab)) {
                        modifiers[Prefab] = prefabModifiers;
                    }

                    var elevatedPrefab = NetUtils.GetElevatedPrefab(Prefab);
                    if (StreetLightUtils.HasSingularStreetLight(elevatedPrefab)) {
                        modifiers[elevatedPrefab] = prefabModifiers;
                    }

                    var bridgePrefab = NetUtils.GetBridgePrefab(Prefab);
                    if (StreetLightUtils.HasSingularStreetLight(bridgePrefab)) {
                        modifiers[bridgePrefab] = prefabModifiers;
                    }
                }
            }

            return modifiers;
        }

        #region Active Selection Data
        protected override string SelectedItemKey => $"StreetLight";

        private const string SelectedRepeatDistanceKey = "StreetLight_RepeatDistance";

        private float? LoadSelectedRepeatDistance()
        {
            return ActiveSelectionData.Instance.GetFloatValue(Prefab, SelectedRepeatDistanceKey);
        }

        private void SaveSelectedRepeatDistance()
        {
            if (SelectedRepeatDistance != DefaultRepeatDistance)
            {
                ActiveSelectionData.Instance.SetFloatValue(Prefab, SelectedRepeatDistanceKey, SelectedRepeatDistance);
            }
            else
            {
                ActiveSelectionData.Instance.ClearValue(Prefab, SelectedRepeatDistanceKey);
            }
        }
        #endregion
    }
}

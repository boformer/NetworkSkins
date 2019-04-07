using System;
using System.Collections.Generic;
using System.Linq;
using NetworkSkins.Net;
using NetworkSkins.Skins;
using NetworkSkins.Skins.Modifiers;
using UnityEngine;

namespace NetworkSkins.Controller
{
    public class StreetLightFeatureController : ItemListFeatureController<PropInfo>
    {
        public float DefaultRepeatDistance { get; private set; }

        public float SelectedRepeatDistance { get; private set; }

        public bool CanEditRepeatDistance { get; private set; }

        public void OnRepeatDistanceChanged(float repeatDistance)
        {
            SelectedRepeatDistance = repeatDistance;

            SaveSelectedRepeatDistance();

            OnChanged();
        }

        protected override void Build()
        {
            base.Build();

            // Use repeat distance of the ground version of the road
            DefaultRepeatDistance = StreetLightUtils.GetDefaultRepeatDistance(Prefab);
            SelectedRepeatDistance = LoadSelectedRepeatDistance() ?? DefaultRepeatDistance;
        }

        protected override void OnChanged()
        {
            CanEditRepeatDistance = SelectedItem != DefaultItem || !(SelectedItem is DefaultVariantItem);
            base.OnChanged();
        }

        protected override List<Item> BuildItems(out Item defaultItem)
        {
            defaultItem = null;

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

            var streetLights = GetAvailableStreetLights();
            
            foreach (var streetLight in streetLights)
            {
                var item = new SimpleItem(streetLight.name, streetLight);
                items.Add(item);

                if (streetLight == singleDefaultStreetLight)
                {
                    defaultItem = item;
                }
            }

            Debug.Log($"Built {items.Count} street light items with default {singleDefaultStreetLight}");

            return items;
        }

        private Dictionary<NetInfo, PropInfo> GetDefaultStreetLights()
        {
            var defaultStreetLights = new Dictionary<NetInfo, PropInfo>();

            var groundStreetLight = StreetLightUtils.GetDefaultStreetLight(Prefab);
            if (groundStreetLight != null) defaultStreetLights[Prefab] = groundStreetLight;

            var elevatedPrefab = NetUtil.GetElevatedPrefab(Prefab);
            if (elevatedPrefab != null)
            {
                var elevatedStreetLight = StreetLightUtils.GetDefaultStreetLight(elevatedPrefab);
                if (elevatedStreetLight != null) defaultStreetLights[elevatedPrefab] = elevatedStreetLight;
            }

            var bridgePrefab = NetUtil.GetBridgePrefab(Prefab);
            if (bridgePrefab != null)
            {
                var bridgeStreetLight = StreetLightUtils.GetDefaultStreetLight(bridgePrefab);
                if (bridgeStreetLight != null) defaultStreetLights[bridgePrefab] = bridgeStreetLight;
            }

            return defaultStreetLights;
        }

        public List<PropInfo> GetAvailableStreetLights()
        {
            var streetLights = new List<PropInfo>();

            var prefabCount = PrefabCollection<PropInfo>.LoadedCount();
            for (uint prefabIndex = 0; prefabIndex < prefabCount; prefabIndex++)
            {
                var prefab = PrefabCollection<PropInfo>.GetLoaded(prefabIndex);
                if (StreetLightUtils.IsStreetLightProp(prefab))
                {
                    streetLights.Add(prefab);
                }
            }

            streetLights.Sort((a, b) => string.Compare(a.GetLocalizedTitle(), b.GetLocalizedTitle(), StringComparison.Ordinal));

            return streetLights;
        }

        protected override Dictionary<NetInfo, List<NetworkSkinModifier>> BuildModifiers()
        {
            var modifiers = new Dictionary<NetInfo, List<NetworkSkinModifier>>();
            
            if (Prefab != null && SelectedItem != null && SelectedItem is SimpleItem item)
            {
                if (item != DefaultItem || SelectedRepeatDistance != DefaultRepeatDistance)
                {
                    var prefabModifiers = new List<NetworkSkinModifier>
                    {
                        new StreetLightModifier(item.Value, SelectedRepeatDistance)
                    };

                    modifiers.Add(Prefab, prefabModifiers);

                    var elevatedPrefab = NetUtil.GetElevatedPrefab(Prefab);
                    if (elevatedPrefab != null) modifiers.Add(elevatedPrefab, prefabModifiers);

                    var bridgePrefab = NetUtil.GetBridgePrefab(Prefab);
                    if (bridgePrefab != null) modifiers.Add(bridgePrefab, prefabModifiers);
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

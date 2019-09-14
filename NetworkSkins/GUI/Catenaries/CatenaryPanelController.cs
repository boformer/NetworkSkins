using System;
using System.Collections.Generic;
using System.Linq;
using NetworkSkins.GUI.Abstraction;
using NetworkSkins.Net;
using NetworkSkins.Skins;
using NetworkSkins.Skins.Modifiers;
using UnityEngine;

namespace NetworkSkins.GUI.Catenaries
{
    public class CatenaryPanelController : ListPanelController<PropInfo>
    {
        protected override List<Item> BuildItems(ref Item defaultItem)
        {
            if (!(Prefab.m_netAI is TrainTrackBaseAI))
            {
                return new List<Item>();
            }

            var defaultCatenaries = GetDefaultCatenaries();
            if (defaultCatenaries.Count == 0)
            {
                return new List<Item>();
            }

            var items = new List<Item>()
            {
                new SimpleItem("#NONE#", null)
            };

            PropInfo singleDefaultCatenary = null;
            var uniqueDefaultCatenaries = new HashSet<PropInfo>(defaultCatenaries.Values);
            if (uniqueDefaultCatenaries.Count == 1)
            {
                singleDefaultCatenary = uniqueDefaultCatenaries.First();
            }
            else
            {
                defaultItem = new DefaultVariantItem();
                items.Add(defaultItem);
            }

            var singleTrack = IsSingleTrack(uniqueDefaultCatenaries);

            var catenaries = GetAvailableCatenaries(singleTrack);

            foreach (var catenary in catenaries)
            {
                var item = new SimpleItem(catenary.name, catenary);
                items.Add(item);

                if (catenary == singleDefaultCatenary)
                {
                    defaultItem = item;
                }
            }

            Debug.Log($"Built {items.Count} catenary items with default {singleDefaultCatenary}");

            return items;
        }

        protected override Item GetSelectedItemFromModifiers(List<NetworkSkinModifier> modifiers)
        {
            foreach (var modifier in modifiers)
            {
                if (modifier is CatenaryModifier catenaryModifier)
                {
                    return FindItemByName(catenaryModifier.Catenary?.name ?? "#NONE#");
                }
            }

            return null;
        }

        private Dictionary<NetInfo, PropInfo> GetDefaultCatenaries()
        {
            var defaultCatenaries = new Dictionary<NetInfo, PropInfo>();

            var groundCatenary = CatenaryUtils.GetDefaultNormalCatenary(Prefab);
            if (groundCatenary != null) defaultCatenaries[Prefab] = groundCatenary;

            var slopePrefab = NetUtils.GetSlopePrefab(Prefab);
            if (slopePrefab != null)
            {
                var slopeCatenary = CatenaryUtils.GetDefaultNormalCatenary(slopePrefab);
                if (slopeCatenary != null) defaultCatenaries[slopePrefab] = slopeCatenary;
            }

            var elevatedPrefab = NetUtils.GetElevatedPrefab(Prefab);
            if (elevatedPrefab != null)
            {
                var elevatedCatenary = CatenaryUtils.GetDefaultNormalCatenary(elevatedPrefab);
                if (elevatedCatenary != null) defaultCatenaries[elevatedPrefab] = elevatedCatenary;
            }

            var bridgePrefab = NetUtils.GetBridgePrefab(Prefab);
            if (bridgePrefab != null)
            {
                var bridgeCatenary = CatenaryUtils.GetDefaultNormalCatenary(bridgePrefab);
                if (bridgeCatenary != null) defaultCatenaries[bridgePrefab] = bridgeCatenary;
            }

            return defaultCatenaries;
        }

        private static bool IsSingleTrack(ICollection<PropInfo> defaultCatenaries)
        {
            return defaultCatenaries.Any(CatenaryUtils.IsSingleRailNormalCatenaryProp);
        }

        private List<PropInfo> GetAvailableCatenaries(bool singleTrack)
        {
            var catenaries = new List<PropInfo>();

            var prefabCount = PrefabCollection<PropInfo>.LoadedCount();
            for (uint prefabIndex = 0; prefabIndex < prefabCount; prefabIndex++)
            {
                var prefab = PrefabCollection<PropInfo>.GetLoaded(prefabIndex);
                if (singleTrack)
                {
                    if (CatenaryUtils.IsSingleRailNormalCatenaryProp(prefab) && CatenaryUtils.IsCatenaryPropVisibeInUI(prefab))
                    {
                        catenaries.Add(prefab);
                    }
                }
                else
                {
                    if (CatenaryUtils.IsDoubleRailNormalCatenaryProp(prefab) && CatenaryUtils.IsCatenaryPropVisibeInUI(prefab))
                    {
                        catenaries.Add(prefab);
                    }
                }
            }

            catenaries.Sort((a, b) => string.Compare(a.GetUncheckedLocalizedTitle(), b.GetUncheckedLocalizedTitle(), StringComparison.Ordinal));

            return catenaries;
        }

        protected override Dictionary<NetInfo, List<NetworkSkinModifier>> BuildModifiers()
        {
            var modifiers = new Dictionary<NetInfo, List<NetworkSkinModifier>>();

            if (SelectedItem != null && SelectedItem is SimpleItem item && item != DefaultItem)
            {
                var prefabModifiers = new List<NetworkSkinModifier>
                {
                    new CatenaryModifier(item.Value)
                };

                modifiers[Prefab] = prefabModifiers;

                var slopePrefab = NetUtils.GetSlopePrefab(Prefab);
                if (slopePrefab != null) modifiers[slopePrefab] = prefabModifiers;

                var elevatedPrefab = NetUtils.GetElevatedPrefab(Prefab);
                if (elevatedPrefab != null) modifiers[elevatedPrefab] = prefabModifiers;

                var bridgePrefab = NetUtils.GetBridgePrefab(Prefab);
                if (bridgePrefab != null) modifiers[bridgePrefab] = prefabModifiers;
            }

            return modifiers;
        }

        protected override string SelectedItemKey => $"Catenary";
    }
}

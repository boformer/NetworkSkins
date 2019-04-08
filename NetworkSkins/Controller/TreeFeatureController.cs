using System;
using System.Collections.Generic;
using NetworkSkins.Net;
using NetworkSkins.Skins;
using NetworkSkins.Skins.Modifiers;
using UnityEngine;

namespace NetworkSkins.Controller
{
    public class TreeFeatureController : ItemListFeatureController<TreeInfo>
    {
        public readonly LanePosition Position;

        public float DefaultRepeatDistance { get; private set; }

        public float SelectedRepeatDistance { get; private set; }
        
        public TreeFeatureController(LanePosition position)
        {
            Position = position;
        }

        public void OnRepeatDistanceChanged(float repeatDistance)
        {
            SelectedRepeatDistance = repeatDistance;

            SaveSelectedRepeatDistance();

            OnChanged();
        }

        protected override void Build()
        {
            base.Build();

            if (Prefab != null)
            {
                DefaultRepeatDistance = TreeUtils.GetDefaultRepeatDistance(Prefab, Position);
                SelectedRepeatDistance = LoadSelectedRepeatDistance() ?? DefaultRepeatDistance;
            }
        }

        protected override List<Item> BuildItems(ref Item defaultItem)
        {
            if (Prefab == null)
            {
                return new List<Item>();
            }

            var defaultTree = TreeUtils.GetDefaultTree(Prefab, Position);
            if (defaultTree == null)
            {
                return new List<Item>();
            }

            var trees = TreeUtils.GetAvailableTrees();

            var items = new List<Item> { new SimpleItem("#NONE#", null) };
            foreach (var tree in trees)
            {
                var item = new SimpleItem(tree.name, tree);
                items.Add(item);

                if (tree == defaultTree)
                {
                    defaultItem = item;
                }
            }

            Debug.Log($"Built {items.Count} tree items with default {defaultTree} in lane {Position}");

            return items;
        }

        protected override Dictionary<NetInfo, List<NetworkSkinModifier>> BuildModifiers()
        {
            var modifiers = new Dictionary<NetInfo, List<NetworkSkinModifier>>();

            if (SelectedItem != null && SelectedItem is SimpleItem item)
            {
                if (item != DefaultItem || SelectedRepeatDistance != DefaultRepeatDistance)
                {
                    modifiers[Prefab] = new List<NetworkSkinModifier>()
                    {
                        new TreeModifier(Position, item.Value, SelectedRepeatDistance)
                    };
                }
            }

            return modifiers;
        }

        #region Active Selection Data
        protected override string SelectedItemKey => $"Tree_{Position}";

        private string SelectedRepeatDistanceKey => $"Tree_{Position}_RepeatDistance";

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

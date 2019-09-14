using System;
using ColossalFramework.UI;
using NetworkSkins.GUI.Abstraction;
using NetworkSkins.GUI.UIFastList;
using NetworkSkins.Locale;
using NetworkSkins.Net;
using NetworkSkins.TranslationFramework;
using UnityEngine;

namespace NetworkSkins.GUI.Surfaces
{
    public class TerrainSurfaceList : ListBase
    {
        protected override Vector2 ListSize => new Vector2(390.0f, 200.0f);
        protected override float RowHeight => 50.0f;

        protected override void RefreshUI(NetInfo netInfo) {
            fastList.width = 378.0f;
            SetupRowsData();
        }

        protected override void SetupRowsData() {
            int selectedIndex = -1;
            if (fastList.RowsData == null) {
                fastList.RowsData = new FastList<object>();
            }
            fastList.RowsData.Clear();
            fastList.RowsData.SetCapacity(NetworkSkinPanelController.TerrainSurface.Items.Count);
            fastList.height = RowHeight * NetworkSkinPanelController.TerrainSurface.Items.Count;
            for (int i = 0; i < NetworkSkinPanelController.TerrainSurface.Items.Count; i++) {
                ListPanelController<Surface>.SimpleItem item = NetworkSkinPanelController.TerrainSurface.Items[i] as ListPanelController<Surface>.SimpleItem;
                ListItem listItem = CreateListItem(item.Value);
                if (NetworkSkinPanelController.TerrainSurface.SelectedItem.Id == listItem.ID) selectedIndex = i;
                fastList.RowsData.Add(listItem);
            }
            fastList.DisplayAt(selectedIndex);
            fastList.SelectedIndex = selectedIndex;
        }

        protected ListItem CreateListItem(Surface surfaceType) {
            TerrainManager terrainManager = TerrainManager.instance;
            Texture2D thumbnail;
            string name;
            var isFavourite = IsFavourite(surfaceType.ToString());
            var isDefault = IsDefault(surfaceType.ToString());
            var prefix = isDefault
                ? string.Concat("(", Translation.Instance.GetTranslation(TranslationID.LABEL_DEFAULT), ") ")
                : string.Empty;
            switch (surfaceType) {
                case Surface.Pavement:
                    name = Translation.Instance.GetTranslation(TranslationID.LABEL_PAVEMENT);
                    thumbnail = terrainManager.m_properties.m_pavementDiffuse;
                    break;
                case Surface.Gravel:
                    name = Translation.Instance.GetTranslation(TranslationID.LABEL_GRAVEL);
                    thumbnail = terrainManager.m_properties.m_gravelDiffuse;
                    break;
                case Surface.Ruined:
                    name = Translation.Instance.GetTranslation(TranslationID.LABEL_RUINED);
                    thumbnail = terrainManager.m_properties.m_ruinedDiffuse;
                    break;
                default:
                    name = Translation.Instance.GetTranslation(TranslationID.LABEL_NONE);
                    thumbnail = Resources.NietIcon;
                    break;
            }
            var id = surfaceType == Surface.None ? "#NONE#" : Enum.GetName(typeof(Surface), surfaceType);
            var displayName = string.Concat(prefix, name);
            return new ListItem(id, displayName, thumbnail, isFavourite, false, isDefault, ItemType.Surfaces, default(Color));
        }
    }
}

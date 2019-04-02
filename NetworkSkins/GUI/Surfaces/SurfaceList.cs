using ColossalFramework.UI;
using NetworkSkins.Locale;
using NetworkSkins.Skins;
using NetworkSkins.TranslationFramework;
using System;
using UnityEngine;

namespace NetworkSkins.GUI.Surfaces
{
    public class SurfaceList : ListBase
    {

        protected override void OnFavouriteChanged(string itemID, bool favourite) {
            throw new NotImplementedException();
        }

        protected override void OnSelectedChanged(string itemID, bool selected) {
            throw new NotImplementedException();
        }

        protected override void SetupRowsData() {
            int selectedIndex = -1;
            fastList.RowsData = new FastList<object>();
            fastList.RowsData.SetCapacity((int)NetworkGroundType.Count);
            for (int surfaceIndex = 0; surfaceIndex < (int)NetworkGroundType.Count; surfaceIndex++) {
                ListItem listItem = CreateListItem((NetworkGroundType)surfaceIndex);
                if (listItem.IsSelected) selectedIndex = (int)surfaceIndex;
                fastList.RowsData.Add(listItem);
            }
            if (selectedIndex != -1) {
                fastList.SelectedIndex = selectedIndex;
            }
        }

        protected ListItem CreateListItem(NetworkGroundType surfaceType) {
            TerrainManager terrainManager = TerrainManager.instance;
            Texture2D thumbnail;
            string id, displayName, prefix, name = string.Empty;
            bool isFavourite, isDefault, isSelected;
            isSelected = IsSelected(surfaceType);
            isFavourite = IsFavourite(surfaceType);
            isDefault = IsDefault(surfaceType);
            prefix = isDefault
                ? Translation.Instance.GetTranslation(TranslationID.LABEL_DEFAULT)
                : string.Empty;
            switch (surfaceType) {
                case NetworkGroundType.Pavement:
                    name = Translation.Instance.GetTranslation(TranslationID.LABEL_PAVEMENT);
                    thumbnail = terrainManager.m_properties.m_pavementDiffuse;
                    break;
                case NetworkGroundType.Gravel:
                    name = Translation.Instance.GetTranslation(TranslationID.LABEL_GRAVEL);
                    thumbnail = terrainManager.m_properties.m_gravelDiffuse;
                    break;
                case NetworkGroundType.Ruined:
                    name = Translation.Instance.GetTranslation(TranslationID.LABEL_RUINED);
                    thumbnail = terrainManager.m_properties.m_ruinedDiffuse;
                    break;
                default:
                    name = Translation.Instance.GetTranslation(TranslationID.LABEL_NONE);
                    thumbnail = UIView.GetAView()?.defaultAtlas?.GetSpriteTexture("Niet");
                    break;
            }
            id = Enum.GetName(typeof(NetworkGroundType), surfaceType);
            displayName = string.Concat(prefix, name);
            return new ListItem(id, displayName, thumbnail, isSelected, isFavourite);
        }

        private bool IsDefault(NetworkGroundType surfaceType) {
            throw new NotImplementedException();
        }

        private bool IsFavourite(NetworkGroundType surfaceType) {
            throw new NotImplementedException();
        }

        private bool IsSelected(NetworkGroundType surfaceType) {
            throw new NotImplementedException();
        }
    }
}

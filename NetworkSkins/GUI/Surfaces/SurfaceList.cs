using NetworkSkins.Locale;
using NetworkSkins.Skins;
using NetworkSkins.TranslationFramework;
using System;
using UnityEngine;

namespace NetworkSkins.GUI.Surfaces
{
    public class SurfaceList : ListBase
    {
        protected ListItem CreateListItem(NetworkGroundType surfaceType) {
            Texture2D thumbnail;
            string id, displayName, prefix, name;
            bool isFavourite, isDefault, isSelected;
            isSelected = IsSelected(surfaceType);
            isFavourite = IsFavourite(surfaceType);
            isDefault = IsDefault(surfaceType);
            switch (surfaceType) {
                case NetworkGroundType.Unchanged:
                    break;
                case NetworkGroundType.Pavement:
                    break;
                case NetworkGroundType.Gravel:
                    break;
                case NetworkGroundType.Ruined:
                    break;
                case NetworkGroundType.None:
                    break;
                default:
                    break;
            }
            //prefix = isDefault
            //    ? Translation.Instance.GetTranslation(TranslationID.LABEL_DEFAULT)
            //    : string.Empty;
            //name = prefabInfo == null
            //    ? Translation.Instance.GetTranslation(TranslationID.LABEL_NONE)
            //    : prefabInfo.GetUncheckedLocalizedTitle();
            //id = prefabInfo == null ? "None" : prefabInfo.name;
            //displayName = string.Concat(prefix, name);
            //thumbnail = prefabInfo == null
            //    ? UIView.GetAView()?.defaultAtlas?.GetSpriteTexture("Niet")
            //    : prefabInfo.m_Atlas?.GetSpriteTexture(prefabInfo.m_Thumbnail);
            return null; //new ListItem(id, displayName, thumbnail, isSelected, isFavourite);
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

        protected override void OnFavouriteChanged(string itemID, bool favourite) {
            throw new NotImplementedException();
        }

        protected override void OnSelectedChanged(string itemID, bool selected) {
            throw new NotImplementedException();
        }

        protected override void SetupRowsData() {

        }
    }
}

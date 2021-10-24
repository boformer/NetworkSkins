using ColossalFramework.UI;
using System;
using System.IO;
using System.Reflection;
using UnityEngine;
using Object = UnityEngine.Object;

namespace NetworkSkins
{
    public class Resources
    {
        public static Resources Atlas { get; private set; } = new Resources();

        public static UITextureAtlas DefaultAtlas => NetworkSkinsMod.defaultAtlas;

        public const string ButtonSmall = "ButtonSmall";
        public const string ButtonSmallPressed = "ButtonSmallPressed";
        public const string ButtonSmallHovered = "ButtonSmallHovered";
        public const string ButtonSmallFocused = "ButtonSmallFocused";
        public static string Blacklisted = "Blacklisted";
        public static string DragHandle =       "DragHandle";
        public static string Star =             "Star";
        public static string StarOutline =      "StarOutline";
        public static string Locked =           "Locked";
        public static string Unlocked =         "Unlocked";
        public static string UnlockedPressed =  "UnlockedPressed";
        public static string UnlockedHovered =  "UnlockedHovered";
        public static string Tree =             "Tree";
        public static string TreePressed =      "TreePressed";
        public static string TreeHovered =      "TreeHovered";
        public static string TreeFocused =      "TreeFocused";
        public static string Light =            "Light";
        public static string LightPressed =     "LightPressed";
        public static string LightHovered =     "LightHovered";
        public static string LightFocused =     "LightFocused";
        public static string Pillar =           "Pillar";
        public static string PillarPressed =    "PillarPressed";
        public static string PillarHovered =    "PillarHovered";
        public static string PillarFocused =    "PillarFocused";
        public static string Catenary =         "Catenary";
        public static string CatenaryPressed =  "CatenaryPressed";
        public static string CatenaryHovered =  "CatenaryHovered";
        public static string CatenaryFocused =  "CatenaryFocused";
        public static string Color =            "Color";
        public static string ColorPressed =     "ColorPressed";
        public static string ColorHovered =     "ColorHovered";
        public static string ColorFocused =     "ColorFocused";
        public static string RoadDecoration =   "Crossing";
        public static string RoadDecorationPressed = "CrossingPressed";
        public static string RoadDecorationHovered = "CrossingHovered";
        public static string RoadDecorationFocused = "CrossingFocused";
        public static string Surface =          "Surface";
        public static string SurfacePressed =   "SurfacePressed";
        public static string SurfaceHovered =   "SurfaceHovered";
        public static string SurfaceFocused =   "SurfaceFocused";
        public static string Pipette = "Pipette";
        public static string PipettePressed = "PipettePressed";
        public static string PipetteHovered = "PipetteHovered";
        public static string PipetteFocused = "PipetteFocused";
        public static string PipetteCursor  = "PipetteCursor";
        public static string Settings =         "Settings";
        public static string SettingsPressed =  "SettingsPressed";
        public static string SettingsHovered =  "SettingsHovered";
        public static string SettingsFocused =  "SettingsFocused";
        public static string Swatch = "Swatch";
        public static string Undo = "Undo";
        public static string UndoPressed = "UndoPressed";
        public static string UndoHovered = "UndoHovered";

        private UITextureAtlas UITextureAtlas { get; set; }

        private readonly string[] _spriteNames = new string[] {
            "Blacklisted",
            "DragHandle",
            "DragHandle",
            "Star",
            "StarOutline",
            "Locked",
            "Unlocked",
            "UnlockedPressed",
            "UnlockedHovered",
            "Tree",
            "TreePressed",
            "TreeHovered",
            "TreeFocused",
            "Light",
            "LightPressed",
            "LightHovered",
            "LightFocused",
            "Pillar",
            "PillarPressed",
            "PillarHovered",
            "PillarFocused",
            "Catenary",
            "CatenaryPressed",
            "CatenaryHovered",
            "CatenaryFocused",
            "Color",
            "ColorPressed",
            "ColorHovered",
            "ColorFocused",
            "Crossing",
            "CrossingPressed",
            "CrossingHovered",
            "CrossingFocused",
            "Surface",
            "SurfacePressed",
            "SurfaceHovered",
            "SurfaceFocused",
            "Pipette",
            "PipettePressed",
            "PipetteHovered",
            "PipetteCursor",
            "Settings",
            "SettingsPressed",
            "SettingsHovered",
            "SettingsFocused",
            "Swatch",
            "Undo",
            "UndoPressed",
            "UndoHovered"
        };

        public Resources() {
            CreateAtlas();
        }

        public static implicit operator UITextureAtlas(Resources atlas) {
            return atlas.UITextureAtlas;
        }

        private void CreateAtlas() {
            UITextureAtlas textureAtlas = ScriptableObject.CreateInstance<UITextureAtlas>();

            Texture2D[] textures = new Texture2D[_spriteNames.Length];
            Texture2D texture2D = new Texture2D(1, 1, TextureFormat.ARGB32, false);

            for (int i = 0; i < _spriteNames.Length; i++)
                textures[i] = GetTextureFromAssemblyManifest(_spriteNames[i] + ".png");

            int maxSize = 1024;
            Rect[] regions = texture2D.PackTextures(textures, 2, maxSize);

            Material material = Object.Instantiate<Material>(UIView.GetAView().defaultAtlas.material);
            material.mainTexture = texture2D;
            textureAtlas.material = material;
            textureAtlas.name = "NetworkSkinsAtlas";

            for (int i = 0; i < _spriteNames.Length; i++) {
                UITextureAtlas.SpriteInfo item = new UITextureAtlas.SpriteInfo {
                    name = _spriteNames[i],
                    texture = textures[i],
                    region = regions[i],
                };

                textureAtlas.AddSprite(item);
            }

            UITextureAtlas = textureAtlas;
        }

        public static Texture2D GetTextureFromAssemblyManifest(string file) {
            try {
                string path = "NetworkSkins.Resources." + file;
                Texture2D texture2D = new Texture2D(1, 1, TextureFormat.ARGB32, false);
                using(Stream manifestResourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(path)) {
                    byte[] array = new byte[manifestResourceStream.Length];
                    manifestResourceStream.Read(array, 0, array.Length);
                    texture2D.LoadImage(array);
                }
                texture2D.wrapMode = TextureWrapMode.Clamp;
                texture2D.Apply();
                return texture2D;
            } catch(Exception ex) {
                Debug.LogException(new Exception($"GetTextureFromAssemblyManifest('{file}') failed!", ex));
                return null;
            }
        }
    }
}

using System.IO;
using System.Reflection;
using UnityEngine;
using ColossalFramework.UI;

namespace NetworkSkins
{
    public class Resources
    {
        public static Resources Atlas { get; private set; } = new Resources();

        private UITextureAtlas UITextureAtlas { get; set; }
        private string[] spriteNames = new string[] {
            "DragHandle",
        };

        public static string DragHandle = "DragHandle";

        public Resources() {
            Create();
        }

        private void Create() {
            UITextureAtlas textureAtlas = ScriptableObject.CreateInstance<UITextureAtlas>();

            Texture2D[] textures = new Texture2D[spriteNames.Length];
            Texture2D texture2D = new Texture2D(1, 1, TextureFormat.ARGB32, false);

            for (int i = 0; i < spriteNames.Length; i++)
                textures[i] = GetTextureFromAssemblyManifest(spriteNames[i] + ".png");

            int maxSize = 1024;
            Rect[] regions = new Rect[spriteNames.Length];
            regions = texture2D.PackTextures(textures, 2, maxSize);

            Material material = UnityEngine.Object.Instantiate<Material>(UIView.GetAView().defaultAtlas.material);
            material.mainTexture = texture2D;
            textureAtlas.material = material;
            textureAtlas.name = "NetworkSkinsAtlas";

            for (int i = 0; i < spriteNames.Length; i++) {
                UITextureAtlas.SpriteInfo item = new UITextureAtlas.SpriteInfo {
                    name = spriteNames[i],
                    texture = textures[i],
                    region = regions[i],
                };

                textureAtlas.AddSprite(item);
            }

            UITextureAtlas = textureAtlas;
        }

        private Texture2D GetTextureFromAssemblyManifest(string file) {
            string path = string.Concat(GetType().Namespace, ".Resources.", file);
            Texture2D texture2D = new Texture2D(1, 1, TextureFormat.ARGB32, false);
            using (Stream manifestResourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(path)) {
                byte[] array = new byte[manifestResourceStream.Length];
                manifestResourceStream.Read(array, 0, array.Length);
                texture2D.LoadImage(array);
            }
            texture2D.Apply();
            return texture2D;
        }

        public static implicit operator UITextureAtlas(Resources atlas) {
            return atlas.UITextureAtlas;
        }
    }
}

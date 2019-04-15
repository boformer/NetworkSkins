using ColossalFramework.UI;
using UnityEngine;

namespace NetworkSkins
{
    public static class TextureUtil
    {
        public static Texture2D MakeReadable(this Texture texture) {
            RenderTexture temporary = RenderTexture.GetTemporary(texture.width, texture.height, 0);
            Graphics.Blit(texture, temporary);
            Texture2D result = temporary.ToTexture2D();
            RenderTexture.ReleaseTemporary(temporary);
            return result;
        }

        public static Texture2D ToTexture2D(this RenderTexture rt) {
            RenderTexture active = RenderTexture.active;
            RenderTexture.active = rt;
            Texture2D texture2D = new Texture2D(rt.width, rt.height);
            texture2D.ReadPixels(new Rect(0f, 0f, rt.width, rt.height), 0, 0);
            texture2D.Apply();
            RenderTexture.active = active;
            return texture2D;
        }

        public static Texture2D GetSpriteTexture(this UITextureAtlas atlas, string spriteName) {
            return atlas?.sprites?.Find(sprite => sprite?.name == spriteName)?.texture;
        }
    }
}

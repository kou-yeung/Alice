using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
using System.IO;
#endif

namespace Zoo.Assets
{
    public class Sprites : ScriptableObject
    {
        [SerializeField]
        Sprite[] sprites;

        public Sprite this[string name]
        {
            get
            {
                foreach(var sprite in sprites)
                {
                    if (sprite.name == name) return sprite;
                }
                return null;
            }
        }
        public Sprite this[int index]
        {
            get
            {
                if (index < sprites.Length) return sprites[index];
                return null;
            }
        }
#if UNITY_EDITOR
        public void UpdateSpriteList(Sprite[] sprites)
        {
            this.sprites = sprites;
        }
#endif
    }

#if UNITY_EDITOR
    /// <summary>
    /// Multipleに設定されたSpriteを Sprites に変換する
    /// </summary>
    public class MultipleSpriteToSprite : AssetPostprocessor
    {
        void OnPostprocessSprites(Texture2D texture, Sprite[] sprites)
        {
            var asset = GetOrCreateSprites(assetPath);
            List<Sprite> s = new List<Sprite>();
            foreach(var v in AssetDatabase.LoadAllAssetsAtPath(assetPath).Where(v => v is Sprite))
            {
                s.Add(v as Sprite);
            }
            asset.UpdateSpriteList(s.ToArray());
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        Sprites GetOrCreateSprites(string path)
        {
            path = Path.ChangeExtension(path, ".asset");
            Sprites res = AssetDatabase.LoadAssetAtPath<Sprites>(path);
            if(res == null)
            {
                AssetDatabase.Refresh();
                res = ScriptableObject.CreateInstance<Sprites>();
                AssetDatabase.CreateAsset(res, path);
            }
            return res;
        }
    }
#endif
}

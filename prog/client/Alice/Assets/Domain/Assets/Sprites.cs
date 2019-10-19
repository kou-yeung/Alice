using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Reflection;
using UniRx;
using System.Text.RegularExpressions;
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

        public int Count { get { return sprites.Length; } }

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
        Sprites GetOrCreateSprites(string path)
        {
            path = Path.ChangeExtension(path, ".asset");
            Sprites res = AssetDatabase.LoadAssetAtPath<Sprites>(path);
            if(res == null)
            {
                res = ScriptableObject.CreateInstance<Sprites>();
                AssetDatabase.CreateAsset(res, path);
                AssetDatabase.SaveAssets();
            }
            return res;
        }

        /// <summary>
        /// パスを指定してSpritesを生成する
        /// </summary>
        /// <param name="assetPath"></param>
        void GenSprites(string assetPath)
        {
            var asset = GetOrCreateSprites(assetPath);

            List<Sprite> s = new List<Sprite>();
            foreach (var v in AssetDatabase.LoadAllAssetsAtPath(assetPath).Where(v => v is Sprite))
            {
                s.Add(v as Sprite);
            }
            asset.UpdateSpriteList(s.ToArray());
            AssetDatabase.SaveAssets();
            EditorUtility.SetDirty(asset);
        }

        /// <summary>
        /// 
        /// </summary>
        void OnPreprocessTexture()
        {
            if (assetPath.Contains("AddressableAssets/Character"))
            {
                AutoSpriteSheet(3, 4);  // 3x4で分割
                Register(assetPath);    // Sprites登録
            }
            else if (assetPath.Contains("AddressableAssets/Effect"))
            {
                Register(assetPath);    // Sprites登録
            }
        }

        /// <summary>
        /// 指定したカウントで自動分割する
        /// </summary>
        /// <param name="countX"></param>
        /// <param name="countY"></param>
        void AutoSpriteSheet(int countX, int countY)
        {
            TextureImporter textureImporter = (TextureImporter)assetImporter;

            // Textureのサイズ取得
            // 参考:http://baba-s.hatenablog.com/entry/2017/06/21/100000
            var size = new object[2] { 0, 0 };
            var method = typeof(TextureImporter).GetMethod("GetWidthAndHeight", BindingFlags.NonPublic | BindingFlags.Instance);
            method.Invoke(textureImporter, size);
            var width = (int)size[0];
            var height = (int)size[1];

            textureImporter.spriteImportMode = SpriteImportMode.Multiple;
            var sliceWidth = width / countX;
            var sliceHeight = height / countY;

            List<SpriteMetaData> metas = new List<SpriteMetaData>();
            for (int y = 0; y < countY; y++)
            {
                for (int x = 0; x < countX; x++)
                {
                    var meta = new SpriteMetaData();
                    meta.pivot = new Vector2(.5f, .5f);
                    meta.name = $"{Path.GetFileNameWithoutExtension(assetPath)}_{(countX * countY - 1) - (y * countX + x)}";

                    meta.rect = new Rect(sliceWidth * x, sliceHeight * y, sliceWidth, sliceHeight);
                    metas.Add(meta);
                }
            }
            textureImporter.spritesheet = metas.ToArray();
        }

        /// <summary>
        /// 指定したパスのSpritesを生成登録する
        /// </summary>
        /// <param name="path"></param>
        void Register(string path)
        {
            // 次のフレームにSpriteを生成(更新)します
            Observable.NextFrame().Subscribe(
                _ => { },
                () => GenSprites(path));
        }

        [MenuItem("Character/Copy")]
        static void CopyCharacter()
        {
            var from = @"D:\Image\素材\ゆうひな素材\歩行ドットMV";
            var to = @"D:\GitHub\Alice\prog\client\Alice\Assets\AddressableAssets\Character";

            foreach(var path in Directory.GetFiles(from, "*.png"))
            {
                var fn = Path.GetFileName(path);
                if (!Regex.IsMatch(fn, @"\$yuhinamv(.*)")) continue;
                File.Copy(path, Path.Combine(to, fn.Replace("$yuhinamv", "")), true);
            }

            AssetDatabase.Refresh();
        }
    }
#endif
}

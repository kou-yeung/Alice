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
        Sprites GetOrCreateSprites(string spritePath)
        {
            Sprites res = AssetDatabase.LoadAssetAtPath<Sprites>(spritePath);
            if(res == null)
            {
                res = ScriptableObject.CreateInstance<Sprites>();
                AssetDatabase.CreateAsset(res, spritePath);
                AssetDatabase.SaveAssets();
            }
            return res;
        }

        /// <summary>
        /// パスを指定してSpritesを生成する
        /// </summary>
        /// <param name="assetPath"></param>
        void GenSprites(string assetPath, string spritePath)
        {
            if(!Directory.Exists(Path.GetDirectoryName(spritePath)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(spritePath));
            }

            var asset = GetOrCreateSprites(spritePath);
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
            if (assetPath.Contains("AddressableAssets/Character/Work"))
            {
                var fn = Path.GetFileNameWithoutExtension(assetPath);
                // 歩行
                {
                    var match = Regex.Match(fn, @"\$yuhina([0-9]+)y?");
                    if (match != Match.Empty)
                    {
                        var spritePath = assetPath.Replace("Work", match.Groups[1].ToString());
                        spritePath = spritePath.Replace(fn, "walk");
                        spritePath = Path.ChangeExtension(spritePath, ".asset");
                        AutoSpriteSheet(3, 4);  // 3x4で分割
                        Register(assetPath, spritePath);    // Sprites登録
                    }
                }
                // アイコン
                {
                    var match = Regex.Match(fn, @"yhvx([0-9]+)");
                    if (match != Match.Empty)
                    {
                        var spritePath = assetPath.Replace("Work", match.Groups[1].ToString());
                        spritePath = spritePath.Replace(fn, "icon");
                        spritePath = Path.ChangeExtension(spritePath, ".asset");
                        AutoSpriteSheet(4, 2);  // 3x4で分割
                        Register(assetPath, spritePath);    // Sprites登録
                    }
                }
            }
            else if (assetPath.Contains("AddressableAssets/Effect"))
            {
                var spritePath = Path.ChangeExtension(assetPath, ".asset");
                Register(assetPath, spritePath);    // Sprites登録
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
                    meta.name = $"{Path.GetFileNameWithoutExtension(assetPath)}_{y}_{x}";

                    meta.rect = new Rect(sliceWidth * x, sliceHeight * (countY-y-1), sliceWidth, sliceHeight);
                    metas.Add(meta);
                }
            }
            textureImporter.spritesheet = metas.ToArray();
        }

        /// <summary>
        /// 指定したパスのSpritesを生成登録する
        /// </summary>
        /// <param name="path"></param>
        void Register(string assetPath, string spritePath)
        {
            // 次のフレームにSpriteを生成(更新)します
            Observable.NextFrame().Subscribe(
                _ => { },
                () => GenSprites(assetPath, spritePath));
        }

        [MenuItem("Character/Copy/Walk")]
        static void CopyCharacter_Walk()
        {
            var from = @"D:\Image\素材\ゆうひな素材\歩行ドットAce";
            var to = @"D:\GitHub\Alice\prog\client\Alice\Assets\AddressableAssets\Character\Work";

            foreach (var path in Directory.GetFiles(from, "*.png"))
            {
                var fn = Path.GetFileName(path);
                if (!Regex.IsMatch(fn, @"\$yuhina([0-9]+)y?.png")) continue;
                File.Copy(path, Path.Combine(to, fn), true);
            }
            AssetDatabase.Refresh();
        }
        [MenuItem("Character/Copy/Icon")]
        static void CopyCharacter_Icon()
        {
            var from = @"D:\Image\素材\ゆうひな素材\顔画像Ace";
            var to = @"D:\GitHub\Alice\prog\client\Alice\Assets\AddressableAssets\Character\Work";

            foreach (var path in Directory.GetFiles(from, "*.png"))
            {
                var fn = Path.GetFileName(path);
                if (!Regex.IsMatch(fn, @"yhvx([0-9]+).png")) continue;
                File.Copy(path, Path.Combine(to, fn), true);
            }
            AssetDatabase.Refresh();
        }
    }
#endif
}

using UnityEngine;
using System;
using System.Linq;
using Zoo.IO;
using System.IO;
using CsvHelper;
using Zoo;
using System.Text.RegularExpressions;
using UnityEngine.Assertions;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Alice.Entities
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class MasterData : ScriptableObject
    {
        public static MasterData Instance { get; private set; }
        static readonly string Path = @"Entities/MasterData.asset";

        public Character[] characters;
        public Skill[] skills;
        public Effect[] effects;
        public Personality[] personalities;
        public Product[] products;
        public TextData[] texts;

        /// <summary>
        /// 初期化
        /// </summary>
        /// <param name="cb"></param>
        public static void Initialize(Action cb)
        {
            ScreenBlocker.Instance?.Push();
            LoaderService.Instance.Preload(new[] { Path }, () =>
            {
                ScreenBlocker.Instance?.Pop();
                Instance = LoaderService.Instance.Load<MasterData>(Path);
                cb?.Invoke();
            });
        }
        // ヘルパー関数
        /// <summary>
        /// 検索: UserSkill -> Skill
        /// </summary>
        /// <param name="skill"></param>
        /// <returns></returns>
        public Skill Find(UserSkill skill)
        {
            return FindSkillByID(skill.id);
        }
        public Skill FindSkillByID(string id)
        {
            return skills.FirstOrDefault(v => v.ID == id);
        }

        /// <summary>
        /// 検索: UserUnit -> Character
        /// </summary>
        /// <param name="unit"></param>
        /// <returns></returns>
        public Character Find(UserUnit unit)
        {
            return characters.First(v => v.ID == unit.characterId);
        }

        /// <summary>
        /// IDからエフェクト情報を取得する
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Effect FindEffectByID(string id)
        {
            return effects.FirstOrDefault(v => v.ID == id);
        }

        /// <summary>
        /// IDから商品の情報を取得する
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Product FindProductByID(string id)
        {
            return products.FirstOrDefault(v => v.ID == id);
        }

        /// <summary>
        /// IDからテキストを取得する
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string FindTextDataByID(string id)
        {
            var res = texts.FirstOrDefault(v => v.ID == id);
            if (res == null)
            {
                Debug.LogWarning($"FindTextDataByID Error:{id}");
                return id;
            } else {
                return res.Text[(int)TextData.CurrentLanguage];
            }
        }


#if UNITY_EDITOR
        public class MasterDataPostprocessor : AssetPostprocessor
        {
            static MasterData GetOrCreate()
            {
                var path = @"Assets/AddressableAssets/Entities/MasterData.asset";
                var res = AssetDatabase.LoadAssetAtPath<MasterData>(path);
                if (res == null)
                {
                    res = ScriptableObject.CreateInstance<MasterData>();
                    AssetDatabase.CreateAsset(res, path);
                    AssetDatabase.SaveAssets();
                }
                return res;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="importedAssets"></param>
            /// <param name="deletedAssets"></param>
            /// <param name="movedAssets"></param>
            /// <param name="movedFromAssetPaths"></param>
            static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
            {
                var pattern = @"AddressableAssets/Entities/(.+).csv";
                var paths = importedAssets.Where(v => Regex.IsMatch(v, pattern));
                if (paths.Count() <= 0) return;

                var asset = GetOrCreate();
                foreach (var path in paths)
                {
                    var match = Regex.Match(path, pattern);
                    switch (match.Groups[1].ToString())
                    {
                        case "Skill":
                            asset.skills = Load<Skill>(path);
                            break;
                        case "Character":
                            asset.characters = Load<Character>(path);
                            break;
                        case "Personality":
                            asset.personalities = Load<Personality>(path);
                            break;
                        case "Effect":
                            asset.effects = Load<Effect>(path);
                            break;
                        case "Product":
                            asset.products = Load<Product>(path);
                            break;
                        case "TextData":
                            asset.texts = Load<TextData>(path);
                            break;
                    }
                }
                AssetDatabase.SaveAssets();
                EditorUtility.SetDirty(asset);
            }

            /// <summary>
            /// パースする
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="path"></param>
            /// <returns></returns>
            static T[] Load<T>(string path)
            {
                using (var csv = new CsvReader(new StringReader(File.ReadAllText(path)), CsvHelperRegister.configuration))
                {
                    return csv.GetRecords<T>().ToArray();
                }
            }
        }
#endif
    }

    ///// <summary>
    ///// マスタデータを管理するクラス
    ///// </summary>
    //public class _MasterData
    //{
    //    public static Character[] characters { get; private set; }
    //    public static Skill[] skills { get; private set; }
    //    public static Effect[] effects { get; private set; }
    //    public static Personality[] personalities { get; private set; }

    //    public static void Initialize(Action cb)
    //    {
    //        Async.Parallel( cb,
    //            (end) => Load<Character>("Entities/Character.csv", res => characters = res, end),
    //            (end) => Load<Skill>("Entities/Skill.csv", res => skills = res, end),
    //            (end) => Load<Effect>("Entities/Effect.csv", res => effects = res, end),
    //            (end) => Load<Personality>("Entities/Personality.csv", res => personalities = res, end)
    //        );
    //    }
    //    static void Load<T>(string path, Action<T[]> cb, Action end)
    //    {
    //        LoaderService.Instance.Preload(new[] { path }, ()=>
    //        {
    //            var asset = LoaderService.Instance.Load<TextAsset>(path);
    //            using (var csv = new CsvReader(new StringReader(asset.text), CsvHelperRegister.configuration))
    //            {
    //                cb?.Invoke(csv.GetRecords<T>().ToArray());
    //            }
    //            end?.Invoke();
    //        });
    //    }

    //    // ヘルパー関数
    //    /// <summary>
    //    /// 検索: UserSkill -> Skill
    //    /// </summary>
    //    /// <param name="skill"></param>
    //    /// <returns></returns>
    //    public static Skill Find(UserSkill skill)
    //    {
    //        return FindSkillByID(skill.id);
    //    }
    //    public static Skill FindSkillByID(string id)
    //    {
    //        return skills.FirstOrDefault(v => v.ID == id);
    //    }

    //    /// <summary>
    //    /// 検索: UserUnit -> Character
    //    /// </summary>
    //    /// <param name="unit"></param>
    //    /// <returns></returns>
    //    public static Character Find(UserUnit unit)
    //    {
    //        return characters.First(v => v.ID == unit.characterId);
    //    }

    //    /// <summary>
    //    /// IDからエフェクト情報を取得する
    //    /// </summary>
    //    /// <param name="id"></param>
    //    /// <returns></returns>
    //    public static Effect FindEffectByID(string id)
    //    {
    //        return effects.FirstOrDefault(v => v.ID == id);
    //    }
    //}
}

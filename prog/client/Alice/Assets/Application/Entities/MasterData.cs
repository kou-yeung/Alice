using UnityEngine;
using System;
using System.Linq;
using Zoo.IO;
using System.IO;
using CsvHelper;
using Zoo;

namespace Alice.Entities
{
    /// <summary>
    /// マスタデータを管理するクラス
    /// </summary>
    public class MasterData
    {
        public static Character[] characters { get; private set; }
        public static Skill[] skills { get; private set; }
        public static Effect[] effects { get; private set; }
        public static Personality[] personalities { get; private set; }

        public static void Initialize(Action cb)
        {
            Async.Parallel( cb,
                (end) => Load<Character>("Entities/Character.csv", res => characters = res, end),
                (end) => Load<Skill>("Entities/Skill.csv", res => skills = res, end),
                (end) => Load<Effect>("Entities/Effect.csv", res => effects = res, end),
                (end) => Load<Personality>("Entities/Personality.csv", res => personalities = res, end)
            );
        }
        static void Load<T>(string path, Action<T[]> cb, Action end)
        {
            LoaderService.Instance.Preload(new[] { path }, ()=>
            {
                var asset = LoaderService.Instance.Load<TextAsset>(path);
                using (var csv = new CsvReader(new StringReader(asset.text), CsvHelperRegister.configuration))
                {
                    cb?.Invoke(csv.GetRecords<T>().ToArray());
                }
                end?.Invoke();
            });
        }

        // ヘルパー関数
        /// <summary>
        /// 検索: UserSkill -> Skill
        /// </summary>
        /// <param name="skill"></param>
        /// <returns></returns>
        public static Skill Find(UserSkill skill)
        {
            return FindSkillByID(skill.id);
        }
        public static Skill FindSkillByID(string id)
        {
            return skills.FirstOrDefault(v => v.ID == id);
        }

        /// <summary>
        /// 検索: UserUnit -> Character
        /// </summary>
        /// <param name="unit"></param>
        /// <returns></returns>
        public static Character Find(UserUnit unit)
        {
            return characters.First(v => v.ID == unit.characterId);
        }

        /// <summary>
        /// IDからエフェクト情報を取得する
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Effect FindEffectByID(string id)
        {
            return effects.FirstOrDefault(v => v.ID == id);
        }
    }
}

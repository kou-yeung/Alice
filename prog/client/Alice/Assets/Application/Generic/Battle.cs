using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Alice.Entities;
using System.Linq;
using System;

namespace Alice.Generic
{
    public class BattleEnemy
    {
        public UserUnit[] unit { get; private set; }
        public UserDeck deck { get; private set; }
        /// <summary>
        /// 敵を生成
        /// </summary>
        /// <returns></returns>
        public static BattleEnemy Gen(Player player/*, UserUnit[] refUnit*/)
        {
            var res = new BattleEnemy();
            var random = new System.Random();

            // 合計バトル回数で敵の数を調整します
            // MEMO : 15回バトルするごとで敵の数を増やす(調整予定
            var count = Mathf.Min((player.totalBattleCount / 15) + 1, 4);

            var rare = player.rank / 3;
            // 相手ユニット候補、ついでにキャラ抽選する
            var characters = MasterData.Instance.characters.Where(v => v.Rare <= rare).OrderBy(v => Guid.NewGuid()).Take(count);
            var skills = MasterData.Instance.skills.Where(v=>v.Rare <= rare).ToArray();

            // 自分の所持ユニット内、最大レベルの４体の取得し平均をとる
            var maxLevel = UserData.cacheHomeRecv.units.Max(v => v.Level());
            var averageLevel = Mathf.RoundToInt((float)UserData.cacheHomeRecv.units.Select(v => v.Level()).Take(4).Average());
            var enemyLevel = Mathf.RoundToInt((maxLevel + averageLevel) / 2.0f);

            Debug.Log($"averageLevel:{averageLevel}, enemyLevel:{enemyLevel}");
            // 自分のスキル所持数
            var skillNum = UserData.cacheHomeRecv.skills.Count();

            List<UserUnit> enemyUnit = new List<UserUnit>();
            List<string> enemyDeck = new List<string>();

            foreach(var character in characters)
            {
                var unit = new UserUnit();
                unit.characterId = character.ID;

                HashSet<string> skill = new HashSet<string>();
                if(skillNum > 0)
                {
                    var sCount = random.Next(1, 3);
                    for (int j = 0; j < sCount; j++)
                    {
                        skill.Add(skills[random.Next(0, skills.Length)].ID);
                    }
                    skillNum -= sCount;
                }
                unit.skill = skill.ToArray();
                unit.exp = Mathf.FloorToInt((enemyLevel - 1) * (enemyLevel - 1));
                enemyUnit.Add(unit);
                enemyDeck.Add(unit.characterId);
            }
            res.unit = enemyUnit.ToArray();
            res.deck = new UserDeck { ids = enemyDeck.ToArray() };

            foreach (var v in res.unit)
            {
                v.skill = v.skill.Where(n => !string.IsNullOrEmpty(n)).Distinct().ToArray();
            }

            return res;
        }
    }
}


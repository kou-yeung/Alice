using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Alice.Entities;
using System.Linq;

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
        public static BattleEnemy Gen(Player player, UserUnit[] refUnit)
        {
            var res = new BattleEnemy();
            var random = new System.Random();

            var rare = player.rank / 5;
            // 相手ユニット
            var skills = MasterData.Instance.skills.Where(v=>v.Rare <= rare).ToArray();
            var characters = MasterData.Instance.characters.Where(v => v.Rare <= rare).ToArray();

            var skillNum = refUnit.Sum(v => v.skill.Count(s => !string.IsNullOrEmpty(s)));

            List<UserUnit> enemyUnit = new List<UserUnit>();
            List<string> enemyDeck = new List<string>();

            // 合計バトル回数で敵の数を調整します
            // MEMO : 15回バトルするごとで敵の数を増やす(調整予定
            var count = Mathf.Min((player.totalBattleCount / 15) + 1, 4); 
            for (int i = 0; i < count; i++)
            {
                enemyDeck.Add("");
                // キャラ抽選
                var character = characters[random.Next(0, characters.Length)];
                if (enemyDeck.Exists(id => id == character.ID)) continue;

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

                var level = Mathf.Max(1, refUnit[Mathf.Min(i, refUnit.Length - 1)].Level() + random.Next(-1,2));
                unit.exp = Mathf.FloorToInt((level - 1) * (level - 1));
                enemyUnit.Add(unit);
                enemyDeck[i] = unit.characterId;
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


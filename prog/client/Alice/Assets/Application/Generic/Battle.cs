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
        public UserDeck[] deck { get; private set; }
        /// <summary>
        /// 敵を生成
        /// </summary>
        /// <returns></returns>
        public static BattleEnemy Gen(UserUnit[] refUnit)
        {
            var res = new BattleEnemy();
            var random = new System.Random();

            // 相手ユニット
            var skills = MasterData.skills;
            var characters = MasterData.characters;

            List<UserUnit> enemyUnit = new List<UserUnit>();
            List<UserDeck> enemyDeck = new List<UserDeck>();

            var count = refUnit.Length;   // 同じ数の敵を用意する
            for (int i = 0; i < count; i++)
            {
                // キャラ抽選
                var character = characters[random.Next(0, characters.Length)];
                if (enemyDeck.Exists(v => v.characterId == character.ID)) continue;

                var unit = new UserUnit();
                unit.characterId = character.ID;
                unit.skill = new string[] { skills[random.Next(0, skills.Length)].ID };
                var level = Mathf.Max(1, refUnit[i].Level() + random.Next(-1,2));
                unit.exp = Mathf.FloorToInt((level - 1) * (level - 1));
                enemyUnit.Add(unit);

                var deck = new UserDeck();
                deck.characterId = unit.characterId;
                deck.position = i;
                enemyDeck.Add(deck);
            }
            res.unit = enemyUnit.ToArray();
            res.deck = enemyDeck.ToArray();

            foreach (var v in res.unit)
            {
                v.skill = v.skill.Where(n => !string.IsNullOrEmpty(n)).Distinct().ToArray();
            }

            return res;
        }
    }
}


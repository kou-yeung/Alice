using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zoo.Communication;

namespace Alice
{
    public class AliceServer : IDummyServer
    {
        System.Random random = new System.Random();
        public void Call(string proto, string data, Action<string> complete = null, Action<string> error = null)
        {
            switch (proto)
            {
                case "Home": complete?.Invoke(Home(data)); break;
                case "Battle": complete?.Invoke(Battle(data)); break;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        string Home(string data)
        {
            HomeRecv recv = new HomeRecv();
            var random = new System.Random();
            // 宝箱一覧
            List <UserChest> chests = new List<UserChest>();

            // 残り時間ある
            chests.Add(new UserChest
            {
                uniq = "remain_chest",
                start = (DateTime.Now - TimeSpan.FromMinutes(random.Next(10, 20))).Ticks,
                end = (DateTime.Now + TimeSpan.FromMinutes(10)).Ticks,
                rate = 1
            });
            // 完了
            chests.Add(new UserChest
            {
                uniq = "ready_chest",
                start = (DateTime.Now - TimeSpan.FromMinutes(random.Next(10, 20))).Ticks,
                end = (DateTime.Now - TimeSpan.FromMinutes(10)).Ticks,
                rate = 3
            });
            recv.chests = chests.ToArray();

            return JsonUtility.ToJson(recv);
        }

        /// <summary>
        /// バトル開始を実行する
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        string Battle(string data)
        {
            var recv = new BattleStartRecv();
            recv.seed = random.Next();

            var unit_random = new System.Random(recv.seed);

            recv.player = new[]
            {
                new UserUnit{ characterId = "Character_001_002", position = 0, skill = new string[]{ "Skill_001_001" } },
                new UserUnit{ characterId = "Character_001_004", position = 1, skill = new string[]{ "Skill_001_002" } },
                new UserUnit{ characterId = "Character_001_006", position = 2, skill = new string[]{ "Skill_001_003", "Skill_002_001" } },
                new UserUnit{ characterId = "Character_001_008", position = 3, skill = new string[]{ "Skill_001_001" } },
            };
            List<UserUnit> enemy = new List<UserUnit>();
            var count = unit_random.Next(1, 4);

            string[] skills = new[]
            {
                "Skill_001_001",
                "Skill_001_002",
                "Skill_001_003",
                "Skill_002_001",
            };
            
            for (int i = 0; i < count; i++)
            {
                var index = unit_random.Next(1, 253);

                var unit = new UserUnit
                {
                    characterId = string.Format("Character_001_{0:D3}", index),
                    position = i,
                    skill = new string[] { skills[unit_random.Next(0,4)] }
                };
                enemy.Add(unit);
            }
            recv.enemy = enemy.ToArray();
            return JsonUtility.ToJson(recv);
        }
    }
}

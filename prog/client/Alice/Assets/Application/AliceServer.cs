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
                case "Home": complete?.Invoke(getItems(data)); break;
                case "Battle": complete?.Invoke(Battle(data)); break;
            }
        }

        string getItems(string data)
        {
            return "ITEM_001_001";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        string Home(string data)
        {
            return "";
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
            recv.player = new[]
            {
                new UserUnit{ characterId = "Character_001_002", position = 0, skill = new string[]{ "Skill_001_001" } },
                new UserUnit{ characterId = "Character_001_004", position = 1, skill = new string[]{ "Skill_001_002" } },
                new UserUnit{ characterId = "Character_001_006", position = 2, skill = new string[]{ "Skill_001_003", "Skill_002_001" } },
                new UserUnit{ characterId = "Character_001_008", position = 3, skill = new string[]{ "Skill_001_001" } },
            };
            List<UserUnit> enemy = new List<UserUnit>();
            var count = random.Next(1, 4);

            string[] skills = new[]
            {
                "Skill_001_001",
                "Skill_001_002",
                "Skill_001_003",
                "Skill_002_001",
            };
            
            for (int i = 0; i < count; i++)
            {
                var index = random.Next(1, 11);

                var unit = new UserUnit
                {
                    characterId = string.Format("Character_001_{0:D3}", index),
                    position = i,
                    skill = new string[] { skills[random.Next(0,4)] }
                };
                enemy.Add(unit);
            }
            recv.enemy = enemy.ToArray();
            //recv.enemy = new[]
            //{
            //    new UserUnit{ characterId = "Character_001_003", position = 0, skill = new string[]{ "Skill_001_001" } },
            //    new UserUnit{ characterId = "Character_001_007", position = 1, skill = new string[]{ "Skill_001_002", "Skill_002_001" } },
            //    //new UserUnit{ characterId = "Character_001_009", position = 0, skill = new string[]{} },
            //    new UserUnit{ characterId = "Character_001_010", position = 3, skill = new string[]{ "Skill_001_003" } },
            //};
            return JsonUtility.ToJson(recv);
        }
    }
}

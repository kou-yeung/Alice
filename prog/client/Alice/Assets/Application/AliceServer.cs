using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zoo.Communication;

namespace Alice
{
    public class AliceServer : IDummyServer
    {
        public void Call(string proto, string data, Action<string> complete = null, Action<string> error = null)
        {
            switch (proto)
            {
                case "getItems": complete?.Invoke(getItems(data)); break;
                case "Battle": complete?.Invoke(Battle(data)); break;
            }
        }

        string getItems(string data)
        {
            return "ITEM_001_001";
        }

        /// <summary>
        /// バトル開始を実行する
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        string Battle(string data)
        {
            //new[] { "Skill_001_001", "Skill_001_002", "Skill_001_003" }
            var recv = new BattleStartRecv();
            recv.seed = 9527;
            recv.player = new[]
            {
                new UserUnit{ characterId = "Character_001_002", position = 0, skill = new string[]{ "Skill_001_001", "Skill_002_001" } },
                new UserUnit{ characterId = "Character_001_004", position = 1, skill = new string[]{ "Skill_001_002" } },
                new UserUnit{ characterId = "Character_001_006", position = 2, skill = new string[]{ "Skill_001_003", "Skill_002_001" } },
                new UserUnit{ characterId = "Character_001_008", position = 3, skill = new string[]{ "Skill_001_001" } },
            };
            recv.enemy = new[]
            {
                new UserUnit{ characterId = "Character_001_003", position = 0, skill = new string[]{ "Skill_001_001" } },
                new UserUnit{ characterId = "Character_001_007", position = 1, skill = new string[]{ "Skill_001_002", "Skill_002_001" } },
                //new UserUnit{ characterId = "Character_001_009", position = 0, skill = new string[]{} },
                new UserUnit{ characterId = "Character_001_010", position = 3, skill = new string[]{ "Skill_001_003" } },
            };
            return JsonUtility.ToJson(recv);
        }
    }
}

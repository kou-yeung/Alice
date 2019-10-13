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
            var recv = new BattleStartRecv();
            recv.player = new[]
            {
                "Character_001_002",
                "Character_001_004",
                "Character_001_006",
                "Character_001_008",
            };
            recv.enemy = new[]
            {
                "Character_001_003",
                "Character_001_007",
                "Character_001_009",
                "Character_001_010",
            };
            return JsonUtility.ToJson(recv);
        }
    }
}

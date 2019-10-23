using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zoo.Communication;
using Alice.Entities;

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
                case "GameSet": complete?.Invoke(GameSet(data)); break;
                case "BeginAds": complete?.Invoke(BeginAds(data)); break;
                case "EndAds": complete?.Invoke(EndAds(data)); break;
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

            List<UserUnit> units = new List<UserUnit>();
            var characters = MasterData.characters;
            var skills = MasterData.skills;
            foreach (var character in characters)
            {
                var unit = new UserUnit { characterId = character.ID, position = -1 };

                // スキル抽選
                var count = random.Next(0, 2);
                unit.skill = new string[count];
                for (int i = 0; i < count; i++)
                {
                    unit.skill[i] = skills[random.Next(0, skills.Length)].ID;
                }
                units.Add(unit);
            }

            // ユニットをセットする
            for (int i = 0; i < 4; i++)
            {
                units[random.Next(0, units.Count)].position = i;
            }

            // ユニット
            recv.units = units.ToArray();

            // 宝箱一覧
            List<UserChest> chests = new List<UserChest>();
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

            // 名前
            recv.names = new[] { "PLAYER", "ENEMY" };

            // プレイヤーユニット
            List<UserUnit> player = new List<UserUnit>();
            foreach (var unit in UserData.cacheUserDeck)
            {
                player.Add(JsonUtility.FromJson<UserUnit>(JsonUtility.ToJson(unit)));
            }
            recv.player = player.ToArray();

            // 相手ユニット
            List<UserUnit> enemy = new List<UserUnit>();
            var count = unit_random.Next(1, 4);
            var skills = MasterData.skills;
            var characters = MasterData.characters;
            for (int i = 0; i < count; i++)
            {
                var character = characters[unit_random.Next(0, characters.Length)];

                var unit = new UserUnit
                {
                    characterId = character.ID,
                    position = i,
                    skill = new string[] { skills[unit_random.Next(0, skills.Length)].ID }
                };
                enemy.Add(unit);
            }
            recv.enemy = enemy.ToArray();
            return JsonUtility.ToJson(recv);
        }

        /// <summary>
        /// 試合完了
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        string GameSet(string data)
        {
            var send = JsonUtility.FromJson<GameSetSend>(data);
            var recv = new GameSetRecv();

            // プレイヤー経験値追加
            recv.player = JsonUtility.FromJson<Player>(JsonUtility.ToJson(UserData.cacheHomeRecv.player));
            recv.player.exp += 1;

            List<UserUnit> modifiedUnit = new List<UserUnit>();
            // ユニットに経験値を与える
            foreach (var unit in UserData.cacheUserDeck)
            {
                var modified = JsonUtility.FromJson<UserUnit>(JsonUtility.ToJson(unit));
                ++modified.exp;
                modifiedUnit.Add(modified);
            }
            recv.modifiedUnit = modifiedUnit.ToArray();

            // 宝箱追加
            if (UserData.cacheHomeRecv.chests.Length < 3)
            {
                recv.modifiedChest = new[]
                {
                    new UserChest
                    {
                        uniq = Guid.NewGuid().ToString(),
                        start = DateTime.Now.Ticks,
                        end = (DateTime.Now + TimeSpan.FromMinutes(10)).Ticks,
                        rate = send.result == BattleConst.Result.Win?2:1
                    }
                };
            }
            return JsonUtility.ToJson(recv);
        }


        /// <summary>
        /// 広告開始
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        string BeginAds(string data)
        {
            Debug.Log(data);
            var res = new AdsBeginRecv();
            res.adsUniq = Guid.NewGuid().ToString();
            return JsonUtility.ToJson(res);
        }

        /// <summary>
        /// 広告終了
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        string EndAds(string data)
        {
            Debug.Log(data);
            var res = new AdsEndRecv();
            res.modifiedChest = new UserChest[0];
            return JsonUtility.ToJson(res);
        }
    }
}

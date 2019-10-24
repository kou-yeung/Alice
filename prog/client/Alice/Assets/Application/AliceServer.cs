using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zoo.Communication;
using Alice.Entities;
using System.Linq;

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
                case "Ads": complete?.Invoke(Ads(data)); break;
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

            var characters = MasterData.characters;
            var skills = MasterData.skills;

            // スキル
            recv.skills = skills.Select(v=> new UserSkill { id = v.ID, count = random.Next(1, 10) }).ToArray();

            // ユニット一覧生成
            List<UserUnit> units = new List<UserUnit>();
            foreach (var character in characters)
            {
                var unit = new UserUnit { characterId = character.ID };

                // スキル抽選
                var count = random.Next(0, 2);
                unit.skill = new string[count];
                for (int i = 0; i < count; i++)
                {
                    unit.skill[i] = skills[random.Next(0, skills.Length)].ID;
                }
                units.Add(unit);
            }
            // ユニット
            recv.units = units.ToArray();

            List<UserDeck> decks = new List<UserDeck>();
            // ユニットをセットする
            for (int i = 0; i < 4; i++)
            {
                var unit = units[random.Next(0, units.Count)];
                if (decks.Exists(v => v.characterId == unit.characterId)) continue;
                var deck = new UserDeck { characterId = unit.characterId, position = i };
                decks.Add(deck);
            }
            recv.decks = decks.ToArray();

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
            var c2s = JsonUtility.FromJson<BattleStartSend>(data);

            var s2c = new BattleStartRecv();
            s2c.seed = random.Next();

            var unit_random = new System.Random(s2c.seed);

            // 名前
            s2c.names = new[] { c2s.player.name, "ENEMY" };
            // プレイヤーユニット
            s2c.playerUnit = c2s.units;
            s2c.playerDeck = c2s.decks;

            // 相手ユニット
            var skills = MasterData.skills;
            var characters = MasterData.characters;

            List<UserUnit> enemyUnit = new List<UserUnit>();
            List<UserDeck> enemyDeck = new List<UserDeck>();
            var count = unit_random.Next(2, 4);
            for (int i = 0; i < count; i++)
            {
                // キャラ抽選
                var character = characters[unit_random.Next(0, characters.Length)];
                if (enemyDeck.Exists(v => v.characterId == character.ID)) continue;

                var unit = new UserUnit();
                unit.characterId = character.ID;
                unit.skill = new string[] { skills[unit_random.Next(0, skills.Length)].ID };
                enemyUnit.Add(unit);

                var deck = new UserDeck();
                deck.characterId = unit.characterId;
                deck.position = i;
                enemyDeck.Add(deck);
            }
            s2c.enemyUnit = enemyUnit.ToArray();
            s2c.enemyDeck = enemyDeck.ToArray();
            return JsonUtility.ToJson(s2c);
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
            foreach (var deck in UserData.cacheHomeRecv.decks)
            {
                var modified = JsonUtility.FromJson<UserDeck>(JsonUtility.ToJson(deck));
                var unit = UserData.cacheHomeRecv.units.First(v => v.characterId == modified.characterId);
                ++unit.exp;
                modifiedUnit.Add(unit);
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
        /// 広告終了
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        string Ads(string data)
        {
            Debug.Log(data);
            var c2v = JsonUtility.FromJson<AdsSend>(data);
            // 時間を減らす
            c2v.chest.end -= TimeSpan.FromMinutes(5).Ticks;

            // 返信
            var s2c = new AdsRecv();
            s2c.modifiedChest = c2v.chest;
            s2c.modifiedAds = Guid.NewGuid().ToString();
            return JsonUtility.ToJson(s2c);
        }
    }
}

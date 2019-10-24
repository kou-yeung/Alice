using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zoo.Communication;
using Alice.Entities;
using System.Linq;
using System.IO;

namespace Alice
{
    public class AliceServer : IDummyServer
    {
        static readonly string fn = Path.Combine(Application.persistentDataPath, "DB.txt");
        /// <summary>
        /// 仮のDBを提供する
        /// </summary>
        [Serializable]
        class DB
        {
            public Player player;
            public UserUnit[] units;
            public UserDeck[] decks;
            public UserChest[] chests;
            public UserSkill[] skills;
            public string token;
        }

        DB db;
        public AliceServer()
        {
            if (File.Exists(fn))
            {
                db = JsonUtility.FromJson<DB>(File.ReadAllText(fn));
            }
        }

        /// <summary>
        /// 新規ユーザの初期化設定
        /// </summary>
        void CreateDB()
        {
            // 生成する
            db = new DB();
            var random = new System.Random();
            // 情報を設定する
            db.player = new Player { name = "ゲスト" };
            //　ユニットの適当に抽選する
            var characters = MasterData.characters;
            var unit = new UserUnit();
            unit.characterId = characters[random.Next(0, characters.Length)].ID;
            unit.skill = new string[0];
            db.units = new[] { unit };
            // デッキにセットする
            db.decks = new[] { new UserDeck { characterId = unit.characterId, position = 0 } };
            // 所持スキル
            db.skills = new UserSkill[0];
            // 宝箱はなかった
            db.chests = new UserChest[0];
            // 認証トークン
            db.token = Guid.NewGuid().ToString();
            // 同期
            Sync();
        }
        /// <summary>
        /// 生成済から
        /// </summary>
        /// <returns></returns>
        bool Created()
        {
            return db != null;
        }
        /// <summary>
        /// 同期する
        /// </summary>
        void Sync()
        {
            File.WriteAllText(fn, JsonUtility.ToJson(db));
        }


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
            if(!Created())
            {
                CreateDB();
            }

            HomeRecv recv = new HomeRecv();
            // プレイヤー情報
            recv.player = db.player;
            // スキル
            recv.skills = db.skills;
            // ユニット
            recv.units = db.units;
            // デッキ情報
            recv.decks = db.decks;
            // 宝箱一覧
            recv.chests = db.chests;
            // 認証トークン
            recv.token = db.token;

            return JsonUtility.ToJson(recv);
        }

        /// <summary>
        /// バトル開始を実行する
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        string Battle(string data)
        {
            Debug.Log(data);
            var c2s = JsonUtility.FromJson<BattleStartSend>(data);

            // デッキ情報とユニットのスキル情報を同期する
            db.decks = c2s.decks;
            foreach (var unit in c2s.units)
            {
                var index = Array.FindIndex(db.units, v => v.characterId == unit.characterId);
                db.units[index] = unit;
            }
            // 同期する
            Sync();

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
            var c2s = JsonUtility.FromJson<GameSetSend>(data);
            var s2c = new GameSetRecv();

            // プレイヤー経験値追加(仮
            db.player.exp += 1;
            s2c.player = db.player;

            // デッキにセットしたユニットに経験値を与える
            List<UserUnit> modifiedUnit = new List<UserUnit>();
            foreach (var deck in db.decks)
            {
                var index = Array.FindIndex(db.units, v => v.characterId == deck.characterId);
                db.units[index].exp += 1;
                modifiedUnit.Add(db.units[index]);
            }
            s2c.modifiedUnit = modifiedUnit.ToArray();

            // 宝箱追加
            if (db.chests.Length < 3)
            {
                var reward = new[]
                {
                    new UserChest
                    {
                        uniq = Guid.NewGuid().ToString(),
                        start = DateTime.Now.Ticks,
                        end = (DateTime.Now + TimeSpan.FromMinutes(10)).Ticks,
                        rate = c2s.result == BattleConst.Result.Win?2:1
                    }
                };
                // 末尾に追加
                db.chests = db.chests.Concat(reward).ToArray();
                s2c.modifiedChest = reward;
            }
            Sync();

            return JsonUtility.ToJson(s2c);
        }

        /// <summary>
        /// 広告終了
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        string Ads(string data)
        {
            var c2s = JsonUtility.FromJson<AdsSend>(data);
            var s2c = new AdsRecv();

            if(c2s.token != db.token)
            {
                throw new Exception("Token 無効");
            }

            // 指定された宝箱の時間を減らす
            for (int i = 0; i < db.chests.Length; i++)
            {
                if(db.chests[i].uniq == c2s.chest.uniq)
                {
                    var time = TimeSpan.FromMinutes(5).Ticks;
                    db.chests[i].start -= time;
                    db.chests[i].end -= time;
                    s2c.modifiedChest = db.chests[i];
                    break;
                }
            }
            db.token = Guid.NewGuid().ToString();
            // 同期する
            Sync();

            // 返信
            s2c.modifiedToken = db.token;
            return JsonUtility.ToJson(s2c);
        }
    }
}

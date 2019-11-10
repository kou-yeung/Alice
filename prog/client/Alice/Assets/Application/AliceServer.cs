using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zoo.Communication;
using Alice.Entities;
using System.Linq;
using System.IO;
using Zoo.Time;

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
            public UserDeck deck;
            public UserUnit[] units;
            public UserChest[] chests;
            public UserSkill[] skills;
        }

        DB db;
        System.Random random = new System.Random();

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
            // 情報を設定する
            db.player = new Player { name = "ゲスト", token = Guid.NewGuid().ToString() };
            //　ユニットの適当に抽選する
            var characters = MasterData.Instance.characters;
            var unit = new UserUnit();
            unit.characterId = characters[random.Next(0, characters.Length)].ID;
            db.units = new[] { unit };
            // デッキ情報にセットする
            db.deck = new UserDeck { ids = new[] { unit.characterId, "", "", "" } };
            // 所持スキル
            unit.skill = new string[0];
            // 所持スキル
            db.skills = new UserSkill[0];
            // 宝箱はなかった
            db.chests = new UserChest[0];
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

        public void Call(string proto, string data, Action<string> complete = null, Action<string> error = null)
        {
            switch (proto)
            {
                case "Home": complete?.Invoke(Home(data)); break;
                case "Battle": complete?.Invoke(Battle(data)); break;
                case "GameSet": complete?.Invoke(GameSet(data)); break;
                case "Ads": complete?.Invoke(Ads(data)); break;
                case "Chest": complete?.Invoke(Chest(data)); break;
            }
        }

        /// <summary>
        /// ログインボーナスのチェック
        /// </summary>
        /// <returns></returns>
        bool CheckLoginBonus()
        {
            var stamp = new DateTime(db.player.stamp);
            var today = DateTime.Today;
            // 24時間以内の場合、falseを返す
            if (DateTime.Today - stamp < TimeSpan.FromHours(24)) return false;
            // 24時間以上の場合、スタンプを更新します
            db.player.stamp = today.Ticks;
            db.player.loginCount += 1;          // 累計ログイン日数 + 1

            // 本日戦闘回数が１０回以上のみチェックする
            if(db.player.todayBattleCount >= 10)
            {
                var count = (float)db.player.todayBattleCount;
                var win = (float)db.player.todayWinCount;

                if( (win/ count) >= 0.65f)
                {
                    // 勝率65%以上ならランクアップ
                    db.player.rank += 1;
                } else
                {
                    // 下回ったらランクダウン
                    db.player.rank = Mathf.Max(db.player.rank - 1, 0 );
                }
            }
            // 本日バトル回数リセット
            db.player.todayBattleCount = 0;     // 本日のバトル回数リセット
            db.player.todayWinCount = 0;        // 本日のバトル回数リセット
            // 広告使用回数回復
            db.player.ads = 15;                 // 仮で15回
            Sync();
            return true;
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
            if(CheckLoginBonus())
            {
                Debug.Log("ログインボーナスもらったよ～");
            }

            HomeRecv s2c = new HomeRecv();
            s2c.svtime = DateTimeOffset.Now.ToUnixTimeSeconds();
            // プレイヤー情報
            s2c.player = db.player;
            // デッキ情報
            s2c.deck = db.deck;
            // スキル
            s2c.skills = db.skills;
            // ユニット
            s2c.units = db.units;
            // 宝箱一覧
            s2c.chests = db.chests;

            return JsonUtility.ToJson(s2c);
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

            // プレイヤー名が変更されたら更新する
            if(c2s.player.name != db.player.name)
            {
                db.player.name = c2s.player.name;
            }
            // デッキ更新
            db.deck = c2s.deck;

            // 情報更新したユニットを同期する
            foreach (var unit in c2s.edited)
            {
                var index = Array.FindIndex(db.units, v => v.characterId == unit.characterId);
                db.units[index] = unit;
            }
            // 同期する
            Sync();

            var s2c = new BattleStartRecv();
            s2c.seed = random.Next();

            var unit_random = new System.Random(s2c.seed);

            // プレイヤーユニット
            s2c.playerUnit = c2s.units;
            s2c.playerDeck = db.deck;
            foreach (var v in s2c.playerUnit)
            {
                v.skill = v.skill.Where(n => !string.IsNullOrEmpty(n)).ToArray();
            }

            // 相手ユニット
            // MEMO : グラウンドサーバの場合、同じランクのプレイヤーデッキを検索する
            // なければ推薦ユニットで構築する
            var hasOther = false;
            if(!hasOther)
            {
                // 名前
                s2c.names = new[] { c2s.player.name, "ゲスト" };
                s2c.enemyUnit = c2s.recommendUnits;
                s2c.enemyDeck = c2s.recommendDeck;
            }
            else
            {
                // 名前
                //s2c.names = new[] { c2s.player.name, other.name };
                //s2c.enemyUnit = enemyUnit.ToArray();
                //s2c.enemyDeck = enemyDeck.ToArray();
            }
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
            s2c.modified = new Modified();

            if (CheckLoginBonus())
            {
                Debug.Log("ログインボーナスもらったよ～");
            }

            db.player.totalBattleCount += 1;    // 累計バトル回数 + 1
            db.player.todayBattleCount += 1;    // 本日のバトル回数 + 1
            if (c2s.result == BattleConst.Result.Win)
            {
                db.player.todayWinCount += 1;    // 本日の勝利回数 + 1
            }
            s2c.modified.player = new[] { db.player };

            // デッキにセットしたユニットに経験値を与える
            List<UserUnit> modifiedUnit = new List<UserUnit>();
            foreach (var deck in db.deck.ids.Where(v => !string.IsNullOrEmpty(v)))
            {
                var index = Array.FindIndex(db.units, v => v.characterId == deck);
                db.units[index].exp += 1;
                modifiedUnit.Add(db.units[index]);
            }
            s2c.modified.unit = modifiedUnit.ToArray();

            // 宝箱追加
            if (db.chests.Length < 3)
            {
                var now = DateTimeOffset.Now.ToUnixTimeSeconds();

                var reward = new[]
                {
                    new UserChest
                    {
                        uniq = Guid.NewGuid().ToString(),
                        start = now,
                        end = now + (15*60),
                        rate = c2s.result == BattleConst.Result.Win?2:1
                    }
                };
                // 末尾に追加
                db.chests = db.chests.Concat(reward).ToArray();
                s2c.modified.chest = reward;
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

            if(c2s.token != db.player.token)
            {
                throw new Exception("Token 無効");
            }

            if(db.player.ads <= 0)
            {
                throw new Exception("これ以上使用できない");
            }

            List<UserChest> modifiedChest = new List<UserChest>();

            // 指定された宝箱の時間を減らす
            for (int i = 0; i < db.chests.Length; i++)
            {
                if(db.chests[i].uniq == c2s.chest.uniq)
                {
                    var time = TimeSpan.FromMinutes(10).Ticks;
                    db.chests[i].start -= time;
                    db.chests[i].end -= time;
                    modifiedChest.Add(db.chests[i]);
                    break;
                }
            }
            db.player.ads -= 1; // 使用した
            db.player.token = Guid.NewGuid().ToString();
            // 同期する
            Sync();

            // 返信
            var s2c = new AdsRecv();
            s2c.modified = new Modified
            {
                player = new[] { db.player },
                chest = modifiedChest.ToArray(),
            };
            return JsonUtility.ToJson(s2c);
        }

        /// <summary>
        /// 宝箱開く
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        string Chest(string data)
        {
            var c2s = JsonUtility.FromJson<ChestSend>(data);

            if (c2s.chest.end > ServerTime.CurrentUnixTime)
            {
                throw new Exception("まだ開けません");
            }

            var s2c = new ChestRecv();
            s2c.modified = new Modified();

            // 宝箱を消す
            db.chests = db.chests.Where(v => v.uniq != c2s.chest.uniq).ToArray();
            s2c.modified.chest = db.chests;
            s2c.modified.remove = new[] { c2s.chest };

            // MEMO : 現在適当に1/2の確率で[Unit][Skill]分岐
            if (random.Next() % 2 == 0 && db.units.Length < MasterData.Instance.characters.Length)
            {
                var character = MasterData.Instance.characters;
                var lots = character.Where(v => !Array.Exists(db.units, u => u.characterId == v.ID)).ToArray();
                var id = lots[random.Next(0, lots.Length)].ID;
                var add = new UserUnit();
                add.characterId = id;
                add.skill = new string[0];

                db.units = db.units.Concat(new[] { add }).ToArray();
                s2c.modified.unit = new[] { add };
            }
            else
            {
                // スキル
                var skill = MasterData.Instance.skills;
                var id = skill[random.Next(0, skill.Length)].ID;

                var index = Array.FindIndex(db.skills, v => v.id == id);
                if(index != -1)
                {
                    db.skills[index].count += 1;
                    s2c.modified.skill = new[] { db.skills[index] };
                }
                else
                {
                    var add = new UserSkill { id = id, count = 1 };
                    db.skills = db.skills.Concat(new[] { add }).ToArray();
                    s2c.modified.skill = new[] { add };
                }
            }
            Sync();
            return JsonUtility.ToJson(s2c);
        }
    }
}

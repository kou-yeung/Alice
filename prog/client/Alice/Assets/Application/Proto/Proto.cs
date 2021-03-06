﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using Alice.Generic;
using Zoo.Time;
using Alice.Logic;
using Zoo.Communication;

namespace Alice
{
    /// <summary>
    /// 所持スキルx1
    /// </summary>
    [Serializable]
    public class UserSkill
    {
        public string id;
        public int count;
    }

    /// <summary>
    /// Unit x 1
    /// </summary>
    [Serializable]
    public class UserUnit
    {
        public string characterId;
        public string[] skill;
        public int exp;             // 経験値:戦闘回数

        /// <summary>
        /// レベル
        /// </summary>
        /// <returns></returns>
        public int Level()
        {
            return LevelTable.Exp2Level(exp);
        }

        /// <summary>
        /// 経験値割合
        /// </summary>
        /// <returns></returns>
        public float Ratio2Levelup()
        {
            var level = Level();
            var start = LevelTable.LevelExp(level - 1);
            var end = LevelTable.LevelExp(level);
            return (float)(exp - start) / (float)(end - start);
        }
    }

    /// <summary>
    /// バトル開始: CL -> SV
    /// </summary>
    public class BattleStartSend
    {
        public Player player;       // プレイヤー情報
        public UserDeck deck;       // デッキ情報
        public UserUnit[] units;    // バトルに使用するユニット

        public UserUnit[] edited;  // 情報を更新したユニット

        // 推薦敵ユニット:サーバ上にヒットした相手がなければこちらを使って対戦させます
        public UserUnit[] recommendUnits;    // 推薦的ユニット
        public UserDeck recommendDeck;     // 配置

        public BattleStartSend()
        {
            var cache = UserData.cacheHomeRecv;
            this.player = cache.player;
            this.deck = cache.deck;
            this.units = cache.units.Where(v => Array.Exists(cache.deck.ids, id => id == v.characterId)).ToArray();
            this.edited = UserData.editedUnit.Values.ToArray();
            // 推薦敵
            var recommend = BattleEnemy.Gen(this.player/*, this.units*/);
            this.recommendUnits = recommend.unit;
            this.recommendDeck = recommend.deck;
        }
    }

    /// <summary>
    /// バトル開始: SV -> CL
    /// </summary>
    [Serializable]
    public class BattleStartRecv
    {
        public int seed;
        public BattleConst.BattleType type;
        public string[] names;  // 名前
        // 味方のユニット情報
        public UserUnit[] playerUnit;
        public UserDeck playerDeck;
        // 相手のユニット情報
        public UserUnit[] enemyUnit;
        public UserDeck enemyDeck;
        // 以下はクライアント側が生成する結果
        public BattleConst.Result result;

        // シャドウ情報から生成する
        public static BattleStartRecv Conversion(ShadowSelf self, ShadowEnemy enemy)
        {
            var res = new BattleStartRecv();
            res.seed = enemy.seed;
            res.type = BattleConst.BattleType.Shadow;
            res.names = new[] { self.name, enemy.name };
            res.playerUnit = self.unit;
            res.playerDeck = self.deck;
            res.enemyUnit = enemy.unit;
            res.enemyDeck = enemy.deck;
            return res;
        }

    }

    /// <summary>
    /// 宝箱
    /// </summary>
    [Serializable]
    public class UserChest
    {
        public string uniq;    // アクセス用ID
        public long created; // 生成時間:変更しない
        public long start; // 開始時間
        public long end;   // 終了時間
        public int rate;   // レアリティ

        /// <summary>
        /// 開ける準備ができた？
        /// </summary>
        /// <returns></returns>
        public bool IsReady()
        {
            return Remain() <= 0;
        }
        /// <summary>
        /// 残り時間
        /// </summary>
        /// <returns></returns>
        public long Remain()
        {
            return Math.Max(0, end - ServerTime.CurrentUnixTime);
        }

        /// <summary>
        /// 残り時間文字列
        /// </summary>
        /// <returns></returns>
        public string RemainText()
        {
            var remain = Remain();

            var hh = remain / 3600;
            var mm = (remain % 3600) / 60;
            var ss = (remain % 60);
            return string.Format("{0:D2}:{1:D2}:{2:D2}", hh, mm, ss);
        }

        /// <summary>
        /// 残り時間の割合
        /// </summary>
        /// <returns></returns>
        public float RemainRatio()
        {
            var max = end - start;
            return (float)Remain() / (float)max;
        }

        /// <summary>
        /// 必要なアラーム数
        /// </summary>
        /// <returns></returns>
        public int NeedAlarmNum()
        {
            return Mathf.CeilToInt(Remain() / (float)Const.AlarmTimeSecond);
        }

        /// <summary>
        /// 開く
        /// </summary>
        public void Open()
        {

        }
    }

    /// <summary>
    /// プレイヤー情報
    /// </summary>
    [Serializable]
    public class Player
    {
        public string name;     // ユーザ名
        public int alarm;       // アラーム(時間短縮アイテム
        public int rank;        // プレイヤーランキング
        public int ads;         // 残り広告使用回数
        public string token;    // 認証トークン
        public long stamp;      // 最後ログインの日付
        public int loginCount;  // 累計ログイン日数
        public int totalBattleCount;    // 累計バトル回数
        public int todayBattleCount;    // 本日バトルした回数
        public int todayWinCount;       // 本日勝利した回数
        public int roomid = -1; // 最後に生成したシャドウのroomid
        public int tutorialFlag;    // チュートリアルフラグ

        public bool HasTutorialFlag(Const.TutorialFlag flag)
        {
            var flags = (Const.TutorialFlag)this.tutorialFlag;
            return flags.HasFlag(flag);
        }

        public void AddTutorialFlag(Const.TutorialFlag flag)
        {
            var flags = (Const.TutorialFlag)this.tutorialFlag;
            this.tutorialFlag = (int)(flags | flag);
        }


        public void ExecTutorial(Const.TutorialFlag flag, Action cb = null)
        {
            if (HasTutorialFlag(flag))
            {
                cb?.Invoke();
                return;
            }

            switch (flag)
            {
                case Const.TutorialFlag.UserNameInput:
                    NameEditDialog.Show(() =>
                    {
                        var tutorialData = new TutorialData { Desc = "BATTLE_DESC".TextData(), TargetButton = "Base/Battle" };
                        TutorialDialog.Show(tutorialData);
                    });
                    break;
                case Const.TutorialFlag.Room:
                    Dialog.Show("ROOM_TUTORIAL".TextData(), Dialog.Type.SubmitOnly);
                    break;
                case Const.TutorialFlag.Record:
                    Dialog.Show("RECORD_TUTORIAL".TextData(), Dialog.Type.SubmitOnly);
                    break;
                case Const.TutorialFlag.Shadow:
                    {
                        var tutorialData = new TutorialData { Desc = "SHADOW_TUTORIAL".TextData(), TargetButton = "VS/Create/Button" };
                        TutorialDialog.Show(tutorialData);
                    }
                    break;
                case Const.TutorialFlag.HeaderRank:
                    {
                        var tutorialData = new TutorialData { Desc = "HEADER_RANK_TUTORIAL".TextData(), TargetButton = "Header/RankIcon" };
                        TutorialDialog.Show(tutorialData);
                    }
                    break;
            }
            // フラグ更新
            AddTutorialFlag(flag);
            // 同期する
            Sync(cb);
        }

        /// <summary>
        /// 同期する
        /// </summary>
        /// <param name="cb"></param>
        public void Sync(Action cb = null)
        {
            var c2s = new SyncSend { player = this };
            // 同期する
            CommunicationService.Instance.Request("PlayerSync", JsonUtility.ToJson(c2s), (res) =>
            {
                UserData.Modify(JsonUtility.FromJson<SyncRecv>(res).modified);
                cb?.Invoke();
            });
        }

    }

    /// <summary>
    /// プレイヤー情報
    /// </summary>
    [Serializable]
    public class UserDeck
    {
        public string[] ids;
    }

    /// <summary>
    /// 編集した差分データ
    /// </summary>
    [Serializable]
    public class Modified
    {
        public Player[] player;         // プレイヤー情報
        public UserSkill[] skill;       // スキルデータ
        public UserUnit[] unit;         // ユニットデータ
        public UserChest[] chest;       // 宝箱データ
        public UserChest[] remove;      // 削除した宝箱
        public string appVersion;       // アプリバージョン
        public int[] nextBonusTime;   // 次のボーナス取得時間
    }

    [Serializable]
    public class Bonus
    {
        public int alarm;
        public bool rankup;
    }

    /// <summary>
    /// ホーム画面: SV -> CL
    /// </summary>
    [Serializable]
    public class HomeRecv
    {
        public long svtime;         // サーバタイム
        public Player player;
        public UserDeck deck;       // デッキ情報
        public UserUnit[] units;    // ユニット一覧
        public UserSkill[] skills;  // スキル一覧
        public UserChest[] chests;

        public string appVersion;   // アプリバージョン
        public bool waitCreate;     // プレイヤーデータ作成待ち
        public long nextBonusTime;  // 次のボーナス更新時間
    }

    [Serializable]
    public class GameSetSend
    {
        public BattleConst.Result result; // 試合結果
    }

    [Serializable]
    public class GameSetRecv
    {
        public Modified modified;
        public int alarm;           // ボーナスアラーム
    }

    /// <summary>
    /// 広告を観ました: cl -> sv
    /// </summary>
    [Serializable]
    public class AdsSend
    {
        public string token;        // 認証トークン
        public UserChest chest;     // 対象
    }
    /// <summary>
    /// 広告終了: sv -> cl
    /// </summary>
    [Serializable]
    public class AdsRecv
    {
        public Modified modified;
    }

    /// <summary>
    /// 宝箱を開く: cl -> sv
    /// </summary>
    [Serializable]
    public class ChestSend
    {
        public UserChest chest;     // 宝箱
    }

    /// <summary>
    /// 宝箱を開く: sv -> cl
    /// </summary>
    [Serializable]
    public class ChestRecv
    {
        public Modified modified;       // 更新したデータ
    }

    /// <summary>
    /// シャドウを生成 cl -> sv
    /// </summary>
    [Serializable]
    public class ShadowCreateSend
    {
        public Player player;       // 自分情報
        public UserDeck deck;       // デッキ情報
        public UserUnit[] units;    // バトルに使用するユニット
        public UserUnit[] edited;    // 同期必要ユニット

        public ShadowCreateSend()
        {
            var cache = UserData.cacheHomeRecv;
            this.player = cache.player;
            this.deck = cache.deck;
            this.units = cache.units.Where(v => Array.Exists(cache.deck.ids, id => id == v.characterId)).ToArray();
            this.edited = UserData.editedUnit.Values.ToArray();
        }
    }

    /// <summary>
    /// シャドウを生成 sv -> cl
    /// </summary>
    [Serializable]
    public class ShadowCreateRecv
    {
        public int roomId;      // ルームID
        public ShadowSelf self;
    }

    /// <summary>
    /// シャドウを生成 cl -> sv
    /// 返信は BattleStartRecv です！！注意
    /// </summary>
    [Serializable]
    public class ShadowBattleSend
    {
        public int roomid;
        public Player player;       // 自分情報
        public UserDeck deck;       // デッキ情報
        public UserUnit[] units;    // バトルに使用するユニット
        public UserUnit[] edited;  // 情報を更新したユニット

        public ShadowBattleSend(int roomid)
        {
            this.roomid = roomid;
            var cache = UserData.cacheHomeRecv;
            this.player = cache.player;
            this.deck = cache.deck;
            this.units = cache.units.Where(v => Array.Exists(cache.deck.ids, id => id == v.characterId)).ToArray();
            this.edited = UserData.editedUnit.Values.ToArray();
        }
    }

    /// <summary>
    /// シャドウ敵情報
    /// </summary>
    [Serializable]
    public class ShadowEnemy
    {
        public int seed;        // 乱数シード
        public string name;     // 名前
        public UserUnit[] unit;
        public UserDeck deck;
    }

    /// <summary>
    /// シャドウ自分情報
    /// </summary>
    [Serializable]
    public class ShadowSelf
    {
        public string name;  // 名前
        public UserUnit[] unit;
        public UserDeck deck;
    }
    /// <summary>
    /// 自分が再生したシャドウのバトル一覧を取得する:c2s
    /// </summary>
    [Serializable]
    public class ShadowListSend
    {
        public int roomid;
    }
    /// <summary>
    /// 自分が再生したシャドウのバトル一覧を取得する:s2c
    /// </summary>
    [Serializable]
    public class ShadowListRecv
    {
        public int roomid;
        public bool isActive;           // まだ有効なのか？
        public ShadowSelf self;         // 自分のシャドウ情報
        public ShadowEnemy[] enemies;   // 敵のシャドウ情報一覧
    }

    /// <summary>
    /// 購入確認 : c2s
    /// </summary>
    [Serializable]
    public class PurchasingSend
    {
        public string id;
        public string platform;
        public string receipt;
    }
    /// <summary>
    /// 購入確認 : s2c
    /// </summary>
    [Serializable]
    public class PurchasingRecv
    {
        public Modified modified;       // 更新したデータ
    }

    /// <summary>
    /// 同期情報
    /// </summary>
    [Serializable]
    public class SyncSend
    {
        public Player player;         // 自分情報
    }

    [Serializable]
    class SyncRecv
    {
        public Modified modified;   // 更新したデータ
        public long nextBonusTime;  // 次のボーナス更新時間
    }

}

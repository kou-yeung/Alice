using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

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
            return Mathf.FloorToInt(Mathf.Sqrt(exp)) + 1;
        }
    }

    /// <summary>
    /// デッキ情報
    /// </summary>
    [Serializable]
    public class UserDeck
    {
        public string characterId;
        public int position;
    }

    /// <summary>
    /// バトル開始: CL -> SV
    /// </summary>
    public class BattleStartSend
    {
        public Player player;       // プレイヤー情報
        public UserUnit[] units;    // バトルに使用するユニット
        public UserDeck[] decks;    // デッキの配置情報
        public UserUnit[] edited;  // 情報を更新したユニット
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
        public UserDeck[] playerDeck;
        // 相手のユニット情報
        public UserUnit[] enemyUnit;
        public UserDeck[] enemyDeck;
        // 以下はクライアント側が生成する結果
        public BattleConst.Result result;
    }


    /// <summary>
    /// 宝箱
    /// </summary>
    [Serializable]
    public class UserChest
    {
        public string uniq;    // アクセス用ID
        public long start; // 開始時間
        public long end;   // 終了時間
        public int rate;   // レアリティ
    }

    /// <summary>
    /// プレイヤー情報
    /// </summary>
    [Serializable]
    public class Player
    {
        public string name;     // ユーザ名
        public int exp;         // 戦闘した回数
        public int coin;        // コイン(将来か課金で買えるようにします
        public string token;    // 認証トークン
        public long stamp;      // 最後ログインの日付
        public int loginCount;  // 累計ログイン日数

        /// <summary>
        /// プレイヤーレベル
        /// </summary>
        /// <returns></returns>
        public int Level()
        {
            return Mathf.FloorToInt(Mathf.Pow(Mathf.Pow(exp / 2, 0.5f) / 2, 0.5f)) + 1;
        }
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
    }

    /// <summary>
    /// ホーム画面: SV -> CL
    /// </summary>
    [Serializable]
    public class HomeRecv
    {
        public Player player;
        public UserUnit[] units;    // ユニット一覧
        public UserDeck[] decks;    // デッキの配置情報
        public UserSkill[] skills;  // スキル一覧
        public UserChest[] chests;
    }

    [Serializable]
    public class GameSetSend
    {
        public string ID;    // バトルID
        public BattleConst.Result result; // 試合結果
    }

    [Serializable]
    public class GameSetRecv
    {
        public Player player;
        public Modified modified;
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
        public bool useItem;        // アイテム使用
    }

    /// <summary>
    /// 宝箱を開く: sv -> cl
    /// </summary>
    [Serializable]
    public class ChestRecv
    {
        public Modified modified;       // 更新したデータ
    }
}

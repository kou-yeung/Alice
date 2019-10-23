﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Alice
{
    /// <summary>
    /// バトル開始: CL -> SV
    /// </summary>
    public class BattleStartSend
    {
    }


    /// <summary>
    /// Unit x 1
    /// </summary>
    [Serializable]
    public class UserUnit
    {
        public string characterId;
        public int position;        // セットされた場合[0-3] セットされてない場合[-1]
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
    /// バトル開始: SV -> CL
    /// </summary>
    [Serializable]
    public class BattleStartRecv
    {
        public int seed;
        public BattleConst.BattleType type;
        public string[] names;  // 名前
        public UserUnit[] player;
        public UserUnit[] enemy;
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
        public string name; // ユーザ名
        public int exp;     // 戦闘した回数
        public int coin;    // コイン(将来か課金で買えるようにします

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
    /// ホーム画面: SV -> CL
    /// </summary>
    [Serializable]
    public class HomeRecv
    {
        public Player player;
        public UserUnit[] units;
        public UserChest[] chests;
        public string ads;  // 広告用認証キー
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
        public UserUnit[] modifiedUnit;
        public UserChest[] modifiedChest;
    }

    /// <summary>
    /// 広告を観ました: cl -> sv
    /// </summary>
    [Serializable]
    public class AdsSend
    {
        public string ads;          // Ads認証ID
        public UserChest chest;     // 対象
    }
    /// <summary>
    /// 広告終了: sv -> cl
    /// </summary>
    [Serializable]
    public class AdsRecv
    {
        public string modifiedAds;          // Ads認証ID
        public UserChest modifiedChest;     // 更新された宝箱
    }
}


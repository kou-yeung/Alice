using System.Collections;
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
        public int position;
        public string[] skill; 
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
    /// ホーム画面: SV -> CL
    /// </summary>
    [Serializable]
    public class HomeRecv
    {
        public UserChest[] chests;
    }
}


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
        public UserUnit[] player;
        public UserUnit[] enemy;
    }
}


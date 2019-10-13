using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Alice
{
    /// <summary>
    /// バトル開始: CL -> SV
    /// </summary>
    public class BattleStartSend
    {
    }

    /// <summary>
    /// バトル開始: SV -> CL
    /// </summary>
    public class BattleStartRecv
    {
        public string[] player;
        public string[] enemy;
    }
}


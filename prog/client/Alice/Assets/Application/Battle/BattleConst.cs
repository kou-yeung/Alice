using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Alice
{
    public static class BattleConst
    {
        public enum State
        {
            Start,      // バトル開始
            Timeline,   // タイムライン更新
            Action,     // アクション選択
            Playback,   // 選択したアクションの再生
        }
    }
}

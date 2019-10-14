using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Alice
{
    public static class BattleConst
    {
        public enum State
        {
            Init,       // 初期化
            Start,      // バトル開始
            Timeline,   // タイムライン更新
            Action,     // アクション選択
            Playback,   // 選択したアクションの再生
        }

        /// <summary>
        /// プレイヤーユニットの座標
        /// </summary>
        public static readonly Vector3[] PlayerUnitPositions = new Vector3[]
        {
            new Vector3(-50,-115),
            new Vector3(-125,-150),
            new Vector3(-200,-115),
            new Vector3(-280,-150),
        };
        /// <summary>
        /// エネミーユニットの座標
        /// </summary>
        public static readonly Vector3[] EnemyUnitPositions = new Vector3[]
        {
            new Vector3(50,-115),
            new Vector3(125,-150),
            new Vector3(200,-115),
            new Vector3(280,-150),
        };

        /// <summary>
        /// 
        /// </summary>
        public enum Side
        {
            Player,
            Enemy,
        }

        /// <summary>
        /// 効果
        /// </summary>
        public enum Effect
        {
            // 即座効果
            Damage = 1,
            Recovery,

            // バフ
            Buff_Base = 200,            // 計算に使用する
            Buff_All = Buff_Base,       // バフ:全パラメータ
            Buff_Atk,                   // バフ:ATK
            Buff_Def,                   // バフ:Def
            Buff_MAtk,                  // バフ:MAtk
            Buff_MDef,                  // バフ:MDef

            // デバフ
            Debuff_Base = 300,     // 計算に使用する
            Debuff_All = 300,      // デバフ:全パラメータ
            Debuff_Atk,            // デバフ:ATK
            Debuff_Def,            // デバフ:Def
            Debuff_MAtk,           // デバフ:MAtk
            Debuff_MDef,           // デバフ:MDef
        }
    }
}



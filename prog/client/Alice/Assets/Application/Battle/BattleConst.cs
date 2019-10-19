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
            TurnEnd,    // ターンエンド
            GameSet,    // ゲーム終了
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
            Damage = 101,               // 通常ダメージ
            DamageRatio,                // 割合ダメージ
            Recovery,                   // 通常回復
            RecoveryRatio,              // 割合回復

            // バフ
            Buff_Base = 200,            // 計算に使用する
            Buff_Atk,                   // バフ:ATK
            Buff_Def,                   // バフ:Def
            Buff_MAtk,                  // バフ:MAtk
            Buff_MDef,                  // バフ:MDef
            Buff_Wait,                  // バフ:MDef

            // デバフ
            Debuff_Base = 300,          // 計算に使用する
            Debuff_Atk,                 // デバフ:ATK
            Debuff_Def,                 // デバフ:Def
            Debuff_MAtk,                // デバフ:MAtk
            Debuff_MDef,                // デバフ:MDef
            Debuff_Wait,                // デバフ:Wait

            // バフ解除
            BuffCancel_Base = 400,      // 計算に使用する
            BuffCancel_Atk,             // バフ解除:ATK
            BuffCancel_Def,             // バフ解除:DEF
            BuffCancel_MAtk,            // バフ解除:MATK
            BuffCancel_MDef,            // バフ解除:MDEF
            BuffCancel_Wait,            // バフ解除:Wait
            BuffCancel_All = BuffCancel_Base, // バフ解除:すべて

            // バフ解除
            DebuffCancel_Base = 500,      // 計算に使用する
            DebuffCancel_Atk,             // デバフ解除:ATK
            DebuffCancel_Def,             // デバフ解除:DEF
            DebuffCancel_MAtk,            // デバフ解除:MATK
            DebuffCancel_MDef,            // デバフ解除:MDEF
            DebuffCancel_Wait,            // デバフ解除:Wait
            DebuffCancel_All = DebuffCancel_Base, // デバフ解除:すべて

        }

        /// <summary>
        /// 対象範囲
        /// </summary>
        public enum Target
        {
            Self,       // 自分
            Friend,     // 味方
            Enemy,      // 敵
            Accession,  // 継承：前の効果設定に依存する
        }

        /// <summary>
        /// スキル種類
        /// </summary>
        public enum SkillType
        {
            Recovery,       // 回復
            Buff,           // バフ
            Debuff,         // デバフ
            BuffCancel,     // バフ解除
            DebuffCancel,   // デバフ解除
            Damage,         // ダメージ
        }
    }
}

using CsvHelper.Configuration;
using System;

namespace Alice.Entities
{
    /// <summary>
    /// スキル効果x1
    /// </summary>
    [Serializable]
    public class Effect
    {
        public static readonly Effect Empty = new Effect { ID = "", Type = BattleConst.Effect.Damage, FX= "火5" };

        public string ID;
        public BattleConst.Effect Type;     // 効果
        public BattleConst.Target Target;   // 対象範囲
        public int Count;                   // 対象者数
        public int Value;                   // パラメータ(効果値)
        public string FX;                   // エフェクトファイル名

        public string Desc()
        {
            switch(Type)
            {
                // 即座効果
                case BattleConst.Effect.Damage: return "ダメージ";
                case BattleConst.Effect.DamageRatio: return "割合ダメージ";
                case BattleConst.Effect.Recovery: return "回復";
                case BattleConst.Effect.RecoveryRatio: return "割合回復";
                // バフ
                case BattleConst.Effect.Buff_Atk:return "物理攻撃アップ";
                case BattleConst.Effect.Buff_Def:return "物理防御アップ";
                case BattleConst.Effect.Buff_MAtk:return "魔法攻撃アップ";
                case BattleConst.Effect.Buff_MDef:return "魔法防御アップ";
                case BattleConst.Effect.Buff_Wait:return "素早さアップ";

                // デバフ
                case BattleConst.Effect.Debuff_Atk: return "物理攻撃ダウン";
                case BattleConst.Effect.Debuff_Def:return "物理防御ダウン";
                case BattleConst.Effect.Debuff_MAtk:return "魔法攻撃ダウン";
                case BattleConst.Effect.Debuff_MDef:return "魔法防御ダウン";
                case BattleConst.Effect.Debuff_Wait:return "素早さダウン";

                // バフ解除
                case BattleConst.Effect.BuffCancel_Atk: return "物理攻撃アップ効果を解除";
                case BattleConst.Effect.BuffCancel_Def:return "物理防御アップ効果を解除";
                case BattleConst.Effect.BuffCancel_MAtk:return "魔法攻撃アップ効果を解除";
                case BattleConst.Effect.BuffCancel_MDef:return "魔法防御アップ効果を解除";
                case BattleConst.Effect.BuffCancel_Wait:return "素早さアップ効果を解除";
                case BattleConst.Effect.BuffCancel_All:return "すべてアップ効果を解除";

                // デバフ解除
                case BattleConst.Effect.DebuffCancel_Atk: return "物理攻撃ダウン効果を解除";
                case BattleConst.Effect.DebuffCancel_Def:return "物理防御ダウン効果を解除";
                case BattleConst.Effect.DebuffCancel_MAtk:return "魔法攻撃ダウン効果を解除";
                case BattleConst.Effect.DebuffCancel_MDef:return "魔法防御ダウン効果を解除";
                case BattleConst.Effect.DebuffCancel_Wait:return "素早さダウン効果を解除";
                case BattleConst.Effect.DebuffCancel_All:return "すべてダウン効果を解除";

                default:return "";
            }
        }
    }

    public sealed class EffectMap :ClassMap<Effect>
    {
        public EffectMap()
        {
            Map(x => x.ID).Name("ID");
            Map(x => x.Type).Name("効果");
            Map(x => x.Target).Name("対象範囲");
            Map(x => x.Count).Name("対象数");
            Map(x => x.Value).Name("パラメータ");
            Map(x => x.FX).Name("FX");
        }
    }
}

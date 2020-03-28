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
            return Type.ToString().TextData();
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

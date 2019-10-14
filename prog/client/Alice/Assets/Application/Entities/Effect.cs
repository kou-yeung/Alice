using CsvHelper.Configuration;

namespace Alice.Entities
{
    /// <summary>
    /// スキル効果x1
    /// </summary>
    public class Effect
    {
        public string ID;
        public BattleConst.Effect Type;     // 効果
        public BattleConst.Target Target;   // 対象範囲
        public int Count;                   // 対象者数
        public int Value;                   // パラメータ(効果値)
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
        }
    }
}

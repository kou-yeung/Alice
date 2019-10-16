using CsvHelper.Configuration;

namespace Alice.Entities
{
    public class Character
    {
        public string ID;
        public string Name;
        public string Image;
        public BattleConst.Personality Personality;
        public int Atk;
        public int Def;
        public int MAtk;
        public int MDef;
        public int Wait;

    }

    public sealed class CharacterMap : ClassMap<Character>
    {
        public CharacterMap()
        {
            Map(x => x.ID).Name("ID");
            Map(x => x.Name).Name("名前");
            Map(x => x.Image).Name("画像ID");
            Map(x => x.Personality).ConvertUsing(row =>
            {
                switch(row.GetField<string>("性格"))
                {
                    case "乱暴": return BattleConst.Personality.Violent;
                    case "知的": return BattleConst.Personality.Intellectual;
                    case "仲間思い": return BattleConst.Personality.Considerate;
                    default:throw new System.Exception("未知の性格ですね。");
                }
            });
            Map(x => x.Atk).Name("ATK");
            Map(x => x.Def).Name("DEF");
            Map(x => x.MAtk).Name("MATK");
            Map(x => x.MDef).Name("MDEF");
            Map(x => x.Wait).Name("WAIT");
        }
    }
}

using CsvHelper.Configuration;
using System.Collections.Generic;

namespace Alice.Entities
{
    public class Character
    {
        public string ID;
        public string Name;
        public string Image;
        public string Personality;
        public int HP;
        public int Atk;
        public int Def;
        public int MAtk;
        public int MDef;
        public int Wait;
        public int[] Trigger;   // 発動確率
    }

    public sealed class CharacterMap : ClassMap<Character>
    {
        static readonly string[] triggerFields = new[]
        {
            "回復","バフ","デバフ","バフ解除","デバフ解除","ダメージ",
        };

        public CharacterMap()
        {
            Map(x => x.ID).Name("ID");
            Map(x => x.Name).Name("名前");
            Map(x => x.Image).Name("画像ID");
            Map(x => x.Personality).Name("性格");
            Map(x => x.HP).Name("HP");
            Map(x => x.Atk).Name("ATK");
            Map(x => x.Def).Name("DEF");
            Map(x => x.MAtk).Name("MATK");
            Map(x => x.MDef).Name("MDEF");
            Map(x => x.Wait).Name("WAIT");
            Map(x => x.Trigger).ConvertUsing(row =>
            {
                var trigger = new int[triggerFields.Length];
                for (int i = 0; i < trigger.Length; i++)
                {
                    trigger[i] = row.GetField<int>(triggerFields[i]);
                }
                return trigger;
            });
        }
    }
}

using CsvHelper.Configuration;
using System.Collections.Generic;
using System;

namespace Alice.Entities
{
    [Serializable]
    public class Character
    {
        [Serializable]
        public struct Param
        {
            public int HP;
            public int Atk;
            public int Def;
            public int MAtk;
            public int MDef;
        }

        public string ID;
        public string Name;
        public int Rare;
        public string Image;
        public string Personality;
        public Param Base;
        public int Wait;
        public Param Grow;
        public int[] Trigger;   // 発動確率


        /// <summary>
        /// 指定のレベルのパラメータを取得する
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        public Param ParamAtLevel(int level)
        {
            Param res;
            res.HP = Base.HP + Grow.HP * level;
            res.Atk = Base.Atk + Grow.Atk * level;
            res.Def = Base.Def + Grow.Def * level;
            res.MAtk = Base.MAtk + Grow.MAtk * level;
            res.MDef = Base.MDef + Grow.MDef * level;
            return res;
        }
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
            Map(x => x.Rare).Name("レア");
            Map(x => x.Image).Name("画像ID");
            Map(x => x.Personality).Name("性格");
            Map(x => x.Base.HP).Name("HP");
            Map(x => x.Base.Atk).Name("ATK");
            Map(x => x.Base.Def).Name("DEF");
            Map(x => x.Base.MAtk).Name("MATK");
            Map(x => x.Base.MDef).Name("MDEF");
            Map(x => x.Wait).Name("WAIT");
            Map(x => x.Grow.HP).Name("成長:HP");
            Map(x => x.Grow.Atk).Name("成長:ATK");
            Map(x => x.Grow.Def).Name("成長:DEF");
            Map(x => x.Grow.MAtk).Name("成長:MATK");
            Map(x => x.Grow.MDef).Name("成長:MDEF");
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

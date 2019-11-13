using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CsvHelper.Configuration;
using System.Linq;
using System;

namespace Alice.Entities
{
    [Serializable]
    public class Skill
    {
        public string ID;
        public string Name;
        public int Rare;
        public string Image;
        public bool Passive;    // パッシブスキルかどうか
        public int CoolTime;
        public int Remain;      // 持続回数
        public BattleConst.Attribute Attribute;
        public string[] Effects;

        [NonSerialized]
        public List<Effect> _EffectsRef;
        public List<Effect> EffectsRef
        {
            get
            {
                if(_EffectsRef == null)
                {
                    _EffectsRef = new List<Effect>();
                    foreach(var effect in Effects)
                    {
                        _EffectsRef.Add(MasterData.Instance.effects.First(v => v.ID == effect));
                    }
                }
                return _EffectsRef;
            }
        }

        /// <summary>
        /// 指定の効果に含むかどうかチェック
        /// </summary>
        /// <param name="effects"></param>
        /// <returns></returns>
        public bool HasEffect(params BattleConst.Effect[] effects)
        {
            return EffectsRef.Where(v => effects.Contains(v.Type)).Count() != 0;
        }
    }

    public sealed class SkillMap : ClassMap<Skill>
    {
        public SkillMap()
        {
            Map(x => x.ID).Name("ID");
            Map(x => x.Name).Name("名前");
            Map(x => x.Rare).Name("レア");
            Map(x => x.Image).Name("画像ID");
            Map(x => x.Passive).ConvertUsing(row => row.GetField<string>("種類") == "パッシブ");
            Map(x => x.CoolTime).Name("CT");
            Map(x => x.Remain).Name("持続回数");
            Map(x => x.Attribute).ConvertUsing(row =>
            {
                switch(row.GetField<string>("属性"))
                {
                    default:
                    case "物理": return BattleConst.Attribute.Physics;
                    case "魔法": return BattleConst.Attribute.Magic;
                }
            });
            Map(x => x.Effects).ConvertUsing(row =>
            {
                List<string> result = new List<string>();
                result.Add(row.GetField<string>("効果１"));
                result.Add(row.GetField<string>("効果２"));
                result.Add(row.GetField<string>("効果３"));
                result.Add(row.GetField<string>("効果４"));
                return result.Where(v => !string.IsNullOrWhiteSpace(v)).ToArray();
            });
        }
    }
}


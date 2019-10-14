using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CsvHelper.Configuration;
using System.Linq;

namespace Alice.Entities
{
    public class Skill
    {
        public string ID;
        public string Name;
        public string Image;
        public string[] Effects;

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
                        _EffectsRef.Add(MasterData.effects.First(v => v.ID == effect));
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
            Map(x => x.Image).Name("画像ID");
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


using CsvHelper.Configuration;
using System.Collections.Generic;
using System.Linq;
using System;

namespace Alice.Entities
{
    /// <summary>
    /// 性格:AIのチェック順に影響します
    /// </summary>
    [Serializable]
    public class Personality
    {
        public string Name;     // 性格名
        public string[] AI;     // 思考
    }

    public sealed class PersonalityMap : ClassMap<Personality>
    {
        // CSVパース用フィールド名
        static readonly string[] aiFields = new[]
        {
            "AI1","AI2","AI3","AI4","AI5","AI6"
        };

        public PersonalityMap()
        {
            Map(x => x.Name).Name("性格");
            Map(x => x.AI).ConvertUsing(row =>
            {
                List<string> ai = new List<string>();
                foreach (var name in aiFields)
                {
                    var field = row.GetField<string>(name);
                    if(!string.IsNullOrEmpty(field))
                    {
                        ai.Add(field);
                    }
                }
                return ai.ToArray();
            });
        }
    }
}

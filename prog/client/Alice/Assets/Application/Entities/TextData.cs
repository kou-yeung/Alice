using System;
using System.Collections.Generic;
using CsvHelper.Configuration;
using UnityEngine;

namespace Alice.Entities
{
    [Serializable]
    public class TextData
    {
        public enum Language
        {
            Japanese,
            English,
            ChineseTraditional,
            LanguageMax,
        }
        public string ID;
        public string[] Text;

        static Language? currentLanguage;// = Language.English;

        public static Language CurrentLanguage
        {
            get
            {
                if (!currentLanguage.HasValue)
                {
                    switch (Application.systemLanguage)
                    {
                        // 日本語
                        case SystemLanguage.Japanese:
                            currentLanguage = Language.Japanese;
                            break;
                        // 中国語圏
                        case SystemLanguage.Chinese:
                        case SystemLanguage.ChineseSimplified:
                        case SystemLanguage.ChineseTraditional:
                            currentLanguage = Language.ChineseTraditional;
                            break;
                        // その他
                        default:
                            currentLanguage = Language.English;
                            break;
                    }
                }
                return currentLanguage.Value;
            }
        }
    }

    public sealed class TextDataMap : ClassMap<TextData>
    {
        public TextDataMap()
        {
            Map(x => x.ID).Name("ID");
            Map(x => x.Text).ConvertUsing(row =>
            {
                List<string> res = new List<string>();
                res.Add(Parse(row.GetField<string>("日本語")));
                res.Add(Parse(row.GetField<string>("英語")));
                res.Add(Parse(row.GetField<string>("中国語")));
                return res.ToArray();
            });
        }

        private string Parse(string text)
        {
            text = text.Replace("<br>", "\n");
            return text;
        }
    }
}

// 拡張メソッド
public static class TextDataEx
{
    public static string TextData(this string @this)
    {
        return Alice.Entities.MasterData.Instance.FindTextDataByID(@this);
    }
}
using System;
using CsvHelper.Configuration;

namespace Alice.Entities
{
    [Serializable]
    public class TextData
    {
        public string ID;
        public string Text;
    }

    public sealed class TextDataMap : ClassMap<TextData>
    {
        public TextDataMap()
        {
            Map(x => x.ID).Name("ID");
            Map(x => x.Text).ConvertUsing(row =>
            {
                return Parse(row.GetField<string>("テキスト"));
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
        return Alice.Entities.MasterData.Instance.FindTextDataByID(@this)?.Text;
    }
}
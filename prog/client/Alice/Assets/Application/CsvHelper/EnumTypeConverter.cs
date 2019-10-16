using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using System;
using Zoo.Util;

namespace CsvHelper
{
    public class EnumTypeConverter<T> : ITypeConverter where T : struct, IConvertible
    {
        public object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
        {
            return EnumExtension<T>.Parse(text);
        }

        public string ConvertToString(object value, IWriterRow row, MemberMapData memberMapData)
        {
            return EnumExtension<T>.ToString((T)value);
        }
    }
}

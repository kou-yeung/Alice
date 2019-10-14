using System;
using System.Linq;

namespace Zoo.Util
{
    public static class EnumExtension<T> where T : struct, IConvertible
    {
        static string[] Keys = Enum.GetNames(typeof(T));
        static T[] Values = Enum.GetValues(typeof(T)).Cast<T>().ToArray();

        /// <summary>
        /// Enum => string
        /// </summary>
        public static string ToString(T e)
        {
            for (int i = 0; i < Values.Length; i++)
            {
                if (Values[i].Equals(e)) return Keys[i];
            }
            return null;
        }

        /// <summary>
        /// string => Enum
        /// </summary>
        public static T Parse(string text)
        {
            for (int i = 0; i < Keys.Length; i++)
            {
                if (Keys[i].Equals(text)) return Values[i];
            }
            return default(T);
        }

        /// <summary>
        /// 走査
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="action"></param>
        public static void ForEach(Action<T> action)
        {
            for (int i = 0; i < Values.Length; i++)
            {
                action(Values[i]);
            }
        }
    }
}

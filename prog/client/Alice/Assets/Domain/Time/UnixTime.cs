///=======================
/// UnixTime のヘルパー
/// 参考 : https://gist.github.com/YuukiTsuchida/06ca3a1f0baf755651b0#file-unixtime
///=======================

using System;

namespace Zoo.Time
{
    public static class UnixTime
    {
        private static readonly DateTime UNIX_EPOCH = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        /*===========================================================================*/
        /**
         * 現在時刻からUnixTimeを計算する.
         *
         * @return UnixTime.
         */
        public static long Now()
        {
            return (FromDateTime(DateTime.UtcNow));
        }

        /*===========================================================================*/
        /**
         * UnixTimeからDateTimeに変換.
         *
         * @param [in] unixTime 変換したいUnixTime.
         * @return 引数時間のDateTime.
         */
        public static DateTime FromUnixTime(long unixTime)
        {
            return UNIX_EPOCH.AddSeconds(unixTime).ToLocalTime();
        }
        public static DateTime FromUnixTime(string unixTime)
        {
            return FromUnixTime(long.Parse(unixTime));
        }

        /*===========================================================================*/
        /**
         * 指定時間をUnixTimeに変換する.
         *
         * @param [in] dateTime DateTimeオブジェクト.
         * @return UnixTime.
         */
        public static long FromDateTime(DateTime dateTime)
        {
            double nowTicks = (dateTime.ToUniversalTime() - UNIX_EPOCH).TotalSeconds;
            return (long)nowTicks;
        }
    }
}

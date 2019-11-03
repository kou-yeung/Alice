///=============================
/// サーバ時間
///=============================

namespace Zoo.Time
{
    public static class ServerTime
    {
        static long unixTime = 0;
        static float realtimeSinceStartup;
        /// <summary>
        /// 初期化
        /// 受信した UnixTime と 保持時のデバイス時間を保持する
        /// </summary>
        /// <param name="unixTime"></param>
        public static void Init(long unixTime)
        {
            ServerTime.unixTime = unixTime;
            ServerTime.realtimeSinceStartup = UnityEngine.Time.realtimeSinceStartup;
        }

        /// <summary>
        /// ローカルで疑似的計算した UnixTime
        /// </summary>
        public static long CurrentUnixTime
        {
            get
            {
                var diff = UnityEngine.Time.realtimeSinceStartup - realtimeSinceStartup;
                return unixTime + (int)diff;
            }
        }
    }
}

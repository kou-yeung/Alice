using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Alice
{
    public static class UserData
    {
        /// <summary>
        /// ホーム情報をキャッシュする
        /// </summary>
        public static HomeRecv cacheHomeRecv { get; private set; }
        public static void CacheHomeRecv(HomeRecv homeRecv)
        {
            cacheHomeRecv = homeRecv;
        }
    }
}

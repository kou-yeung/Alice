﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Alice
{
    public static class Const
    {
        public static int PLAYER_RANK_MAX = 10;
        public static int AdsRewardTimeSecond = (10 * 60);     // 10分
        public static int AlarmTimeSecond = (15 * 60);         // 15分


        /// <summary>
        /// OS種類
        /// </summary>
        public enum Platform
        {
            Unknown,
            UnityEditor,        // UnitEditor
            iOS,                // iOS
            Android,            // Android
        }
    }
}

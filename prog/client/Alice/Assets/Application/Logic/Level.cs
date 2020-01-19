using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Alice.Logic
{
    public static class LevelTable
    {
        public static int Exp2Level(int exp)
        {
            return Mathf.FloorToInt(Mathf.Sqrt(exp)) + 1;
        }

        public static int LevelExp(int level)
        {
            return level * level;
        }
    }
}

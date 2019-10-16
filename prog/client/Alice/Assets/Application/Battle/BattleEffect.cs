using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Alice
{
    public class BattleEffect
    {
        public BattleUnit target;           // 効果対象
        public BattleConst.Effect type;     // 効果種類
        public int value;                   // 効果値

        public BattleEffect(BattleUnit target, BattleConst.Effect type, int value)
        {
            this.target = target;
            this.type = type;
            this.value = value;
        }
    }
}

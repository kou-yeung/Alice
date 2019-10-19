﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Alice.Entities;

namespace Alice
{
    public class BattleEffect
    {
        public BattleUnit target;           // 効果対象
        public BattleConst.Effect type { get { return effect.Type; } } // 効果種類
        public string FX { get { return effect.FX; } } // FXファイル名
        public int value;                       // 効果値

        Effect effect;

        public BattleEffect(BattleUnit target, Effect effect, int value)
        {
            this.target = target;
            this.effect = effect;
            this.value = value;
        }
    }
}

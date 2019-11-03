using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Alice.Entities;

namespace Alice
{
    /// <summary>
    /// バトルの行動
    /// </summary>
    public class BattleAction
    {
        public BattleUnit behaviour { get; set; }
        public Skill skill { get; set; }
        public List<BattleEffect> effects { get; set; } = new List<BattleEffect>();

        public BattleAction(BattleUnit behaviour, Skill skill)
        {
            this.behaviour = behaviour;
            this.skill = skill;
        }
    }
}


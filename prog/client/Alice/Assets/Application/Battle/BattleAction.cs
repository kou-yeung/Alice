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
        public BattleUnit behavioure { get; set; }
        public Skill skill { get; set; }
        public List<BattleEffect> effects { get; set; } = new List<BattleEffect>();

        public BattleAction(BattleUnit behavioure, Skill skill)
        {
            this.behavioure = behavioure;
            this.skill = skill;
        }
    }
}


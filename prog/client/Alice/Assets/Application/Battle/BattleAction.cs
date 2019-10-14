using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Alice
{
    /// <summary>
    /// バトルの行動
    /// </summary>
    public class BattleAction
    {
        public BattleUnit behavioure { get; set; }
        public string skill { get; set; }
        public List<BattleEffect> effects { get; set; } = new List<BattleEffect>();

        public BattleAction(BattleUnit behavioure, string skill)
        {
            this.behavioure = behavioure;
            this.skill = skill;
        }
    }
}


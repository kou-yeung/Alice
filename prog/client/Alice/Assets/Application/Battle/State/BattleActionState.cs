using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zoo.StateMachine;

namespace Alice
{
    public class BattleActionState : IState<Battle>
    {
        public override void Begin(Battle owner)
        {
            Debug.Log("BattleActionState : Begin");
            owner.EnableAction(true);
        }

        public override void End(Battle owner)
        {
            owner.EnableAction(false);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zoo.StateMachine;

namespace Alice
{
    public class BattleStartState : IState<Battle>
    {
        public override void Begin(Battle owner)
        {
            Debug.Log("BattleStartState : Begin");
            owner.controller.ChangeState(BattleConst.State.Timeline);
        }
    }
}

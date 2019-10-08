using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zoo.StateMachine;

namespace Alice
{
    public class BattleTimelineState : IState<Battle>
    {
        public override void Begin(Battle owner)
        {
            Debug.Log("BattleTimelineState : Begin");
            owner.controller.ChangeState(BattleConst.State.Action);
        }
    }
}

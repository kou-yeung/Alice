using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Zoo.StateMachine;

namespace Alice
{
    public class BattleInitState : IState<Battle>
    {
        public override void Begin(Battle owner)
        {
            owner.controller.ChangeState(BattleConst.State.Start);
        }
    }
}

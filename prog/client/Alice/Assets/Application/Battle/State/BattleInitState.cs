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
            // バトル開始時はスリープしないように
            Screen.sleepTimeout = SleepTimeout.NeverSleep;

            owner.controller.ChangeState(BattleConst.State.Passive);
        }
    }
}

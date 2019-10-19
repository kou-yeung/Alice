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
            owner.controller.phase.Change("Battle Start", () =>
            {
                owner.controller.ChangeState(BattleConst.State.Timeline);
            });
        }
    }
}

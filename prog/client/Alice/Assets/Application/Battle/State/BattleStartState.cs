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
                owner.timeline.root.gameObject.SetActive(true);
                owner.controller.ChangeState(BattleConst.State.Timeline);
            });
        }
    }
}

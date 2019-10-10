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
            Debug.Log("BattleInitState : Begin");
            var unit = owner.controller.CreateUnit(Guid.NewGuid().ToString());
            unit.gameObject.transform.SetParent(owner.transform, false);
            owner.controller.ChangeState(BattleConst.State.Start);
        }
    }
}

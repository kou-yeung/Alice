using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zoo.StateMachine;
using System.Linq;

namespace Alice
{
    public class BattleTimelineState : IState<Battle>
    {
        public override void Begin(Battle owner)
        {
            var units = owner.controller.units;
            var sortedUnits = units.Keys.OrderBy(uniq => units[uniq].current.Wait).ToArray();
            owner.controller.UpdateSortedUniqs(sortedUnits);
            owner.controller.ChangeState(BattleConst.State.Action);
        }
    }
}

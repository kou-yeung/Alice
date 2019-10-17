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
            var units = owner.controller.units.Values.ToList();
            units.Sort((a, b) =>
            {
                return a.current.Wait.CompareTo(b.current.Wait);
            });

            // 時間を進む:先頭UnitのWait時間分を減らす
            var wait = units[0].current.Wait;
            foreach (var unit in units)
            {
                unit.current.Wait -= wait;
            }
            owner.controller.UpdateSortedBattleUnits(units);
            owner.controller.ChangeState(BattleConst.State.Action);
        }
    }
}

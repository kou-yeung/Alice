using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zoo.StateMachine;
using System.Linq;

namespace Alice
{
    public class BattleFinallyState : IState<Battle>
    {
        public override void Begin(Battle owner)
        {
            // 必要なくなったものの後始末
            foreach(var unit in owner.controller.units)
            {
                unit.Value.Destory();
            }
            owner.controller.units.Clear();

            // 次のバトルを備えてタイムライン非表示
            owner.timeline.root.gameObject.SetActive(false);
            // バトルを非表示する
            owner.gameObject.SetActive(false);
            // フェーズを破棄
            owner.controller.phase.Destory();
        }
    }
}

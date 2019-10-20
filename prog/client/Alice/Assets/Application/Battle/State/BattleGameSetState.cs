using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zoo.StateMachine;
using System.Linq;

namespace Alice
{
    /// <summary>
    /// 試合終了
    /// </summary>
    public class BattleGameSetState : IState<Battle>
    {
        public override void Begin(Battle owner)
        {
            // タイムラインアイコンを回収する
            foreach(var kv in owner.controller.timeline)
            {
                kv.Value.Destroy();
            }
        }
    }
}

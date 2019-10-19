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
            //// Wait加算
            //var behaviour = owner.controller.currentActionBattleUnit;
            //behaviour.current.Wait = behaviour.characterData.Wait;

            //// タイム更新
            //owner.controller.ChangeState(BattleConst.State.Timeline);
        }
    }
}

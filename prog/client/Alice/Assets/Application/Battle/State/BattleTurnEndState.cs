using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zoo.StateMachine;
using System.Linq;

namespace Alice
{
    public class BattleTurnEndState : IState<Battle>
    {
        public override void Begin(Battle owner)
        {
            // 試合終了判定
            var playerCount = owner.controller.units.Count(kv => kv.Value.side == BattleConst.Side.Player);
            var enemyCount = owner.controller.units.Count(kv => kv.Value.side == BattleConst.Side.Enemy);

            if(playerCount <= 0 && enemyCount <= 0)
            {
                // 引き分け:ダメージ反射による全滅の可能性を考える
                owner.controller.ChangeState(BattleConst.State.GameSet);
            }
            else if(playerCount <= 0)
            {
                // 味方数 0 なら負け
                owner.controller.ChangeState(BattleConst.State.GameSet);
            }
            else if(enemyCount <= 0)
            {
                // 相手数 0 なら勝ち
                owner.controller.ChangeState(BattleConst.State.GameSet);
            }
            else
            {
                // Wait加算
                var behaviour = owner.controller.currentActionBattleUnit;
                behaviour.current.Wait = behaviour.characterData.Wait;
                // タイム更新
                owner.controller.ChangeState(BattleConst.State.Timeline);
            }
        }
    }
}

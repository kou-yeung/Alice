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
            // 行動後始末
            owner.controller.currentAction.behaviour.PostAction();

            // 試合終了判定
            var playerCount = owner.controller.units.Count(kv => kv.Value.side == BattleConst.Side.Player);
            var enemyCount = owner.controller.units.Count(kv => kv.Value.side == BattleConst.Side.Enemy);

            if(playerCount <= 0 && enemyCount <= 0)
            {
                owner.SetBattleResult(BattleConst.Result.Draw);
                // 引き分け:ダメージ反射による全滅の可能性を考える
                owner.controller.phase.Change("引き分け", () =>
                {
                    owner.controller.ChangeState(BattleConst.State.GameSet);
                });
            }
            else if(playerCount <= 0)
            {
                // 味方数 0 なら負け
                owner.SetBattleResult(BattleConst.Result.Lose);
                owner.controller.phase.Change("敗北", () =>
                {
                    owner.controller.ChangeState(BattleConst.State.GameSet);
                });
            }
            else if(enemyCount <= 0)
            {
                // 相手数 0 なら勝ち
                owner.SetBattleResult(BattleConst.Result.Win);
                owner.controller.phase.Change("勝利", () =>
                {
                    owner.controller.ChangeState(BattleConst.State.GameSet);
                });
            }
            else
            {
                // タイム更新
                owner.controller.ChangeState(BattleConst.State.Timeline);
            }
        }
    }
}

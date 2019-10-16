using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zoo.StateMachine;

namespace Alice
{
    public class BattleActionState : IState<Battle>
    {
        public override void Begin(Battle owner)
        {
            var uniq = owner.controller.currentActionUniq;
            var behaviour = owner.controller.units[uniq];
            // 行動選択
            var skill = BattleAI.Instance.Exec(owner.controller.units[uniq]);
            // 行動による効果計算
            var action = BattleLogic.Instance.Exec(new BattleAction(behaviour, skill));
            // 行動保持する
            owner.controller.CurrentAction(action);
            // 行動再生
            owner.controller.ChangeState(BattleConst.State.Playback);
        }
    }
}

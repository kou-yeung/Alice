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
            var behaviour = owner.controller.currentActionBattleUnit;
            // 行動前の準備
            behaviour.PreAction();
            // 行動選択
            var skill = BattleAI.Instance.Exec(behaviour);
            // 行動による効果計算
            var action = BattleLogic.Instance.Exec(new BattleAction(behaviour, skill));
            // 行動保持する
            owner.controller.CurrentAction(action);
            // スキルを使用したため登録
            behaviour.UseSkill(skill);
            // 行動再生
            BattlePlayback.Play(action, () =>
            {
                owner.controller.ChangeState(BattleConst.State.TurnEnd);
            });
        }
    }
}

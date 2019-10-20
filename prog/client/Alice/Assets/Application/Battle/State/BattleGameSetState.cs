using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zoo.StateMachine;
using System.Linq;
using UniRx;
using System;

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
            owner.controller.timeline.Clear();

            // MEMO : 将来はシェアなどの機能を追加すると思いますが、今は３秒待ったら終了する
            Observable.Timer(TimeSpan.FromSeconds(3)).Subscribe(_ => { },
                () =>
                {
                    owner.controller.ChangeState(BattleConst.State.Finally);
                });
        }
    }
}

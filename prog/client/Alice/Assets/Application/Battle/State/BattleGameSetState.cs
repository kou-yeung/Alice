using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zoo.StateMachine;
using System.Linq;
using UniRx;
using System;
using Zoo.Communication;

namespace Alice
{
    /// <summary>
    /// 試合終了
    /// </summary>
    public class BattleGameSetState : IState<Battle>
    {
        public override void Begin(Battle owner)
        {
            // 「記録から再生」と「シャドウバトル」はGameSetを実行する必要ありません
            if (owner.fromRecord || owner.recv.type == BattleConst.BattleType.Shadow)
            {
                OnNext(owner);
            }
            else
            {
                var send = new GameSetSend();
                send.result = owner.recv.result;
                CommunicationService.Instance.Request("GameSet", JsonUtility.ToJson(send), (recv) =>
                {
                    var data = JsonUtility.FromJson<GameSetRecv>(recv);
                    UserData.Modify(data.modified);
                    OnNext(owner);
                });
            }
        }

        void OnNext(Battle owner)
        {
            //Observable.Timer(TimeSpan.FromSeconds(3)).Subscribe(_ => { },
            //() =>
            //{
                owner.controller.ChangeState(BattleConst.State.Finally);
            //});
        }
    }
}

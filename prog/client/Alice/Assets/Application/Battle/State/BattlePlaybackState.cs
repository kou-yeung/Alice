using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zoo.StateMachine;
using UniRx;

namespace Alice
{
    public class BattlePlaybackState : IState<Battle>
    {
        public override void Begin(Battle owner)
        {
            Debug.Log("BattlePlaybackState : Begin");

            Observable.FromCoroutine<Unit>(o => Playback(o)).Subscribe((_)=>
            {
                Debug.Log("Playback Wait!!");
            },
            ()=>
            {
                owner.controller.ChangeState(BattleConst.State.Timeline);
            });
        }

        IEnumerator Playback(IObserver<Unit> observer)
        {
            for (int i = 0; i < 3; i++)
            {
                observer.OnNext(Unit.Default);
                yield return new WaitForSeconds(1);
            }
            observer.OnCompleted();
        }
    }
}

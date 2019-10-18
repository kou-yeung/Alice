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

            foreach(var effect in owner.controller.currentAction.effects)
            {
                Debug.Log($"Action : {effect.target.uniq} ({effect.target.side}) : {effect.type} : {effect.value}");
            }

            Observable.FromCoroutine<Unit>(o => Playback(o)).Subscribe((_)=>
            {
                Debug.Log("Playback Wait!!");
            },
            ()=>
            {
                owner.controller.ChangeState(BattleConst.State.TurnEnd);
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

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zoo.StateMachine;
using UniRx;
using Zoo;
using System.Linq;

namespace Alice
{
    public class BattlePlaybackState : IState<Battle>
    {
        public override void Begin(Battle owner)
        {
            Debug.Log("BattlePlaybackState : Begin");

            var action = owner.controller.currentAction;

            Async.Waterflow(() =>
            {
                // 待機モーションに設定
                foreach(var kv in owner.controller.units)
                {
                    kv.Value.actor.setAnimation("Idle");
                }
                // 演出終了
                owner.controller.ChangeState(BattleConst.State.TurnEnd);
            },
            (end) => Attack(action, end),   // 演出:アクション開始
            (end) => Damage(action, end),   // 演出:ダメージ
            (end) => Recovery(action, end)  // 演出:回復
            );

            //foreach(var effect in owner.controller.currentAction.effects)
            //{
            //    Debug.Log($"Action : {effect.target.uniq} ({effect.target.side}) : {effect.type} : {effect.value}");
            //}

            //Observable.FromCoroutine<Unit>(o => Playback(o)).Subscribe((_)=>
            //{
            //    Debug.Log("Playback Wait!!");
            //},
            //()=>
            //{
            //    owner.controller.ChangeState(BattleConst.State.TurnEnd);
            //});
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

        /// <summary>
        /// 演出:アクション開始
        /// </summary>
        /// <param name="action"></param>
        /// <param name="cb"></param>
        void Attack(BattleAction action, Action cb)
        {
            action.behavioure.actor.setAnimation("Attack", cb);
        }

        /// <summary>
        /// 演出:ダメージ
        /// </summary>
        /// <param name="action"></param>
        /// <param name="cb"></param>
        void Damage(BattleAction action, Action cb)
        {
            var types = new[] { BattleConst.Effect.Damage, BattleConst.Effect.DamageRatio };
            var effects = action.effects.Where(v => types.Contains(v.type)).ToArray();
            if(effects.Length > 0)
            {
                var function = Async.Passive(cb, effects.Length);
                foreach (var effect in effects)
                {
                    effect.target.actor.setAnimation("Hit", function);
                }
            } else
            {
                cb();
            }
        }

        /// <summary>
        /// 演出:回復
        /// </summary>
        /// <param name="action"></param>
        /// <param name="cb"></param>
        void Recovery(BattleAction action, Action cb)
        {
            var types = new[] { BattleConst.Effect.Recovery, BattleConst.Effect.RecoveryRatio };
            var effects = action.effects.Where(v => types.Contains(v.type)).ToArray();
            if (effects.Length > 0)
            {
                var function = Async.Passive(cb, effects.Length);
                foreach (var effect in effects)
                {
                    effect.target.actor.setAnimation("Recovery", function);
                }
            }
            else
            {
                cb();
            }
        }
    }
}

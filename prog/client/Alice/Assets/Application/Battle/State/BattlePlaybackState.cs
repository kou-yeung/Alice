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
            (end) => Cutin(action, end),    // 演出:スキルカットイン
            (end) => Attack(action, end),   // 演出:アクション開始
            (end) => Damage(action, end),   // 演出:ダメージ
            (end) => Recovery(action, end),  // 演出:回復
            (end) => Dead(action, end)      // 演出死亡
            );
        }


        /// <summary>
        /// 指定したエフェクトファイルの一覧を取得
        /// ※死亡したユニットは含まない
        /// </summary>
        /// <param name="effects"></param>
        /// <returns></returns>
        BattleEffect[] GetBattleEffectByTypes(List<BattleEffect> effects, BattleConst.Effect[] types)
        {
            return effects.Where(v => types.Contains(v.type) && !v.target.current.IsDead).ToArray();
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
            var effects = GetBattleEffectByTypes(action.effects, types);
            if(effects.Length > 0)
            {
                var function = Async.Passive(cb, effects.Length);
                foreach (var effect in effects)
                {
                    effect.target.actor.setAnimation("Hit", function);
                    effect.target.Damage(effect.value);
                    FX.Play(effect.FX, effect.target.root.transform);
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
            var effects = GetBattleEffectByTypes(action.effects, types);
            if (effects.Length > 0)
            {
                var function = Async.Passive(cb, effects.Length);
                foreach (var effect in effects)
                {
                    effect.target.actor.setAnimation("Recovery", function);
                    effect.target.Recovery(effect.value);
                    FX.Play(effect.FX, effect.target.actor.transform);
                }
            }
            else
            {
                cb();
            }
        }

        /// <summary>
        /// 演出:スキルカットイン
        /// </summary>
        /// <param name="action"></param>
        /// <param name="cb"></param>
        void Cutin(BattleAction action, Action cb)
        {
            if(action.skill != null)
            {
                var owner = Battle.Instance.controller;
                owner.phase.Change($"{action.skill.Name}", cb);
            }
            else
            {
                //Debug.Log($"通常攻撃");
                cb();
            }
        }

        /// <summary>
        /// 演出:死亡
        /// </summary>
        /// <param name="action"></param>
        /// <param name="cb"></param>
        void Dead(BattleAction action, Action cb)
        {
            var deads = action.effects.Where(v => v.target.current.IsDead).Select(v=>v.target).Distinct();
            foreach(var dead in deads)
            {
                Battle.Instance.controller.units.Remove(dead.uniq);
                dead.actor.setAnimation("Dead", () =>
                {
                    GameObject.Destroy(dead.root);
                });
            }
            cb();
        }
    }
}

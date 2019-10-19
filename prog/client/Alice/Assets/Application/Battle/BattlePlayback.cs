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
    public static class BattlePlayback
    {
        /// <summary>
        /// BattleActionを再生する
        /// </summary>
        /// <param name="action"></param>
        /// <param name="cb"></param>
        public static void Play(BattleAction action, Action cb)
        {
            Async.Waterflow(cb,
            (end) => Cutin(action, end),    // 演出:スキルカットイン
            (end) => Action(action, end),   // 演出:アクション開始
            (end) => Damage(action, end),   // 演出:ダメージ
            (end) => Recovery(action, end),  // 演出:回復
            // 演出:バフ
            (end) => Condition(action, BattleConst.Effect.Buff_Atk, end),      // 演出:バフ:ATK
            (end) => Condition(action, BattleConst.Effect.Buff_Def, end),      // 演出:バフ:DEF
            (end) => Condition(action, BattleConst.Effect.Buff_MAtk, end),      // 演出:バフ:MATK
            (end) => Condition(action, BattleConst.Effect.Buff_MDef, end),      // 演出:バフ:MDEF
            (end) => Condition(action, BattleConst.Effect.Buff_Wait, end),      // 演出:バフ:WAIT
            // 演出:デバフ
            (end) => Condition(action, BattleConst.Effect.Debuff_Atk, end),      // 演出:デバフ:ATK
            (end) => Condition(action, BattleConst.Effect.Debuff_Def, end),      // 演出:デバフ:DEF
            (end) => Condition(action, BattleConst.Effect.Debuff_MAtk, end),      // 演出:デバフ:MATK
            (end) => Condition(action, BattleConst.Effect.Debuff_MDef, end),      // 演出:デバフ:MDEF
            (end) => Condition(action, BattleConst.Effect.Debuff_Wait, end),      // 演出:デバフ:WAIT
            (end) => Dead(action, end)      // 演出死亡
            );
        }

        /// <summary>
        /// 指定したエフェクトファイルの一覧を取得
        /// ※死亡したユニットは含まない
        /// </summary>
        /// <param name="effects"></param>
        /// <returns></returns>
        static BattleEffect[] GetBattleEffectByTypes(List<BattleEffect> effects, BattleConst.Effect[] types)
        {
            return effects.Where(v => types.Contains(v.type) && !v.target.current.IsDead).ToArray();
        }

        /// <summary>
        /// 演出:アクション開始
        /// </summary>
        /// <param name="action"></param>
        /// <param name="cb"></param>
        static void Action(BattleAction action, Action cb)
        {
            action.behavioure.actor.setAnimation("Attack", cb);
        }

        /// <summary>
        /// 演出:ダメージ
        /// </summary>
        /// <param name="action"></param>
        /// <param name="cb"></param>
        static void Damage(BattleAction action, Action cb)
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
        static void Recovery(BattleAction action, Action cb)
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
        static void Cutin(BattleAction action, Action cb)
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
        static void Dead(BattleAction action, Action cb)
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

        /// <summary>
        /// 演出:バフ演出
        /// </summary>
        /// <param name="action"></param>
        /// <param name="cb"></param>
        static void Condition(BattleAction action, BattleConst.Effect type, Action cb)
        {
            var effects = GetBattleEffectByTypes(action.effects, new[] { type });
            if (effects.Length > 0)
            {
                var function = Async.Passive(cb, effects.Length);
                foreach (var effect in effects)
                {
                    effect.target.actor.setAnimation("Recovery", function);
                    effect.target.AddCondition(effect.type, effect.value, effect.remain);
                    FX.Play(effect.FX, effect.target.actor.transform);
                }
            }
            else
            {
                cb();
            }

        }
    }
}

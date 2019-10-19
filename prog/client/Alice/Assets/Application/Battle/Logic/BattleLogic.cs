using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Alice.Entities;
using System.Linq;
using System;
using UnityEngine.Assertions;

namespace Alice
{
    public class BattleLogic
    {
        public static BattleLogic Instance { get; private set; }
        static BattleLogic() { Instance = new BattleLogic(); }

        // ロジックの共通インタフェース定義
        delegate int Logic(BattleUnit behavioure, BattleUnit target, Effect effect);
        // ロジックカタログ
        Dictionary<string, Logic> logics = new Dictionary<string, Logic>();

        public BattleLogic()
        {
            Register("通常ダメージ", Damage);
            Register("割合ダメージ", DamageRatio);
            Register("通常回復", Recovery);
            Register("割合回復", RecoveryRatio);
            Register("補正値", Correction);
        }

        /// <summary>
        /// エフェクトの対象取得(人数は関係なく
        /// </summary>
        /// <param name="behavioure"></param>
        /// <param name="effect"></param>
        /// <returns></returns>
        List<BattleUnit> EffectTargets(BattleUnit behavioure, Effect effect)
        {
            BattleConst.Target target = (effect != null) ? effect.Target : BattleConst.Target.Enemy;

            List<BattleUnit> res = new List<BattleUnit>();
            var units = Battle.Instance.controller.units;
            switch (target)
            {
                case BattleConst.Target.Self:
                    res.Add(behavioure);
                    break;
                case BattleConst.Target.Friend:
                    res.AddRange(units.Values.Where(v => v.side == behavioure.side));
                    break;
                case BattleConst.Target.Enemy:
                    res.AddRange(units.Values.Where(v => v.side != behavioure.side));
                    break;
            }
            return res;
        }
        
        /// <summary>
        /// 対象をランダム抽選する
        /// </summary>
        /// <param name="units"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        List<BattleUnit> LotsTargets(List<BattleUnit> units, int count)
        {
            Assert.IsTrue(units != null, "効果対象が設定してください");
            Assert.IsTrue(units.Count != 0, "効果対象がない");
            Assert.IsTrue(count != 0, "効果対象数が0になっています");

            // 抽選最大数制限
            count = Mathf.Min(units.Count, count);
            List<BattleUnit> res = new List<BattleUnit>();
            List<BattleUnit> wrk = units.ToList();
            for (int i = 0; i < count; i++)
            {
                var index = Battle.Instance.random.Next(0, wrk.Count);
                res.Add(wrk[index]);
                wrk.RemoveAt(index);
            }
            return res;
        }

        /// <summary>
        /// ロジックに沿って効果を返す
        /// </summary>
        /// <returns></returns>
        public BattleAction Exec(BattleAction action)
        {
            List<BattleUnit> cacheTargets = null;

            if (action.skill != null)
            {
                foreach (var effect in action.skill.EffectsRef)
                {
                    if(effect.Target == BattleConst.Target.Accession)
                    {
                        // 継承の場合、継承から抽選する
                        cacheTargets = LotsTargets(cacheTargets, effect.Count);
                    }
                    else
                    {
                        // 効果の対象一覧取得
                        cacheTargets = EffectTargets(action.behavioure, effect);
                        // 対象数を抽選する
                        cacheTargets = LotsTargets(cacheTargets, effect.Count);
                    }

                    // 計算ロジック取得
                    Logic logic = null;
                    switch (effect.Type)
                    {
                        case BattleConst.Effect.Damage:
                            logic = logics["通常ダメージ"];
                            break;
                        case BattleConst.Effect.DamageRatio:
                            logic = logics["割合ダメージ"];
                            break;
                        case BattleConst.Effect.Recovery:
                            logic = logics["通常回復"];
                            break;
                        case BattleConst.Effect.RecoveryRatio:
                            logic = logics["割合回復"];
                            break;
                        case BattleConst.Effect.Buff_Atk:
                        case BattleConst.Effect.Buff_Def:
                        case BattleConst.Effect.Buff_MAtk:
                        case BattleConst.Effect.Buff_MDef:
                        case BattleConst.Effect.Debuff_Atk:
                        case BattleConst.Effect.Debuff_Def:
                        case BattleConst.Effect.Debuff_MAtk:
                        case BattleConst.Effect.Debuff_MDef:
                            logic = logics["補正値"];
                            break;
                    }

                    Assert.IsTrue(logic != null, "ロジックがnullになっています");
                    // 対象に効果を与える
                    foreach (var target in cacheTargets)
                    {
                        // 効果値を計算
                        var value = logic(action.behavioure, target, effect);
                        action.effects.Add(new BattleEffect(target, effect, value));
                    }

                }
            } else
            {
                // 効果の対象一覧取得
                cacheTargets = EffectTargets(action.behavioure, null);
                // 対象数を抽選する:必ず単体攻撃
                cacheTargets = LotsTargets(cacheTargets, 1);
                Logic logic = logics["通常ダメージ"];
                // 対象に効果を与える
                foreach (var target in cacheTargets)
                {
                    // 効果値を計算
                    var value = logic(action.behavioure, target, null);
                    action.effects.Add(new BattleEffect(target, null, value));
                }
            }
            return action;
        }

        /// <summary>
        /// ロジックを登録
        /// </summary>
        /// <param name="name"></param>
        /// <param name="logic"></param>
        void Register(string name, Logic logic)
        {
            logics.Add(name, logic);
        }

        /// <summary>
        /// 通常ダメージ
        /// </summary>
        int Damage(BattleUnit behavioure, BattleUnit target, Effect effect)
        {
            return 10;
        }

        /// <summary>
        /// 割合ダメージ
        /// </summary>
        int DamageRatio(BattleUnit behavioure, BattleUnit target, Effect effect)
        {
            return 10;
        }

        /// <summary>
        /// 通常回復
        /// </summary>
        int Recovery(BattleUnit behavioure, BattleUnit target, Effect effect)
        {
            return 10;
        }

        /// <summary>
        /// 割合回復
        /// </summary>
        int RecoveryRatio(BattleUnit behavioure, BattleUnit target, Effect effect)
        {
            return 10;
        }

        /// <summary>
        /// 補正値
        /// </summary>
        int Correction(BattleUnit behavioure, BattleUnit target, Effect effect)
        {
            return 5;
        }
    }
}

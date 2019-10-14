using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Alice
{
    public class BattleLogic
    {
        public static BattleLogic Instance { get; private set; }
        static BattleLogic() { Instance = new BattleLogic(); }

        // ロジックの共通インタフェース定義
        delegate int Logic();
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
        /// ロジックに沿って効果を返す
        /// </summary>
        /// <returns></returns>
        public BattleAction Exec(BattleAction action)
        {
            if(action.skill != null)
            {
                foreach(var effect in action.skill.EffectsRef)
                {
                    switch(effect.Type)
                    {
                        case BattleConst.Effect.Damage:
                            action.effects.Add(new BattleEffect(null, effect.Type, logics["通常ダメージ"]()));
                            break;
                        case BattleConst.Effect.DamageRatio:
                            action.effects.Add(new BattleEffect(null, effect.Type, logics["割合ダメージ"]()));
                            break;
                        case BattleConst.Effect.Recovery:
                            action.effects.Add(new BattleEffect(null, effect.Type, logics["通常回復"]()));
                            break;
                        case BattleConst.Effect.RecoveryRatio:
                            action.effects.Add(new BattleEffect(null, effect.Type, logics["割合回復"]()));
                            break;
                        case BattleConst.Effect.Buff_All:
                        case BattleConst.Effect.Buff_Atk:
                        case BattleConst.Effect.Buff_Def:
                        case BattleConst.Effect.Buff_MAtk:
                        case BattleConst.Effect.Buff_MDef:
                        case BattleConst.Effect.Debuff_All:
                        case BattleConst.Effect.Debuff_Atk:
                        case BattleConst.Effect.Debuff_Def:
                        case BattleConst.Effect.Debuff_MAtk:
                        case BattleConst.Effect.Debuff_MDef:
                            action.effects.Add(new BattleEffect(null, effect.Type, logics["補正値"]()));
                            break;
                    }
                }
            } else
            {
                action.effects.Add(new BattleEffect(null, BattleConst.Effect.Damage, logics["通常ダメージ"]()));
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
        int Damage()
        {
            return 10;
        }

        /// <summary>
        /// 割合ダメージ
        /// </summary>
        int DamageRatio()
        {
            return 10;
        }

        /// <summary>
        /// 通常回復
        /// </summary>
        int Recovery()
        {
            return 10;
        }

        /// <summary>
        /// 割合回復
        /// </summary>
        int RecoveryRatio()
        {
            return 10;
        }

        /// <summary>
        /// 補正値
        /// </summary>
        int Correction()
        {
            return 5;
        }
    }
}

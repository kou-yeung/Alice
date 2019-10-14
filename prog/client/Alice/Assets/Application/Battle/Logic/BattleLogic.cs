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
            Register("スキルダメージ", Skill);
            Register("回復", Recovery);
            Register("バフ", Buff);
        }

        /// <summary>
        /// ロジックに沿って効果を返す
        /// </summary>
        /// <returns></returns>
        public BattleAction Exec(BattleAction action)
        {
            if(action.skill != null)
            {
                switch(action.skill)
                {
                    case "SKILL_002_001":
                        action.effects.Add(new BattleEffect(null, BattleConst.Effect.Recovery, logics["回復"]()));
                        action.effects.Add(new BattleEffect(null, BattleConst.Effect.Recovery, logics["回復"]()));
                        break;
                    case "SKILL_001_001":
                        action.effects.Add(new BattleEffect(null, BattleConst.Effect.Buff_All, logics["バフ"]()));
                        break;
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
        /// ダメージ
        /// </summary>
        int Damage()
        {
            return 10;
        }

        /// <summary>
        /// スキル
        /// </summary>
        int Skill()
        {
            return 20;
        }

        /// <summary>
        /// 回復
        /// </summary>
        int Recovery()
        {
            return 10;
        }

        /// <summary>
        /// バフ
        /// </summary>
        int Buff()
        {
            return 5;
        }
    }
}

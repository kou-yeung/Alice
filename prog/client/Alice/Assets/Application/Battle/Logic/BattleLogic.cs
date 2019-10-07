using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Alice
{
    public class BattleLogic
    {
        // ロジックの共通インタフェース定義
        delegate int Logic();
        // ロジックカタログ
        Dictionary<string, Logic> logics = new Dictionary<string, Logic>();

        public BattleLogic()
        {
            Register("通常ダメージ", Damage);
            Register("スキルダメージ", Skill);
        }

        /// <summary>
        /// ロジックに沿って効果を返す
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public int Exec(string type)
        {
            return logics[type]();
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
        /// <returns></returns>
        int Damage()
        {
            return 10;
        }

        /// <summary>
        /// スキル
        /// </summary>
        /// <returns></returns>
        int Skill()
        {
            return 20;
        }
    }
}

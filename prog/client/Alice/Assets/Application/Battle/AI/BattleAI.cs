using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Alice.Entities;

namespace Alice
{
    /// <summary>
    /// ユニットの行動選択
    /// </summary>
    public class BattleAI
    {
        public static BattleAI Instance { get; private set; }
        static BattleAI() { Instance = new BattleAI(); }

        // AIの共通インタフェース定義
        delegate Skill AI(BattleUnit behaviour);
        // ロジックカタログ
        Dictionary<string, AI> ais = new Dictionary<string, AI>();

        public BattleAI()
        {
            //Register("回復", Recovery);
            //Register("バフスキル", Buff);
            Register("回復", Empty);
            Register("バフ", Empty);
            Register("デバフ", Empty);
            Register("バフ解除", Empty);
            Register("デバフ解除", Empty);
            Register("ダメージ", Empty);
        }

        /// <summary>
        /// 行動者と受け取り、使用するスキルIDを返す
        /// return : スキルID(string) 通常攻撃(null)
        /// </summary>
        /// <returns></returns>
        public Skill Exec(BattleUnit behaviour)
        {
            // 思考による抽選する
            foreach(var ai in behaviour.ais)
            {
                var skill = ais[ai](behaviour);
                if (skill != null) return skill;
            }
            // 通常攻撃
            return null;
        }

        /// <summary>
        /// AIを登録
        /// </summary>
        /// <param name="name"></param>
        /// <param name="ai"></param>
        void Register(string name, AI ai)
        {
            ais.Add(name, ai);
        }
        /// <summary>
        /// 抽選
        /// </summary>
        /// <returns></returns>
        bool Lots(int rate, int max = 100)
        {
            var random = Battle.Instance.random;
            return rate > random.Next(max);
        }

        /// <summary>
        /// 回復スキルの使用抽選
        /// </summary>
        /// <returns></returns>
        Skill Recovery(BattleUnit behaviour)
        {
            var types = new[] { BattleConst.Effect.Recovery, BattleConst.Effect.RecoveryRatio };
            var skills = behaviour.skills.Where(v => v.HasEffect(types));
            foreach (var skill in skills)
            {
                // 将来は一回のみ抽選するようにします
                if (this.Lots(30)) return skill;
            }
            return null;
        }

        /// <summary>
        /// バフスキルの使用抽選
        /// </summary>
        /// <param name="behaviour"></param>
        /// <returns></returns>
        Skill Buff(BattleUnit behaviour)
        {
            var types = new[]
            {
                BattleConst.Effect.Buff_All,
                BattleConst.Effect.Buff_Atk,
                BattleConst.Effect.Buff_Def,
                BattleConst.Effect.Buff_MAtk,
                BattleConst.Effect.Buff_MDef,
            };
            var skills = behaviour.skills.Where(v => v.HasEffect(types));
            foreach (var skill in skills)
            {
                // 将来は一回のみ抽選するようにします
                if (this.Lots(30)) return skill;
            }
            return null;
        }

        /// <summary>
        /// 未実装時使用
        /// </summary>
        /// <param name="behaviour"></param>
        /// <returns></returns>
        Skill Empty(BattleUnit behaviour)
        {
            return null;
        }
    }
}

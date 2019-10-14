using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

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
        delegate string AI(BattleUnit behaviour);
        // ロジックカタログ
        Dictionary<string, AI> ais = new Dictionary<string, AI>();
        // 数字からAI登録名に変換用マップ
        Dictionary<int, string> aiMap = new Dictionary<int, string>()
        {
            { 1, "回復スキル" },
            { 2, "バフスキル" },
        };

        public BattleAI()
        {
            Register("回復スキル", Recovery);
            Register("バフスキル", Buff);
        }

        /// <summary>
        /// 行動者と受け取り、使用するスキルIDを返す
        /// return : スキルID(string) 通常攻撃(null)
        /// </summary>
        /// <returns></returns>
        public string Exec(BattleUnit behaviour)
        {
            // 実行順によって抽選する
            var ids = new[] { 2, 1 };
            foreach(var ai in ids.Select(id => aiMap[id]))
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
        string Recovery(BattleUnit behaviour)
        {
            return this.Lots(30) ? "SKILL_002_001" : null;
        }

        /// <summary>
        /// バフスキルの使用抽選
        /// </summary>
        /// <param name="behaviour"></param>
        /// <returns></returns>
        string Buff(BattleUnit behaviour)
        {
            return this.Lots(30) ? "SKILL_001_001" : null;
        }
    }
}

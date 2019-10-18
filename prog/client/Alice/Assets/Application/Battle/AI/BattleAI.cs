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
            Register("回復", Recovery);
            Register("バフ", Buff);
            Register("デバフ", Debuff);
            Register("バフ解除", BuffCancel);
            Register("デバフ解除", DebuffCancel);
            Register("ダメージ", Damage);
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
        /// 回復
        /// </summary>
        /// <returns></returns>
        Skill Recovery(BattleUnit behaviour)
        {
            // トリガ抽選
            if (!Lots(behaviour.Trigger(BattleConst.SkillType.Recovery))) return null;

            var types = new[] { BattleConst.Effect.Recovery, BattleConst.Effect.RecoveryRatio };
            var skills = behaviour.skills.Where(v => v.HasEffect(types));
            foreach (var skill in skills)
            {
                // todo:cooltimeチェック
                return skill;
            }
            return null;
        }

        /// <summary>
        /// バフ
        /// </summary>
        /// <param name="behaviour"></param>
        /// <returns></returns>
        Skill Buff(BattleUnit behaviour)
        {
            // トリガ抽選
            if (!Lots(behaviour.Trigger(BattleConst.SkillType.Buff))) return null;

            var types = new[]
            {
                BattleConst.Effect.Buff_Atk, BattleConst.Effect.Buff_Def,
                BattleConst.Effect.Buff_MAtk, BattleConst.Effect.Buff_MDef,
                BattleConst.Effect.Buff_Wait,
            };
            var skills = behaviour.skills.Where(v => v.HasEffect(types));
            foreach (var skill in skills)
            {
                // todo:cooltimeチェック
                return skill;
            }
            return null;
        }

        /// <summary>
        /// デバフ
        /// </summary>
        /// <param name="behaviour"></param>
        /// <returns></returns>
        Skill Debuff(BattleUnit behaviour)
        {
            // トリガ抽選
            if (!Lots(behaviour.Trigger(BattleConst.SkillType.Debuff))) return null;

            var types = new[]
            {
                BattleConst.Effect.Debuff_Atk, BattleConst.Effect.Debuff_Def,
                BattleConst.Effect.Debuff_MAtk, BattleConst.Effect.Debuff_MDef,
                BattleConst.Effect.Debuff_Wait,
            };
            var skills = behaviour.skills.Where(v => v.HasEffect(types));
            foreach (var skill in skills)
            {
                // todo:cooltimeチェック
                return skill;
            }
            return null;
        }

        /// <summary>
        /// バフ解除
        /// </summary>
        /// <param name="behaviour"></param>
        /// <returns></returns>
        Skill BuffCancel(BattleUnit behaviour)
        {
            // トリガ抽選
            if (!Lots(behaviour.Trigger(BattleConst.SkillType.BuffCancel))) return null;

            var types = new[]
            {
                BattleConst.Effect.BuffCancel_All,
                BattleConst.Effect.BuffCancel_Atk, BattleConst.Effect.BuffCancel_Def,
                BattleConst.Effect.BuffCancel_MAtk, BattleConst.Effect.BuffCancel_MDef,
                BattleConst.Effect.BuffCancel_Wait,
            };
            var skills = behaviour.skills.Where(v => v.HasEffect(types));
            foreach (var skill in skills)
            {
                // todo:cooltimeチェック
                return skill;
            }
            return null;
        }

        /// <summary>
        /// デバフ解除
        /// </summary>
        /// <param name="behaviour"></param>
        /// <returns></returns>
        Skill DebuffCancel(BattleUnit behaviour)
        {
            // トリガ抽選
            if (!Lots(behaviour.Trigger(BattleConst.SkillType.DebuffCancel))) return null;

            var types = new[]
            {
                BattleConst.Effect.DebuffCancel_All,
                BattleConst.Effect.DebuffCancel_Atk, BattleConst.Effect.DebuffCancel_Def,
                BattleConst.Effect.DebuffCancel_MAtk, BattleConst.Effect.DebuffCancel_MDef,
                BattleConst.Effect.DebuffCancel_Wait,
            };
            var skills = behaviour.skills.Where(v => v.HasEffect(types));
            foreach (var skill in skills)
            {
                // todo:cooltimeチェック
                return skill;
            }
            return null;
        }

        /// <summary>
        /// ダメージ
        /// </summary>
        /// <param name="behaviour"></param>
        /// <returns></returns>
        Skill Damage(BattleUnit behaviour)
        {
            // トリガ抽選
            if (!Lots(behaviour.Trigger(BattleConst.SkillType.Damage))) return null;

            var types = new[] { BattleConst.Effect.Damage, BattleConst.Effect.DamageRatio };
            var skills = behaviour.skills.Where(v => v.HasEffect(types));
            foreach (var skill in skills)
            {
                // todo:cooltimeチェック
                return skill;
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

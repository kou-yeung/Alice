using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Alice.Entities;
using System;

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
        delegate Skill AI(BattleUnit behaviour, Skill[] skills);
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
            // 使用可能スキル一覧を取得する
            var skills = behaviour.skills.Where(v => behaviour.CanUseSkill(v)).ToArray();

            // 使用するスキルがないのでチェックする必要がありません
            if (skills.Length <= 0) return null;

            // 思考による抽選する
            foreach (var ai in behaviour.ais)
            {
                var skill = ais[ai](behaviour, skills);
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
        /// スキル抽選
        /// </summary>
        /// <param name="skill"></param>
        /// <returns></returns>
        Skill Lots(Skill[] skill)
        {
            if (skill == null) return null;
            if (skill.Length <= 0) return null;
            // 抽選する
            var random = Battle.Instance.random;
            return skill[random.Next(0, skill.Length)];
        }

        /// <summary>
        /// このスキル発動するかどうかのチェック用共通処理
        /// </summary>
        /// <param name="behaviour"></param>
        /// <param name="skill"></param>
        /// <param name="check"></param>
        bool CheckEffectTarget(BattleUnit behaviour, Skill skill, Func<Effect, List<BattleUnit>, bool> check)
        {
            List<BattleUnit> cacheTargets = null;
            foreach (var effect in skill.EffectsRef)
            {
                if (effect.Target != BattleConst.Target.Accession)
                {
                    // 効果の対象一覧取得
                    cacheTargets = BattleLogic.EffectTargets(behaviour, effect);
                }
                if (check(effect, cacheTargets)) return true;
            }
            return false;
        }

        /// <summary>
        /// 回復
        /// </summary>
        /// <returns></returns>
        Skill Recovery(BattleUnit behaviour, Skill[] skills)
        {
            // トリガ抽選
            if (!Lots(behaviour.Trigger(BattleConst.SkillType.Recovery))) return null;

            var types = new[] { BattleConst.Effect.Recovery, BattleConst.Effect.RecoveryRatio };
            var option = skills.Where(v =>
            {
                if (!v.HasEffect(types)) return false;
                // エフェクト対象がダメージを受けてる
                return CheckEffectTarget(behaviour, v, (effect, units) =>
                {
                    switch(effect.Type)
                    {
                        // 割合ダメージの場合
                        case BattleConst.Effect.RecoveryRatio:
                            return units.Any(unit =>
                            {
                                var value = unit.current.MaxHP * (effect.Value / 100f);
                                var damage = unit.current.MaxHP - unit.current.HP;
                                return damage >= value;
                            });
                        // 通常ダメージ
                        default:
                        case BattleConst.Effect.Recovery:
                            return units.Any(unit => unit.current.HP != unit.current.MaxHP);
                    }
                });
            }).ToArray();
            // 抽選する
            return Lots(option);
        }

        /// <summary>
        /// バフ
        /// </summary>
        /// <param name="behaviour"></param>
        /// <returns></returns>
        Skill Buff(BattleUnit behaviour, Skill[] skills)
        {
            // トリガ抽選
            if (!Lots(behaviour.Trigger(BattleConst.SkillType.Buff))) return null;

            var types = new[]
            {
                BattleConst.Effect.Buff_Atk, BattleConst.Effect.Buff_Def,
                BattleConst.Effect.Buff_MAtk, BattleConst.Effect.Buff_MDef,
                BattleConst.Effect.Buff_Wait,
            };
            var option = skills.Where(v =>
            {
                if (!v.HasEffect(types)) return false;
                return CheckEffectTarget(behaviour, v, (effect, units) =>
                {
                    switch (effect.Type)
                    {
                        // 魔法攻撃アップの場合、魔法スキルが所持しているかを確認する
                        case BattleConst.Effect.Buff_MAtk:
                            return units.Any(unit => unit.skills.Any(skill => skill.Attribute == BattleConst.Attribute.Magic));
                        // それ以外のパラメータはチェックしない
                        default:return true;
                    }
                });
            }).ToArray();
            // 抽選する
            return Lots(option);
        }

        /// <summary>
        /// デバフ
        /// </summary>
        /// <param name="behaviour"></param>
        /// <returns></returns>
        Skill Debuff(BattleUnit behaviour, Skill[] skills)
        {
            // トリガ抽選
            if (!Lots(behaviour.Trigger(BattleConst.SkillType.Debuff))) return null;

            var types = new[]
            {
                BattleConst.Effect.Debuff_Atk, BattleConst.Effect.Debuff_Def,
                BattleConst.Effect.Debuff_MAtk, BattleConst.Effect.Debuff_MDef,
                BattleConst.Effect.Debuff_Wait,
            };

            var option = skills.Where(v =>
            {
                if (!v.HasEffect(types)) return false;
                return CheckEffectTarget(behaviour, v, (effect, units) =>
                {
                    switch (effect.Type)
                    {
                        // 魔法攻撃ダウンの場合、魔法スキルが所持しているかを確認する
                        case BattleConst.Effect.Debuff_MAtk:
                            return units.Any(unit => unit.skills.Any(skill => skill.Attribute == BattleConst.Attribute.Magic));
                        // それ以外のパラメータはチェックしない
                        default: return true;
                    }
                });
            }).ToArray();
            // 抽選する
            return Lots(option);
        }

        /// <summary>
        /// バフ解除
        /// </summary>
        /// <param name="behaviour"></param>
        /// <returns></returns>
        Skill BuffCancel(BattleUnit behaviour, Skill[] skills)
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
            var option = skills.Where(v =>
            {
                if (!v.HasEffect(types)) return false;
                return CheckEffectTarget(behaviour, v, (effect, units) =>
                {
                    switch (effect.Type)
                    {
                        // 何かのバフを受けてるかチェック
                        case BattleConst.Effect.BuffCancel_All:
                            return units.Any(unit => unit.BuffCount() > 0);
                        default:
                            // 指定した効果をかかっているか
                            return units.Any(unit => unit.HasCondition(BattleConst.Cancel2Effect(effect.Type)));
                    }
                });
            }).ToArray();
            // 抽選する
            return Lots(option);
        }

        /// <summary>
        /// デバフ解除
        /// </summary>
        /// <param name="behaviour"></param>
        /// <returns></returns>
        Skill DebuffCancel(BattleUnit behaviour, Skill[] skills)
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

            var option = skills.Where(v =>
            {
                if (!v.HasEffect(types)) return false;
                return CheckEffectTarget(behaviour, v, (effect, units) =>
                {
                    switch (effect.Type)
                    {
                        // 何かのデバフを受けてるかチェック
                        case BattleConst.Effect.DebuffCancel_All:
                            return units.Any(unit => unit.DebuffCount() > 0);
                        default:
                            // 指定した効果をかかっているか
                            return units.Any(unit => unit.HasCondition(BattleConst.Cancel2Effect(effect.Type)));
                    }
                });
            }).ToArray();
            // 抽選する
            return Lots(option);
        }

        /// <summary>
        /// ダメージ
        /// </summary>
        /// <param name="behaviour"></param>
        /// <returns></returns>
        Skill Damage(BattleUnit behaviour, Skill[] skills)
        {
            // トリガ抽選
            if (!Lots(behaviour.Trigger(BattleConst.SkillType.Damage))) return null;
            var types = new[] { BattleConst.Effect.Damage, BattleConst.Effect.DamageRatio };
            var option = skills.Where(v => v.HasEffect(types)).ToArray();
            return Lots(option);
        }

        /// <summary>
        /// 未実装時使用
        /// </summary>
        /// <param name="behaviour"></param>
        /// <returns></returns>
        Skill Empty(BattleUnit behaviour, Skill[] skills)
        {
            return null;
        }
    }
}

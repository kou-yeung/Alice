using System.Collections.Generic;
using Alice.Entities;
using System.Linq;
using System.Text;

namespace Alice.Generic
{
    public static class Message
    {
        static StringBuilder sb = new StringBuilder();

        class Map
        {
            public string desc;
            public string end;

            public Map(string desc, string end)
            {
                this.desc = desc;
                this.end = end;
            }
        }

        static Dictionary<BattleConst.Effect, Map> map = new Dictionary<BattleConst.Effect, Map>
        {
            // 即座効果
            {BattleConst.Effect.Damage, new Map("ダメージを与え", "る") },
            {BattleConst.Effect.DamageRatio, new Map("割合ダメージを与え", "る") },
            {BattleConst.Effect.Recovery, new Map("回復", "する") },
            {BattleConst.Effect.RecoveryRatio, new Map("割合回復", "する") },
            // バフ
            {BattleConst.Effect.Buff_Atk, new Map("物理攻撃アップ", "する") },
            {BattleConst.Effect.Buff_Def, new Map("物理防御アップ", "する") },
            {BattleConst.Effect.Buff_MAtk, new Map("魔法攻撃アップ", "する") },
            {BattleConst.Effect.Buff_MDef, new Map("魔法防御アップ", "する") },
            {BattleConst.Effect.Buff_Wait, new Map("素早さアップ", "する") },

            // デバフ
            {BattleConst.Effect.Debuff_Atk, new Map("物理攻撃ダウン", "する") },
            {BattleConst.Effect.Debuff_Def, new Map("物理防御ダウン", "する") },
            {BattleConst.Effect.Debuff_MAtk, new Map("魔法攻撃ダウン", "する") },
            {BattleConst.Effect.Debuff_MDef, new Map("魔法防御ダウン", "する") },
            {BattleConst.Effect.Debuff_Wait, new Map("素早さダウン", "する") },

            // バフ解除
            {BattleConst.Effect.BuffCancel_Atk, new Map("物理攻撃アップ効果を解除", "する") },
            {BattleConst.Effect.BuffCancel_Def, new Map("物理防御アップ効果を解除", "する") },
            {BattleConst.Effect.BuffCancel_MAtk, new Map("魔法攻撃アップ効果を解除", "する") },
            {BattleConst.Effect.BuffCancel_MDef, new Map("魔法防御アップ効果を解除", "する") },
            {BattleConst.Effect.BuffCancel_Wait, new Map("素早さアップ効果を解除", "する") },
            {BattleConst.Effect.BuffCancel_All, new Map("すべてアップ効果を解除", "する") },

            // デバフ解除
            {BattleConst.Effect.DebuffCancel_Atk, new Map("物理攻撃ダウン効果を解除", "する") },
            {BattleConst.Effect.DebuffCancel_Def, new Map("物理防御ダウン効果を解除", "する") },
            {BattleConst.Effect.DebuffCancel_MAtk, new Map("魔法攻撃ダウン効果を解除", "する") },
            {BattleConst.Effect.DebuffCancel_MDef, new Map("魔法防御ダウン効果を解除", "する") },
            {BattleConst.Effect.DebuffCancel_Wait, new Map("素早さダウン効果を解除", "する") },
            {BattleConst.Effect.DebuffCancel_All, new Map("すべてダウン効果を解除", "する") },

        };
        /// <summary>
        /// スキルの説明文
        /// </summary>
        /// <param name="skill"></param>
        /// <returns></returns>
        public static string Desc(UserSkill data)
        {
            var skill = MasterData.FindSkillByID(data.id);
            var effects = skill.Effects;

            sb.Clear();

            var end = "";

            foreach (var id in effects)
            {
                var effect = MasterData.effects.First(v => v.ID == id);

                if(sb.Length != 0)
                {
                    if(end != "る") sb.Append("し、");
                    else sb.Append("、");
                }

                // 敵/味方/自分/さらに(継承)
                // 対象数: 自分は必要なし
                switch(effect.Target)
                {
                    case BattleConst.Target.Self:
                        sb.Append("自分に");
                        break;
                    case BattleConst.Target.Friend:
                        sb.Append($"味方{effect.Count}体に");
                        break;
                    case BattleConst.Target.Enemy:
                        sb.Append($"敵{effect.Count}体に");
                        break;
                    case BattleConst.Target.Accession:
                        sb.Append($"さらに{effect.Count}体に");
                        break;
                }

                var info = map[effect.Type];
                sb.Append(info.desc);
                end = info.end;
            }
            sb.Append(end);

            return sb.ToString();
        }
    }
}

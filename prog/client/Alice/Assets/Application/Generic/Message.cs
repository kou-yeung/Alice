using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Alice.Entities;
using System;
using System.Linq;
using System.Text;

namespace Alice.Generic
{
    public static class Message
    {
        static StringBuilder sb = new StringBuilder();
        /// <summary>
        /// スキルの説明文
        /// </summary>
        /// <param name="skill"></param>
        /// <returns></returns>
        public static string Desc(UserSkill data)
        {
            var skill = MasterData.skills.First(v => v.ID == data.id);
            var effects = skill.Effects;

            sb.Clear();

            var end = "";

            foreach (var id in effects)
            {
                var effect = MasterData.effects.First(v => v.ID == id);
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
                switch(effect.Type)
                {
                    case BattleConst.Effect.Damage:
                        sb.Append("ダメージを与え");
                        end = "る";
                        break;
                    case BattleConst.Effect.DamageRatio:
                        sb.Append("割合ダメージを与え");
                        end = "る";
                        break;
                }
            }
            sb.Append("する");

            return sb.ToString();
        }
    }
}

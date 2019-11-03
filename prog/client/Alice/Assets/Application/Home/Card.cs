using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zoo.IO;
using Alice.Entities;
using System.Linq;
using Zoo.Assets;
using System.Text;
using System;

namespace Alice
{
    /// <summary>
    /// ユニット情報を表示するカード
    /// </summary>
    public class Card : MonoBehaviour
    {
        public Thumbnail thumbnail;
        public Text Param;
        public GameObject info;
        public Button[] skill;

        public Action<int> OnEditEvent;
        public Action<UserUnit, int> OnSkillEvent;

        UserUnit cacheUnit;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="unit"></param>
        public void Setup(UserUnit unit)
        {
            cacheUnit = unit;

            if (unit == null)
            {
                info.SetActive(false);
                return;
            }
            info.SetActive(true);

            thumbnail.Setup(unit);

            var data = MasterData.characters.FirstOrDefault(v => v.ID == unit.characterId);
            var lv = unit.Level();
            var b = data.Base;
            var g = data.Grow;

            var sb = new StringBuilder();
            sb.AppendLine($"ATK: {b.Atk + g.Atk * lv}");
            sb.AppendLine($"MATK: {b.MAtk + g.MAtk * lv}");
            sb.AppendLine($"Def: {b.Def + g.Def * lv}");
            sb.AppendLine($"MDef: {b.MDef + g.MDef * lv}");
            sb.AppendLine($"WAIT: {data.Wait}");
            var next = (lv) * (lv) - unit.exp;
            sb.AppendLine($"LVUP: {next}");

            // スキル設定
            for (int i = 0; i < skill.Length; i++)
            {
                var text = skill[i].GetComponentInChildren<Text>();

                if (i < unit.skill.Length)
                {
                    text.text = MasterData.FindSkillByID(unit.skill[i])?.Name;
                }
                else
                {
                    text.text = "+";
                }
            }

            Param.text = sb.ToString();
        }

        /// <summary>
        /// 編集を押します
        /// </summary>
        public void OnEdit(int index)
        {
            OnEditEvent?.Invoke(index);
        }

        /// <summary>
        /// スキルを押します
        /// </summary>
        public void OnSkill(int index)
        {
            OnSkillEvent?.Invoke(cacheUnit, index);
        }
    }
}

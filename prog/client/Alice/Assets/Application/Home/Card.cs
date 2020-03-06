﻿using System.Collections;
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
        [SerializeField]
        Slider gauge = null;

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


            var data = MasterData.Instance.Find(unit);
            var param = data.ParamAtLevel(unit.Level());

            var sb = new StringBuilder();
            sb.AppendLine($"ATK: {param.Atk}");
            sb.AppendLine($"Def: {param.Def}");
            sb.AppendLine($"MATK: {param.MAtk}");
            sb.AppendLine($"MDef: {param.MDef}");
            sb.AppendLine($"WAIT: {data.Wait}");

            gauge.value = unit.Ratio2Levelup();

            // スキル設定
            for (int i = 0; i < skill.Length; i++)
            {
                var text = skill[i].GetComponentInChildren<Text>();

                if (i < unit.skill.Length)
                {
                    text.text = MasterData.Instance.FindSkillByID(unit.skill[i])?.NameWithInfo;
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

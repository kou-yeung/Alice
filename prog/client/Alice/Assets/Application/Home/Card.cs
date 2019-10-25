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
        public Action<int> OnEditEvent;

        public GameObject info;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="unit"></param>
        public void Setup(UserUnit unit)
        {
            if(unit == null)
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
            Debug.Log($"OnSkill:{index}");
        }
    }
}

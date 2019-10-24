using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zoo.IO;
using Alice.Entities;
using System.Linq;
using Zoo.Assets;
using System.Text;

namespace Alice
{
    /// <summary>
    /// ユニット情報を表示するカード
    /// </summary>
    public class Card : MonoBehaviour
    {
        public TimelineIcon icon;
        public Text level;
        public Text Param;

        public GameObject info;
        UserUnit currentUnit;
        Character characterData;

        string IconPath
        {
            get
            {
                return $"Character/{characterData.Image}/icon.asset";
            }
        }        
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

            currentUnit = unit;
            characterData = MasterData.characters.FirstOrDefault(v => v.ID == currentUnit.characterId);
            // アイコン
            LoaderService.Instance.Preload(new []{IconPath}, () =>
            {
                if (currentUnit != unit) return;
                var sprites = LoaderService.Instance.Load<Sprites>(IconPath);
                icon.Setup(characterData);
            });
            var lv = unit.Level();
            // レベル
            level.text = $"Lv.{lv}";

            var b = characterData.Base;
            var g = characterData.Grow;

            var sb = new StringBuilder();
            sb.AppendLine($"ATK: {b.Atk + g.Atk * lv}");
            sb.AppendLine($"MATK: {b.MAtk + g.MAtk * lv}");
            sb.AppendLine($"Def: {b.Def + g.Def * lv}");
            sb.AppendLine($"MDef: {b.MDef + g.MDef * lv}");
            sb.AppendLine($"WAIT: {characterData.Wait}");
            var next = (lv) * (lv) - unit.exp;
            sb.AppendLine($"LVUP: {next}");
            Param.text = sb.ToString();
        }
    }
}

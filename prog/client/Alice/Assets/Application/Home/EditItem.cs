using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text;
using Alice.Entities;
using Alice.Logic;

namespace Alice
{
    /// <summary>
    /// デッキ編成アイテム
    /// </summary>
    public class EditItem : MonoBehaviour
    {
        [SerializeField]
        Thumbnail thumbnail = null;
        [SerializeField]
        Slider gauge = null;
        [SerializeField]
        Text info = null;

        public UserUnit cacheUnit { get; private set; }
        public void Setup(UserUnit unit, UserUnit diff = null)
        {
            this.cacheUnit = unit;
            gameObject.SetActive(unit != null);
            if (unit == null) return;

            thumbnail.Setup(unit);

            var sb = new StringBuilder();

            var level = unit.Level();
            var data = MasterData.Instance.Find(unit);
            var param = data.ParamAtLevel(level);

            if (diff != null)
            {
                var diffData = MasterData.Instance.Find(diff);
                var diffParam = diffData.ParamAtLevel(diff.Level());

                sb.AppendLine($"ATK:{param.Atk}{Diff(diffParam.Atk, param.Atk)}");
                sb.AppendLine($"DEF:{param.Def}{Diff(diffParam.Def, param.Def)}");
                sb.AppendLine($"MATK:{param.MAtk}{Diff(diffParam.MAtk, param.MAtk)}");
                sb.AppendLine($"MDEF:{param.MDef}{Diff(diffParam.MDef, param.MDef)}");
                sb.AppendLine($"WAIT:{data.Wait}{Diff(diffData.Wait, data.Wait, true)}");
            }
            else
            {
                sb.AppendLine($"ATK:{param.Atk}");
                sb.AppendLine($"DEF:{param.Def}");
                sb.AppendLine($"MATK:{param.MAtk}");
                sb.AppendLine($"MDEF:{param.MDef}");
                sb.AppendLine($"WAIT:{data.Wait}");
            }

            info.text = sb.ToString();
            gauge.value = unit.Ratio2Levelup();
        }

        string Diff(int from, int to, bool re = false)
        {
            var diff = from - to;

            if (diff > 0)
            {
                var color = re ? "blue" : "red";
                return $"<color={color}>(↓{Mathf.Abs(diff)})</color>";
            }
            else if (diff < 0)
            {
                var color = re ? "red": "blue";
                return $"<color={color}>(↑{Mathf.Abs(diff)})</color>";
            }
            else
            {
                return "";
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text;
using Alice.Entities;

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
        public void Setup(UserUnit unit)
        {
            this.cacheUnit = unit;
            gameObject.SetActive(unit != null);
            if (unit == null) return;

            thumbnail.Setup(unit);

            var sb = new StringBuilder();

            var level = unit.Level();
            var data = MasterData.Instance.Find(unit);
            var param = data.ParamAtLevel(level);

            sb.AppendLine($"ATK:{param.Atk} MATK:{param.MAtk}");
            sb.AppendLine($"DEF:{param.Def} MDEF:{param.MDef}");
            sb.AppendLine($"WAIT:{data.Wait}");
            info.text = sb.ToString();

            var start = (level-1) * (level - 1);
            var end = (level) * (level);

            Debug.Log($"start({start}) end({end}) exp({unit.exp})");
            var ratio = (float)(unit.exp-start) / (float)(end - start);
            gauge.value = ratio;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Alice.Entities;
using Zoo.IO;
using System.Linq;
using Zoo.Assets;

namespace Alice
{
    public class Thumbnail : MonoBehaviour
    {
        string IconPath
        {
            get
            {
                return $"Character/{characterData.Image}/icon.asset";
            }
        }

        public TimelineIcon icon;
        public Text level;
        public Text desc;

        UserUnit currentUnit;
        Character characterData;

        public void Setup(UserUnit unit)
        {
            currentUnit = unit;
            characterData = MasterData.characters.FirstOrDefault(v => v.ID == currentUnit.characterId);
            // アイコン
            LoaderService.Instance.Preload(new[] { IconPath }, () =>
            {
                if (currentUnit != unit) return;
                var sprites = LoaderService.Instance.Load<Sprites>(IconPath);
                icon.Setup(characterData);
            });
            var lv = unit.Level();
            // レベル
            level.text = $"Lv.{lv}";
        }

        /// <summary>
        /// 追加説明を設定
        /// </summary>
        /// <param name="richText"></param>
        public void SetDesc(string richText)
        {
            // 表示・非表示
            desc.transform.parent.gameObject.SetActive(!string.IsNullOrEmpty(richText));
            desc.text = richText;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zoo.IO;
using Alice.Entities;
using System.Linq;
using Zoo.Assets;

namespace Alice
{
    /// <summary>
    /// ユニット情報を表示するカード
    /// </summary>
    public class Card : MonoBehaviour
    {
        public TimelineIcon icon;
        public Text level;
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
                gameObject.SetActive(false);
            }
            gameObject.SetActive(true);

            currentUnit = unit;
            characterData = MasterData.characters.FirstOrDefault(v => v.ID == currentUnit.characterId);
            // アイコン
            LoaderService.Instance.Preload(new []{IconPath}, () =>
            {
                if (currentUnit != unit) return;
                var sprites = LoaderService.Instance.Load<Sprites>(IconPath);
                icon.Setup(characterData);
            });
            // レベル
            level.text = $"Lv.{unit.Level()}";
        }
    }
}

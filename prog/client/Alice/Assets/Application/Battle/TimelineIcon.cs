using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Alice.Entities;
using Zoo.Assets;
using Zoo.IO;
using Zoo;

namespace Alice
{
    public class TimelineIcon : MonoBehaviour
    {
        const string PrefabKey = "TimelineIcon";

        public Color colorSelf;
        public Color colorEnemy;
        public Image background;
        public Image icon;

        Sprites sprites;
        
        /// <summary>
        /// アイコンや背景色などセットアップする
        /// </summary>
        public void Setup(BattleUnit unit)
        {
            sprites = LoaderService.Instance.Load<Sprites>($"Character/{unit.characterData.Image}/icon.asset");
            // 背景色設定
            background.color = unit.side == BattleConst.Side.Player ? colorSelf : colorEnemy;
            // アイコンを設定する
            icon.sprite = sprites[0];
        }

        /// <summary>
        /// 破棄する
        /// </summary>
        public void Destroy()
        {
            PrefabPool.Release(PrefabKey, gameObject);
        }

        /// <summary>
        /// TimelineIconを生成する
        /// </summary>
        /// <param name="unit"></param>
        public static TimelineIcon Gen(BattleUnit unit)
        {
            if(!PrefabPool.Has(PrefabKey))
            {
                var prefab = LoaderService.Instance.Load<GameObject>("Prefab/TimelineIcon.prefab");
                PrefabPool.Regist(PrefabKey, prefab);
            }
            var go = PrefabPool.Get(PrefabKey);
            var res = go.GetComponent<TimelineIcon>();
            res.Setup(unit);
            return res;
        }
    }
}

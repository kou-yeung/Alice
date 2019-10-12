/// <summary>
/// バトルユニット:キャラ一体分
/// </summary>
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zoo.IO;
using Zoo.Assets;

namespace Alice
{
    public class BattleUnit
    {
        public BattleConst.Side side { get; private set; }
        public GameObject gameObject { get; private set; }
        public Image image { get; private set; }
        public Sprites sprites { get; private set; }

        public BattleUnit(string uniq, int id, BattleConst.Side side)
        {
            this.side = side;
            var prefab = LoaderService.Instance.Load<GameObject>("Character.prefab");
            gameObject = GameObject.Instantiate(prefab);

            sprites = LoaderService.Instance.Load<Sprites>(string.Format("Character/$yuhinamv{0:D3}.asset", id));
            image = gameObject.GetComponent<Image>();
            image.sprite = sprites[7];

            if(side == BattleConst.Side.Enemy)
            {
                image.transform.localScale = new Vector3(-1, 1, 1);
            }
        }
    }
}

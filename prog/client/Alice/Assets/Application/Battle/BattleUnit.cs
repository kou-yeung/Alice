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
using Alice.Entities;
using System.Linq;

namespace Alice
{
    public class BattleUnit
    {
        public BattleConst.Side side { get; private set; }
        public GameObject gameObject { get; private set; }
        public Image image { get; private set; }
        public Sprites sprites { get; private set; }
        public Character characterData { get; private set; }

        public BattleUnit(string uniq, string id, BattleConst.Side side)
        {
            this.characterData = MasterData.characters.First(v => v.ID == id);

            this.side = side;
            var prefab = LoaderService.Instance.Load<GameObject>("Character.prefab");
            gameObject = GameObject.Instantiate(prefab);

            sprites = LoaderService.Instance.Load<Sprites>(string.Format("Character/$yuhinamv{0}.asset", this.characterData.Image));
            image = gameObject.GetComponent<Image>();
            image.sprite = sprites[7];

            if(side == BattleConst.Side.Enemy)
            {
                image.transform.localScale = new Vector3(-1, 1, 1);
            }
        }
    }
}

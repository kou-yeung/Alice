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
        public GameObject gameObject { get; private set; }
        public Image image { get; private set; }
        public Sprites sprites { get; private set; }
        public BattleUnit(string uniq)
        {
            var prefab = LoaderService.Instance.Load<GameObject>("Character.prefab");
            gameObject = GameObject.Instantiate(prefab);
            sprites = LoaderService.Instance.Load<Sprites>("Character/$yuhinamv010.asset");

            image = gameObject.GetComponent<Image>();
            image.sprite = sprites[2];
        }
    }
}

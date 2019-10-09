/// <summary>
/// バトルユニット:キャラ一体分
/// </summary>
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zoo.IO;

namespace Alice
{
    public class BattleUnit
    {
        public Image image;
        public BattleUnit(string uniq)
        {
            Debug.Log(LoaderService.Instance.Load<IList<Sprite>>("Character/$yuhinamv001.png"));
        }
    }
}

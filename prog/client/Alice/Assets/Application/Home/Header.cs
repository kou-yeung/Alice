using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zoo;

namespace Alice
{
    public class Header : MonoBehaviour
    {
        public static Header Instance { get; private set; }
        public InputField Name;
        public Text Level;
        public Text Coin;

        void Awake()
        {
            Instance = this;
            Observer.AddObserver("HomeRecv", Setup);
            Setup();
        }

        /// <summary>
        /// セットアップ
        /// </summary>
        public void Setup()
        {
            var player = UserData.cacheHomeRecv.player;
            Name.text = string.IsNullOrEmpty(player.name) ? "ゲスト" : player.name;
            Level.text = $"LV: {player.exp}";
            Coin.text = $"COIN: {player.coin}";
        }
    }
}

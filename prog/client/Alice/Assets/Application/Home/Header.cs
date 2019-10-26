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
        public Text Rank;
        public Text Rate;
        public Text Alarm;
        public Text Ads;

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
            Rank.text = $"{player.rank + 1}";

            if(player.todayBattleCount >= 10)
            {
                var max = player.todayBattleCount;
                var win = player.todayWinCount * 100;
                var rate = Mathf.FloorToInt(win/max);
                Rate.text = $"({rate}%)";
            }
            else
            {
                Rate.text = $"(-)";
            }

            Alarm.text = $"{player.alarm}";
            Ads.text = $"{player.ads}";
        }
    }
}

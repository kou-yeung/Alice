using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zoo.Time;

namespace Alice
{
    /// <summary>
    /// 宝箱x1
    /// </summary>
    public class Chest : MonoBehaviour
    {
        [SerializeField]
        Image[] rare;
        [SerializeField]
        Slider remainGauge;
        [SerializeField]
        Text remainTime;

        public Action CliceEvent;
        UserChest cacheUserChest;

        public void Setup(UserChest chest)
        {
            cacheUserChest = chest;
            if (chest == null)
            {
                gameObject.SetActive(false);
                return;
            }
            gameObject.SetActive(true);
            // レアリティを設定する
            for (int i = 0; i < rare.Length; i++)
            {
                rare[i].gameObject.SetActive(i < chest.rate);
            }
            Update();
        }

        /// <summary>
        /// 
        /// </summary>
        void Update()
        {
            if (cacheUserChest == null) return;

            var remain = Math.Max(0, cacheUserChest.end - ServerTime.CurrentUnixTime);
            var max = cacheUserChest.end - cacheUserChest.start;

            if(remain <= 0)
            {
                remainGauge.gameObject.SetActive(false);
                remainTime.text = "★READY★";
            }
            else
            {
                remainGauge.gameObject.SetActive(true);
                remainGauge.value = (float)remain / (float)max;
                // 残り時間
                remainTime.text = string.Format("{0:D2}:{1:D2}", remain / 60, remain % 60);
            }
        }

        /// <summary>
        /// 宝箱がクリックされたら
        /// </summary>
        public void OnClick()
        {
            CliceEvent?.Invoke();
        }
    }
}

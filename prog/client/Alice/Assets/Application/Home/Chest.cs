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
        Image[] stars;
        [SerializeField]
        Text num;
        [SerializeField]
        Slider remainGauge;
        [SerializeField]
        Text remainTime;
        [SerializeField]
        Text alarm;

        public Action CliceEvent;
        public UserChest cacheUserChest { get; private set; }
        bool showAlarm;

        public void Setup(UserChest chest, bool showAlarm = true)
        {
            this.showAlarm = showAlarm;
            cacheUserChest = chest;
            if (chest == null)
            {
                gameObject.SetActive(false);
                return;
            }
            gameObject.SetActive(true);

            var rare = chest.rate + 1;
            if (rare > stars.Length)
            {
                num.gameObject.SetActive(true);
                num.text = rare.ToString();
                // レアリティを設定する
                for (int i = 0; i < stars.Length; i++)
                {
                    stars[i].gameObject.SetActive(i < 1);
                }
            }
            else
            {
                num.gameObject.SetActive(false);
                // レアリティを設定する
                for (int i = 0; i < stars.Length; i++)
                {
                    stars[i].gameObject.SetActive(i < rare);
                }
            }
            Update();
        }

        /// <summary>
        /// 
        /// </summary>
        void Update()
        {
            if (cacheUserChest == null) return;

            if(cacheUserChest.IsReady())
            {
                remainGauge.gameObject.SetActive(false);
                remainTime.text = "★READY★";
                alarm.transform.parent.gameObject.SetActive(false);
            }
            else
            {
                remainGauge.gameObject.SetActive(true);
                alarm.transform.parent.gameObject.SetActive(this.showAlarm);
                remainGauge.value = cacheUserChest.RemainRatio();
                // 残り時間
                remainTime.text = cacheUserChest.RemainText();
                // 必要なアラーム数
                var needAlarm = cacheUserChest.NeedAlarmNum();
                alarm.text = needAlarm.ToString();
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

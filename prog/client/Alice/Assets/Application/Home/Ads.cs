﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.Advertisements;
using Zoo.Communication;

namespace Alice
{
    /// <summary>
    /// 広告の開始＆終了管理
    /// </summary>
    public class Ads : MonoBehaviour
    {
        public static Ads Instance { get; private set; }
        Image blocker;

        // Start is called before the first frame update
        void Start()
        {
            Instance = this;
            blocker = GetComponent<Image>();
            blocker.enabled = false;
        }

        /// <summary>
        /// 広告表示
        /// </summary>
        /// <param name=""></param>
        public void Show(UserChest chest, Action<ShowResult> cb)
        {
            blocker.enabled = true;
            var send = new AdsBeginSend { chest = chest };
            CommunicationService.Instance.Request("BeginAds", JsonUtility.ToJson(send), (begin) =>
            {
                Advertisement.Show(new ShowOptions { resultCallback = (showResult)=>
                {
                    if(showResult == ShowResult.Finished)
                    {
                        blocker.enabled = false;
                        CommunicationService.Instance.Request("EndAds", begin, (end) =>
                        {
                            var data = JsonUtility.FromJson<AdsEndRecv>(end);
                            UserData.Modify(data.modifiedChest);
                            blocker.enabled = false;
                            cb?.Invoke(showResult);
                        });
                    }
                    else
                    {
                        blocker.enabled = false;
                        cb?.Invoke(showResult);
                    }
                }});
            });
        }
    }
}

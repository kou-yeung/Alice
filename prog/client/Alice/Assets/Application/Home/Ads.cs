using System.Collections;
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

        void Start()
        {
            Instance = this;
            blocker = GetComponent<Image>();
        }

        /// <summary>
        /// 広告表示
        /// </summary>
        /// <param name=""></param>
        public void Show(UserChest chest, Action<ShowResult> cb)
        {
            blocker.enabled = true;

            Advertisement.Show(new ShowOptions { resultCallback = (showResult)=>
            {
                if(showResult == ShowResult.Finished)
                {
                    Finished(chest, cb);
                }
                else
                {
                    blocker.enabled = false;
                    cb?.Invoke(showResult);
                }
            }});
        }

        public void Finished(UserChest chest, Action<ShowResult> cb)
        {
            var c2v = new AdsSend();
            c2v.token = UserData.cacheHomeRecv.player.token;
            c2v.chest = chest;

            CommunicationService.Instance.Request("Ads", JsonUtility.ToJson(c2v), (res) =>
            {
                var data = JsonUtility.FromJson<AdsRecv>(res);
                UserData.Modify(data.modified);
                blocker.enabled = false;
                cb?.Invoke(ShowResult.Finished);
            });
        }
    }
}

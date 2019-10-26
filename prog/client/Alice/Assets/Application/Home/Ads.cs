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
        UserChest cacheChest;
        Action<ShowResult> cb;
        // Start is called before the first frame update
        void Start()
        {
            Instance = this;
            blocker = GetComponent<Image>();
            blocker.gameObject.SetActive(false);
        }

        /// <summary>
        /// 広告表示
        /// </summary>
        /// <param name=""></param>
        public void Show(UserChest chest, Action<ShowResult> cb)
        {
            blocker.gameObject.SetActive(true);
            this.cacheChest = chest;
            this.cb = cb;

            if (!Advertisement.IsReady()) return;

            Advertisement.Show(new ShowOptions { resultCallback = (showResult)=>
            {
                if(showResult == ShowResult.Finished)
                {
                    Finished();
                }
                else
                {
                    blocker.enabled = false;
                    cb?.Invoke(showResult);
                }
            }});
        }

        public void Finished()
        {
            blocker.gameObject.SetActive(false);

            var c2v = new AdsSend();
            c2v.token = UserData.cacheHomeRecv.player.token;
            c2v.chest = this.cacheChest;

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

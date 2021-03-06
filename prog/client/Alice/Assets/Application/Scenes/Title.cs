﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Zoo.Auth;
using Zoo;
using Zoo.Communication;
using Alice.Entities;
using UnityEngine.Advertisements;
using System;
using UnityEngine.Purchasing;
using Zoo.Sound;

namespace Alice
{
    public class Title : MonoBehaviour
    {
        void Start()
        {
            SoundService.Instance.PlayBGM(Const.BGM.Home);

            // 初期化を実行
            Async.Parallel(() =>
            {
                GetHome((data) =>
                {
                    if (UserData.CacheHomeRecv(data))
                    {
                        SceneManager.LoadScene("Home");
                    }
                });
            },
            (end) => AuthService.Instance.SignInAnonymously(end),
            (end) => MasterData.Initialize(end),
            (end) => StartCoroutine(InitializeAds(end))
            );
        }

        /// <summary>
        /// 広告APIの初期化
        /// </summary>
        IEnumerator InitializeAds(Action cb)
        {
#if UNITY_IOS
            var id = "3335719";
#else
            var id = "3335718";
#endif
#if UNITY_ADS
            Advertisement.Initialize(id);
            // Ads の初期化待ち
            while (!Advertisement.isInitialized || !Advertisement.IsReady())
            {
                yield return null;
            }
#else
            yield return null;
#endif
            cb?.Invoke();
        }

        void GetHome(Action<HomeRecv> cb)
        {
            // ホーム情報を取得し、シーンを遷移する
            CommunicationService.Instance.Request("Home", "", (res) =>
            {
                var data = JsonUtility.FromJson<HomeRecv>(res);
                if (data.waitCreate)
                {
                    // 再帰呼び出し
                    GetHome(cb);
                }
                else
                {
                    cb?.Invoke(data);
                }
            });
        }
    }
}

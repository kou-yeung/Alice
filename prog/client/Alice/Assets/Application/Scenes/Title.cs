using System.Collections;
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

namespace Alice
{
    public class Title : MonoBehaviour
    {
        public Text wait;
        public void OnScreenButton()
        {
            // 通信状況を確認する
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                PlatformDialog.SetButtonLabel("OK");
                PlatformDialog.Show(
                    "警告",
                    "ネット接続できません。通信環境を確認してください",
                    PlatformDialog.Type.SubmitOnly,
                    () =>
                    {
                        Debug.Log("OK");
                    }
                );
                return;
            }
            Async.Parallel(() =>
            {
                // ホーム情報を取得し、シーンを遷移する
                CommunicationService.Instance.Request("Home", "", (res) =>
                {
                    UserData.CacheHomeRecv(JsonUtility.FromJson<HomeRecv>(res));
                    SceneManager.LoadScene("Home");
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
#if UNITY_ADS
            // https://github.com/unity3d-jp/unityads-help-jp/wiki/Integration-Guide-for-Unity
            //Advertisement.Initialize("2788195");  // MEMO : 必要なくなったかも？
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
    }
}

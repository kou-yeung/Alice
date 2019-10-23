using UnityEngine;
using UnityEngine.SceneManagement;
using Zoo.Auth;
using Zoo.Communication;
using Zoo.IO;
using Alice.Entities;
using Zoo;
using UnityEngine.Advertisements;
using System;
using System.Collections;

namespace Alice
{
    public class Boot : MonoBehaviour
    {
        public enum Backend
        {
            Local,
            Firebase,
        }

        public Backend backend = Backend.Local;

        void Start()
        {
            InitializeServiceLocator();

            Async.Parallel(() =>
            {
                // ホーム情報を取得し、シーンを遷移する
                CommunicationService.Instance.Request("Home", "", (res) =>
                {
                    UserData.CacheHomeRecv(JsonUtility.FromJson<HomeRecv>(res));
                    SceneManager.LoadSceneAsync("Home");
                });
            },
                (end) => AuthService.Instance.SignInAnonymously(end),
                (end) => MasterData.Initialize(end),
                (end) => StartCoroutine(InitializeAds(end))
            );
        }

        // サービスロケータの初期化
        void InitializeServiceLocator()
        {
            Debug.Log("サービスロケータの初期化");
            // ローダー
            LoaderService.SetLocator(new LoaderAddressableAssets("Assets/AddressableAssets/"));

            // サーババックエンド
            switch (backend)
            {
                case Backend.Local:
                    AuthService.SetLocator(new AuthLocal());    // 認証
                    CommunicationService.SetLocator(new CommunicationLocal(new Alice.AliceServer()));    // 通信
                    break;
                case Backend.Firebase:
                    AuthService.SetLocator(new AuthFirebase());    // 認証
                    CommunicationService.SetLocator(new CommunicationFirebase("alice-321c1"));    // 通信
                    break;
            }
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

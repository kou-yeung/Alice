using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zoo.Auth;
using Zoo.Communication;
using Zoo.IO;
using Zoo.Crypto;
using Zoo;
using Zoo.Sound;

namespace Alice
{
    public class Boot : MonoBehaviour
    {
        public enum Backend
        {
            Local,
            Firebase,
        }
        public enum Scene
        {
            Title,
            MasterDataUploader
        }

        public Backend backend = Backend.Local;
        public Scene scene = Scene.Title;
        public bool enableSound = true;

        public GameObject[] poolPrefabs;

        void Start()
        {
            InitializeServiceLocator();

            // ScreenBlockセットアップ:通信
            CommunicationService.Crypto = new CryptoBase64();
            CommunicationService.ConnectionBegin = ()=> { ScreenBlocker.Instance?.Push(); };
            CommunicationService.ConnectionEnd = () => { ScreenBlocker.Instance?.Pop(); };
            CommunicationService.WarningMessage = (message) =>
            {
                Dialog.Show(message.TextData(), Dialog.Type.SubmitOnly);
            };
            CommunicationService.ErrorMessage = (message) =>
            {
                Dialog.Show(message.TextData(), Dialog.Type.SubmitOnly, ()=> {
                    SceneManager.LoadSceneAsync(scene.ToString());
                });
            };

            // 登録
            foreach(var prefab in poolPrefabs)
            {
                PrefabPool.Regist(prefab.name, prefab);
            }
            SceneManager.LoadSceneAsync(scene.ToString());
        }

        // サービスロケータの初期化
        void InitializeServiceLocator()
        {
            Debug.Log("サービスロケータの初期化");
            // ローダー
            LoaderService.SetLocator(new LoaderAddressableAssets("Assets/AddressableAssets/"));

            // Sound
            if(enableSound)
            {
                SoundService.SetLocator(new SoundClip());
            } else
            {
                SoundService.SetLocator(new SoundMute());
            }
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
    }
}

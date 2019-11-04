using UnityEngine;
using UnityEngine.SceneManagement;
using Zoo.Auth;
using Zoo.Communication;
using Zoo.IO;
using Zoo.Crypto;

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

        void Start()
        {
            InitializeServiceLocator();

            // ScreenBlockセットアップ:通信
            CommunicationService.Crypto = new CryptoBase64();
            CommunicationService.ConnectionBegin = ()=> { ScreenBlocker.Instance?.Push(); };
            CommunicationService.ConnectionEnd = () => { ScreenBlocker.Instance?.Pop(); };
            CommunicationService.WarningMessage = (message) =>
            {
                PlatformDialog.SetButtonLabel("OK");
                PlatformDialog.Show( "確認", message, PlatformDialog.Type.SubmitOnly,
                    () => {}
                );
            };
            CommunicationService.ErrorMessage = (message) =>
            {
                PlatformDialog.SetButtonLabel("OK");
                PlatformDialog.Show("エラー", message, PlatformDialog.Type.SubmitOnly,
                    () => {
                        SceneManager.LoadSceneAsync(scene.ToString());
                    }
                );
            };
            SceneManager.LoadSceneAsync(scene.ToString());
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
    }
}

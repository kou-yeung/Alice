using UnityEngine;
using UnityEngine.SceneManagement;
using Zoo.Auth;
using Zoo.Communication;
using Zoo.IO;
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

            // ScreenBlockセットアップ:通信
            CommunicationService.ConnectionBegin = ()=> { ScreenBlocker.Instance.Push(); };
            CommunicationService.ConnectionEnd = () => { ScreenBlocker.Instance.Pop(); };
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
                        SceneManager.LoadSceneAsync("Title");
                    }
                );
            };
            SceneManager.LoadSceneAsync("Title");
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

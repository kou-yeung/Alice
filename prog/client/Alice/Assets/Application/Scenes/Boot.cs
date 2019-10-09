using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zoo.Auth;
using Zoo.Communication;
using Zoo.IO;

public class Boot : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // タイトルへ
        SceneManager.LoadSceneAsync("Title");
    }

    // サービスロケータの初期化
    [RuntimeInitializeOnLoadMethod]
    static void InitializeServiceLocator()
    {
        Debug.Log("サービスロケータの初期化");
        AuthService.SetLocator(new AuthLocal());    // 認証
        CommunicationService.SetLocator(new CommunicationLocal(new AliceServer()));    // 通信
        LoaderService.SetLocator(new LoaderAddressableAssets("Assets/AddressableAssets/"));    // ローダー
    }
}

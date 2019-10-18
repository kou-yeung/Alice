using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zoo.Auth;
using Zoo.Communication;
using Zoo.IO;
using Alice.Entities;
using Zoo;
public class Boot : MonoBehaviour
{
    public string nextScene;

    void Start()
    {
        Async.Parallel(() =>
        {
            if(!string.IsNullOrEmpty(nextScene))
            {
                SceneManager.LoadSceneAsync(nextScene);
            }
        },
            (end) => MasterData.Initialize(end)
        );
    }

    // サービスロケータの初期化
    [RuntimeInitializeOnLoadMethod]
    static void InitializeServiceLocator()
    {
        Debug.Log("サービスロケータの初期化");
        AuthService.SetLocator(new AuthLocal());    // 認証
        CommunicationService.SetLocator(new CommunicationLocal(new Alice.AliceServer()));    // 通信
        LoaderService.SetLocator(new LoaderAddressableAssets("Assets/AddressableAssets/"));    // ローダー
    }
}

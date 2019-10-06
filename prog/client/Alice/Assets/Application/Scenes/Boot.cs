using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zoo.Auth;
using Zoo.Communication;

public class Boot : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // サービスロケータの初期化
        AuthService.SetLocator(new AuthLocal());    // 認証
        CommunicationService.SetLocator(new CommunicationLocal(new AliceServer()));    // 通信


        // タイトルへ
        SceneManager.LoadSceneAsync("Title");
    }
}

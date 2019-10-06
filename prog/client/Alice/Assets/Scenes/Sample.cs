using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;
using System.Linq;
using Zoo.Auth;
using Zoo.Communication;

public class Sample : MonoBehaviour
{
    /// <summary>
    /// ローカルサーバー
    /// </summary>
    class LocalServer : ICommunication
    {
        public void Request(string proto, string data, Action<string> complete = null, Action<string> error = null)
        {
            switch(proto)
            {
                case "GetTime":
                    complete?.Invoke(DateTime.Now.ToString());
                    break;
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        AuthService.SetLocator(new AuthFirebase());
        CommunicationService.SetLocator(new CommunicationFirebase("alice-321c1"));

        AuthService.Instance.SignInAnonymously(() =>
        {
            // 成功
            Debug.Log("成功");
            //CommunicationService.Instance.Request("ping", "from unity!!", (res) => Debug.Log(res), error => Debug.LogError(error));
            CommunicationService.Instance.Request("getItems", "from unity!!", (res) => Debug.Log(res), error => Debug.LogError(error));
        },
        (error) =>
        {
            Debug.Log(error);
        });


    }

    // Update is called once per frame
    void Update()
    {
    }

    public void OnButton()
    {
        CommunicationService.Instance.Request("getItems", "from unity!!", (res) => Debug.Log(res), error => Debug.LogError(error));

    }
}

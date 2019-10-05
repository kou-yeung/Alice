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
        AuthService.Instance.SignInAnonymously(() =>
        {
            Debug.Log("成功!!");
        },
        (error) =>
        {
            Debug.Log(error);
        });


        CommunicationService.SetLocator(new LocalServer());
        CommunicationService.Instance.Request("GetTime", "", (res) => Debug.Log(res));
    }

    // Update is called once per frame
    void Update()
    {
    }
}

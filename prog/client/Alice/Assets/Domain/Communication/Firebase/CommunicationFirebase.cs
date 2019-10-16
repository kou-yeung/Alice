using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.Networking;
using Firebase.Functions;

namespace Zoo.Communication
{
    public class CommunicationFirebase : ICommunication
    {
        const string UrlFormat = @"https://us-central1-{0}.cloudfunctions.net/";
        string baseUrl { get; set; }

        public CommunicationFirebase(string projectName)
        {
            this.baseUrl = string.Format(UrlFormat, projectName);
        }

        /// <summary>
        /// リクエスト送信
        /// </summary>
        /// <param name="proto"></param>
        /// <param name="data"></param>
        /// <param name="complete"></param>
        /// <param name="error"></param>
        public void Request(string proto, string data, Action<string> complete = null, Action<string> error = null)
        {
            Debug.Log($"Request proto({proto}) data({data})");

            var functions = FirebaseFunctions.DefaultInstance;
            
            functions.GetHttpsCallable(proto).CallAsync(data).ContinueWith(task =>
            {
                // エラー処理
                if (task.IsCanceled)
                {
                    error?.Invoke($"Request:{proto}[{data}] was Canceled!!");
                    return;
                }
                if (task.IsFaulted)
                {
                    error?.Invoke($"Request:{proto}[{data}] was Faulted!! {task.Exception}");
                    return;
                }
                complete?.Invoke(task.Result.Data as string);
            });
        }

        IEnumerator Fetch(IObserver<string> observer, UnityWebRequest request)
        {
            yield return request.SendWebRequest();

            if(request.error != null)
            {
                observer.OnError(new Exception(request.error));
            } else
            {
                observer.OnNext(request.downloadHandler.text);
                observer.OnCompleted();
            }
        }
    }
}


//https://us-central1-alice-321c1.cloudfunctions.net/helloWorld


using System;
using UnityEngine;
using Firebase.Functions;
using Firebase.Extensions;

namespace Zoo.Communication
{
    class Message
    {
        public string error;
        public string warning;
    }

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
            CommunicationService.ConnectionBegin?.Invoke();

            Debug.Log($"Request proto({proto}) data({data})");

            var functions = FirebaseFunctions.DefaultInstance;

            data = CommunicationService.Crypto.Encrypt(data);
            functions.GetHttpsCallable(proto).CallAsync(data).ContinueWithOnMainThread(task =>
            {
                CommunicationService.ConnectionEnd?.Invoke();

                // エラー処理
                if (task.IsCanceled)
                {
                    CommunicationService.WarningMessage?.Invoke("通信できませんでした");
                    error?.Invoke($"Request:{proto}[{data}] was Canceled!!");
                    return;
                }
                if (task.IsFaulted)
                {
                    CommunicationService.WarningMessage?.Invoke("通信できませんでした");
                    error?.Invoke($"Request:{proto}[{data}] was Faulted!! {task.Exception}");
                    return;
                }

                var recv = CommunicationService.Crypto.Decrypt(task.Result.Data as string);
                Debug.Log($"Recv {recv}");

                var message = JsonUtility.FromJson<Message>(recv);
                if (!string.IsNullOrEmpty(message.error))
                {
                    CommunicationService.ErrorMessage?.Invoke(message.error);
                }
                else if(!string.IsNullOrEmpty(message.warning))
                {
                    CommunicationService.WarningMessage?.Invoke(message.warning);
                } else
                {
                    complete?.Invoke(recv);
                }
            });
        }
    }
}


//https://us-central1-alice-321c1.cloudfunctions.net/helloWorld


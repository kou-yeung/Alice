using System;
using Firebase.Auth;
using Firebase.Extensions;
using UnityEngine;

namespace Zoo.Auth
{
    public class AuthFirebase : IAuth
    {
        public void SignInAnonymously(Action complete = null, Action<string> error = null)
        {
            var auth = FirebaseAuth.DefaultInstance;
            auth.SignInAnonymouslyAsync().ContinueWithOnMainThread(task =>
            {
                // エラー処理
                if (task.IsCanceled)
                {
                    error?.Invoke("SignInAnonymously was Canceled!!");
                    return;
                }
                if(task.IsFaulted)
                {
                    error?.Invoke($"SignInAnonymously was Faulted!! {task.Exception}");
                    return;
                }
                // 成功処理
                var user = task.Result;
                Debug.Log($"User signed in successfully. name:[{user.DisplayName}] id:[{user.UserId}]");

                complete?.Invoke();
            });
        }
    }
}

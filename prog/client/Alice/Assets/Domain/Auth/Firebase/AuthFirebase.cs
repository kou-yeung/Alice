using System;
using Firebase.Auth;
using UnityEngine;

namespace Zoo.Auth
{
    public class AuthFirebase : IAuth
    {
        public void SignInAnonymously(Action complete = null, Action<string> error = null)
        {
            var auth = FirebaseAuth.DefaultInstance;
            auth.SignInAnonymouslyAsync().ContinueWith(task =>
            {
                // エラー処理
                if (error != null)
                {
                    if (task.IsCanceled)
                    {
                        error("SignInAnonymously was Canceled!!");
                        return;
                    }
                    if(task.IsFaulted)
                    {
                        error($"SignInAnonymously was Faulted!! {task.Exception}");
                        return;
                    }
                }

                // 成功処理
                var user = task.Result;
                Debug.Log($"User signed in successfully. name:[{user.DisplayName}] id:[{user.UserId}]");
                complete?.Invoke();
            });
        }
    }
}

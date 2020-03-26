using System;
using Firebase.Auth;
using Firebase.Extensions;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
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

        public void SignOut()
        {
            FirebaseAuth.DefaultInstance.SignOut();
        }

#if UNITY_EDITOR
        [MenuItem("Auth/Firebase/Clear")]
        public static void Clear()
        {
            FirebaseAuth.DefaultInstance.SignOut();
        }
        [MenuItem("Auth/Firebase/SignIn")]
        public static void SignIn()
        {
            new AuthFirebase().SignInAnonymously();
        }
#endif
    }
}

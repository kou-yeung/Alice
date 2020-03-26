using System;

namespace Zoo.Auth
{
    public class AuthLocal : IAuth
    {
        public void SignInAnonymously(Action complete = null, Action<string> error = null)
        {
            // 必ず成功にする
            complete?.Invoke();
        }

        public void SignOut()
        {
            // 空き実装
        }
    }
}

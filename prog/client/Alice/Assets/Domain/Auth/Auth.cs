using System;

namespace Zoo
{
    public interface IAuth
    {
        //void Login(Action complete = null, Action<string> error = null);
        void SignInWithEmailAndPassword(string email, string password, Action complete = null, Action<string> error = null);
    }
    /// <summary>
    /// 認証サービスを提供する
    /// </summary>
    public class AuthService : ServiceLocator<IAuth> { }
}

﻿using System;

namespace Zoo.Auth
{
    public interface IAuth
    {
        void SignInAnonymously(Action complete = null, Action<string> error = null);
    }
    /// <summary>
    /// 認証サービスを提供する
    /// </summary>
    public class AuthService : ServiceLocator<IAuth>{}
}

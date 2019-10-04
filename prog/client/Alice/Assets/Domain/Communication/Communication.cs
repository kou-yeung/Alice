using System;

namespace Zoo.Communication
{
    public interface ICommunication
    {
        void Request(string proto, string data, Action<string> complete = null, Action<string> error = null);
    }

    /// <summary>
    /// コミュニケーションサービスを提供する
    /// </summary>
    public class CommunicationService : ServiceLocator<ICommunication> {}
}

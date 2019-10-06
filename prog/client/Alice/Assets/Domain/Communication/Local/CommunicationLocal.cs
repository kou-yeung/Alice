using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Zoo.Communication
{
    public interface IDummyServer
    {
        void Call(string proto, string data, Action<string> complete = null, Action<string> error = null);
    }

    public class CommunicationLocal : ICommunication
    {
        IDummyServer server;
        public CommunicationLocal(IDummyServer server)
        {
            this.server = server;
        }

        public void Request(string proto, string data, Action<string> complete = null, Action<string> error = null)
        {
            server?.Call(proto, data, complete, error);
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zoo.Communication;

public class AliceServer : IDummyServer
{
    public void Call(string proto, string data, Action<string> complete = null, Action<string> error = null)
    {
        switch(proto)
        {
            case "getItems":
                complete?.Invoke(getItems(data));
                break;
        }
    }

    string getItems(string data)
    {
        return "ITEM_001_001";
    }
}

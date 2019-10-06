using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;
using System.Linq;
using Zoo.Auth;
using Zoo.Communication;

public class Sample : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void OnButton()
    {
        CommunicationService.Instance.Request("getItems", "from unity!!", (res) => Debug.Log(res), error => Debug.LogError(error));

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zoo.Auth;

public class Title : MonoBehaviour
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
        AuthService.Instance.SignInAnonymously(() =>
        {
            SceneManager.LoadSceneAsync("SampleScene");
        });
    }
}

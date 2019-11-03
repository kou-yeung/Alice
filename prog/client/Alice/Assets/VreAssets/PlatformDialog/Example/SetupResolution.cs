using UnityEngine;
using System.Collections;

public class SetupResolution : MonoBehaviour {

	// Use this for initialization
	void Start () {
		float screenRate = (float)1024 / Screen.height;
		if( screenRate > 1 ) screenRate = 1;
		int width = (int)(Screen.width * screenRate);
		int height = (int)(Screen.height * screenRate);

		Screen.SetResolution( width , height, true, 15);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}

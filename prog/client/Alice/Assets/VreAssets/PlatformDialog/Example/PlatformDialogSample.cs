using UnityEngine;
using System.Collections;

public class PlatformDialogSample : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnGUI() {
		if( GUILayout.Button ("show dialog 1") ) {
			PlatformDialog.SetButtonLabel("OK", "Cancel");
			PlatformDialog.Show(
				"message", 
				PlatformDialog.Type.OKCancel, 
				() => {
					Debug.Log("OK");
				},
				() => {
					Debug.Log("Cancel");
				}
			);

			StartCoroutine( AutoClose() );
		}

		if( GUILayout.Button ("show dialog 2") ) {
			PlatformDialog.SetButtonLabel("Yes", "No");
			PlatformDialog.Show(
				"title",
				"message", 
				PlatformDialog.Type.OKCancel, 
				() => {
					Debug.Log("Yes");
				},
				() => {
					Debug.Log("No");
				}
			);
		}

		if( GUILayout.Button ("show dialog 3") ) {
			PlatformDialog.SetButtonLabel("OK");
			PlatformDialog.Show(
				"title",
				"message", 
				PlatformDialog.Type.SubmitOnly, 
				() => {
					Debug.Log("OK");
				},
				null
			);
		}
	}

	private IEnumerator AutoClose() {
		yield return new WaitForSeconds( 10.0f );
		PlatformDialog.Dismiss();
	}
}

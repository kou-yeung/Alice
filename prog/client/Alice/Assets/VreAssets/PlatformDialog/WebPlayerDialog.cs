#if UNITY_WEBPLAYER
using UnityEngine;
using System.Collections;

public class WebPlayerDialog : IDialogInterface {
	private string javascriptInitialize = @"
	";

	private string javascriptConfirm = @"
		var unity = u.getUnity();
		if(window.confirm('MESSAGE')){
			unity.SendMessage('PlatformDialog', 'OnPositive', 'positive');
		}
		else{
			unity.SendMessage('PlatformDialog', 'OnNegative', 'positive');
		}";

	private string javascriptAlert = @"
		var unity = u.getUnity();
		if(window.alert('MESSAGE')){
			unity.SendMessage('PlatformDialog', 'OnPositive', 'positive');
		}
		else{
			unity.SendMessage('PlatformDialog', 'OnNegative', 'positive');
		}";

	public WebPlayerDialog() {
		// Application.ExternalEval( javascriptInitialize );
	}

	public void Show (string message, PlatformDialog.Type buttonType) {
		if( buttonType == PlatformDialog.Type.SubmitOnly ) {
			Application.ExternalEval( javascriptAlert.Replace("MESSAGE", message) );
		}
		else {
			Application.ExternalEval( javascriptConfirm.Replace("MESSAGE", message) );
		}
	}

	public void Show (string title, string message, PlatformDialog.Type buttonType) {
		Show(message, buttonType);
	}

	public void SetButtonLabel( string positive ) {

	}

	public void SetButtonLabel( string positive, string negative ) {

	}

	public void Dismiss() {

	}
}
#endif

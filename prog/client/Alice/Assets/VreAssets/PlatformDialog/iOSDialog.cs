using UnityEngine;
using System;
using System.Collections;
using System.Runtime.InteropServices;

public class iOSDialog : IDialogInterface {
#if UNITY_IPHONE && !UNITY_EDITOR
	[DllImport("__Internal")]
    private static extern void DialogShowWithMessage (string message, int buttonType);
	[DllImport("__Internal")]
	private static extern void DialogShowWithTitle (string title, string message, int buttonType);
	[DllImport("__Internal")]
	private static extern void DialogSetButtonLabel( string positive );
	[DllImport("__Internal")]
	private static extern void DialogSetButtonLabels( string positive, string negative );
	[DllImport("__Internal")]
	private static extern void DialogDismiss();
	
	

	public void Show (string message, PlatformDialog.Type buttonType) {
		DialogShowWithMessage(message, buttonType.ToInt() );
	}

	public void Show (string title, string message, PlatformDialog.Type buttonType) {
		DialogShowWithTitle( title, message, buttonType.ToInt() );
	}

	public void SetButtonLabel( string positive ) {
		DialogSetButtonLabel(positive);
	}
	public void SetButtonLabel( string positive, string negative ) {
		DialogSetButtonLabels(positive,negative);
	}

	public void Dismiss() {
		DialogDismiss();
	}
#else
	public void Show (string message, PlatformDialog.Type buttonType) {}
	public void Show (string title, string message, PlatformDialog.Type buttonType) {}
	public void SetButtonLabel( string positive ) {}
	public void SetButtonLabel( string positive, string negative ) {}
	public void Dismiss() {}
#endif
}

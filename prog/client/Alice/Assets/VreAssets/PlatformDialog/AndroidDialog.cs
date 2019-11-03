using UnityEngine;
using System.Collections;

public class AndroidDialog : IDialogInterface {
#if UNITY_ANDROID && !UNITY_EDITOR
	public void Show ( string message, PlatformDialog.Type buttonType ) {
		using (AndroidJavaClass javaClass = new AndroidJavaClass("unity.plugins.dialog.NativeDialog")) {
			javaClass.CallStatic("ShowDialog", message, buttonType.ToInt());
        }
	} 

	public void Show ( string title, string message, PlatformDialog.Type buttonType ) {
		using (AndroidJavaClass javaClass = new AndroidJavaClass("unity.plugins.dialog.NativeDialog")) {
			javaClass.CallStatic("ShowDialog", title, message, buttonType.ToInt());
        }
	}

	public void SetButtonLabel( string positive ) {
		using (AndroidJavaClass javaClass = new AndroidJavaClass("unity.plugins.dialog.NativeDialog")) {
			javaClass.CallStatic("SetButtonLabel", positive );
        }
	}

	public void SetButtonLabel( string positive, string negative ) {
		using (AndroidJavaClass javaClass = new AndroidJavaClass("unity.plugins.dialog.NativeDialog")) {
			javaClass.CallStatic("SetButtonLabel", positive, negative );
        }
	}

	public void Dismiss() {
		using (AndroidJavaClass javaClass = new AndroidJavaClass("unity.plugins.dialog.NativeDialog")) {
			javaClass.CallStatic("Dismiss");
        }
	}
#else
	public void Show (string message, PlatformDialog.Type buttonType) {}
	public void Show (string title, string message, PlatformDialog.Type buttonType) {}
	public void SetButtonLabel( string positive ) {}
	public void SetButtonLabel( string positive, string negative ) {}
	public void Dismiss() {}
#endif
}

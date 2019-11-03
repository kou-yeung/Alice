using UnityEngine;
using System.Collections;

public interface IDialogInterface {
	void Show (string message, PlatformDialog.Type buttonType);
	void Show (string title, string message, PlatformDialog.Type buttonType);
	void SetButtonLabel( string positive );
	void SetButtonLabel( string positive, string negative );
	void Dismiss();
}

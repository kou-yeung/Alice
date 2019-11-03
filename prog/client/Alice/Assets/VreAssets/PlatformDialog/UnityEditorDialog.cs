#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections;

public class UnityEditorDialog : IDialogInterface {
	private string labelPositiveButton;
	private string labelNegativeButton;
	

	public void Show (string message, PlatformDialog.Type buttonType) {
		string labelPositive = null;
		string labelNegative = null;

		if( buttonType == PlatformDialog.Type.SubmitOnly ) {
			labelPositive = GetLabelPositiveButton();
		}
		else {
			labelPositive = GetLabelPositiveButton();
			labelNegative = GetLabelNegativeButton();
		}


		if( EditorUtility.DisplayDialog(" ", message, labelPositive, labelNegative ) ) {
			PlatformDialog.Instance.gameObject.SendMessage("OnPositive", "positive");
		}
		else {
			PlatformDialog.Instance.gameObject.SendMessage("OnNegative", "negative");
		}
	}

	public void Show (string title, string message, PlatformDialog.Type buttonType) {
		string labelPositive = null;
		string labelNegative = null;
		
		if( buttonType == PlatformDialog.Type.SubmitOnly ) {
			labelPositive = GetLabelPositiveButton();
		}
		else {
			labelPositive = GetLabelPositiveButton();
			labelNegative = GetLabelNegativeButton();
		}

		if( EditorUtility.DisplayDialog(title, message, labelPositive, labelNegative ) ) {
			PlatformDialog.Instance.gameObject.SendMessage("OnPositive", "positive");
		}
		else {
			PlatformDialog.Instance.gameObject.SendMessage("OnNegative", "negative");
		}
	}

	public void SetButtonLabel( string positive ) {
		this.labelPositiveButton = positive;
	}

	public void SetButtonLabel( string positive, string negative ) {
		this.labelPositiveButton = positive;
		this.labelNegativeButton = negative;
	}
	public void Dismiss() {

	}

	private string GetLabelPositiveButton() {
		if( labelPositiveButton == null ) {
			return "OK";
		}

		return labelPositiveButton;
	}

	private string GetLabelNegativeButton() {
		if( labelNegativeButton == null ) {
			return "Cancel";
		}

		return labelNegativeButton;
	}
}
#endif

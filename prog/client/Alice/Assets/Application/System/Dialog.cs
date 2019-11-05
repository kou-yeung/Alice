using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Zoo;

namespace Alice
{
    public class Dialog : MonoBehaviour
    {
        public enum Type
        {
            SubmitOnly = 0,
            OKCancel = 1,
        }

        public RectTransform background;
        public Text message;
        public Button positive;
        public Button negative;

        Action positiveDelegate;
        Action negativeDelegate;

        public static void Show(string message, Type type, Action positiveDelegate = null, Action negativeDelegate = null)
        {
            var dialog = PrefabPool.Get("Dialog").GetComponent<Dialog>();
            dialog.positiveDelegate = positiveDelegate;
            dialog.negativeDelegate = negativeDelegate;
            dialog.message.text = message;
            dialog.negative.gameObject.SetActive(type == Type.OKCancel);

            // Canvasに追加する
            dialog.transform.SetParent(GameObject.Find("Canvas").transform, false);
            dialog.transform.SetAsLastSibling();

            Open(dialog);
        }

        /// <summary>
        /// 開く演出
        /// </summary>
        static void Open(Dialog dialog)
        {
            dialog.background.localScale = Vector3.one * .75f;
            dialog.background.LeanScale(Vector3.one, 0.25f).setEaseOutBounce();
        }

        /// <summary>
        /// 閉じる演出
        /// </summary>
        static void Close(Dialog dialog)
        {
            dialog.background.LeanScale(Vector3.one * .8f, 0.2f).setEaseInBack().setOnComplete(() =>
            {
                PrefabPool.Release("Dialog", dialog.gameObject);
            });
        }

        /// <summary>
        /// OKボタンが押された
        /// </summary>
        public void OnPositive()
        {
            positiveDelegate?.Invoke();
            Close(this);
        }

        /// <summary>
        /// Cancelボタンが押された
        /// </summary>
        public void OnNegative()
        {

            negativeDelegate?.Invoke();
            Close(this);
        }
    }
}

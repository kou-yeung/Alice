using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Zoo;

namespace Alice
{
    public class Dialog : BaseDialog
    {
        public enum Type
        {
            SubmitOnly = 0,
            OKCancel = 1,
        }

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
            dialog.Open();
        }

        /// <summary>
        /// 閉じる
        /// </summary>
        protected override void OnClosed()
        {
            PrefabPool.Release("Dialog", this.gameObject);
            base.OnClosed();
        }

        /// <summary>
        /// OKボタンが押された
        /// </summary>
        public void OnPositive()
        {
            positiveDelegate?.Invoke();
            Close();
        }

        /// <summary>
        /// Cancelボタンが押された
        /// </summary>
        public void OnNegative()
        {
            negativeDelegate?.Invoke();
            Close();
        }
    }
}

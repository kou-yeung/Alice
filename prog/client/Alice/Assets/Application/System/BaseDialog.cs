using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Alice
{
    public class BaseDialog : MonoBehaviour
    {
        public RectTransform background;

        protected virtual void OnClosed()
        {
        }

        /// <summary>
        /// 開く演出
        /// </summary>
        protected void Open()
        {
            // Canvasに追加する
            this.transform.SetParent(GameObject.Find("Canvas").transform, false);
            this.transform.SetAsLastSibling();

            this.background.localScale = Vector3.one * .75f;
            this.background.LeanScale(Vector3.one, 0.25f).setEaseOutBounce();
        }

        /// <summary>
        /// 閉じる演出
        /// </summary>
        public void Close()
        {
            this.background.LeanScale(Vector3.one * .8f, 0.2f).setEaseInBack().setOnComplete(() =>
            {
                this.OnClosed();
            });
        }
    }
}

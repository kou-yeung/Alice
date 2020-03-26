using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zoo;

namespace Alice
{
    public class TutorialData
    {
        public string Desc;             // フォーカス前のテキスト:ダイアログ表示
        public string TargetButton;     // 対象ボタン
        public int flag;
    }

    public class TutorialDialog : BaseDialog
    {
        [SerializeField] GameObject arrowRoot;
        [SerializeField] Image arrow;

        Transform originParent;
        GameObject target;
        Button targetButton;
        LayoutGroup group;
        int siblingIndex;

        private void Start()
        {
            LeanTween.moveLocalY(arrow.gameObject, 10f, 0.5f).setEase(LeanTween.shake).setLoopCount(-1);
        }


        void OnTargetClick()
        {
            // イベント削除
            targetButton.onClick.RemoveListener(OnTargetClick);

            // 元の親に戻す
            target.transform.SetParent(originParent, true);
            target.transform.SetSiblingIndex(siblingIndex);

            // グループを戻す
            if (group) group.enabled = true;
            this.arrowRoot.transform.SetParent(this.transform);
            this.Close();
        }
        /// <summary>
        /// チュートリアル設定
        /// </summary>
        /// <param name="data"></param>
        void Setup(TutorialData data)
        {
            target = GameObject.Find(data.TargetButton);
            
            group = target.GetComponentInParent<LayoutGroup>();
            if(group && group.enabled)
            {
                group.enabled = false;
            } else
            {
                group = null;
            }

            // 元の親を保持し、自分を親にします
            originParent = target.transform.parent;
            siblingIndex = target.transform.GetSiblingIndex();

            target.transform.SetParent(this.transform, true);

            // 矢印を設定する
            var rt = this.arrowRoot.GetComponent<RectTransform>();
            rt.SetParent(target.transform, false);

            var size = target.GetComponent<RectTransform>().sizeDelta;
            rt.anchoredPosition = new Vector2(0, size.y / 2);

            // ボタンを取得し、イベントを
            targetButton = target.GetComponentInChildren<Button>();
            targetButton.onClick.AddListener(OnTargetClick);
        }

        public static void Show(TutorialData data)
        {
            if (!string.IsNullOrEmpty(data.Desc))
            {
                Dialog.Show(data.Desc, Dialog.Type.SubmitOnly, () =>
                {
                    var dialog = PrefabPool.Get(nameof(TutorialDialog)).GetComponent<TutorialDialog>();
                    dialog.Open();
                    dialog.Setup(data);
                });
            }
            else
            {
                var dialog = PrefabPool.Get(nameof(TutorialDialog)).GetComponent<TutorialDialog>();
                dialog.Open();
                dialog.Setup(data);
            }
        }

        /// <summary>
        /// 閉じる
        /// </summary>
        protected override void OnClosed()
        {
            PrefabPool.Release(nameof(TutorialDialog), this.gameObject);
            base.OnClosed();
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zoo;
using System;
using Zoo.Communication;

namespace Alice
{
    public class NameEditDialog : BaseDialog
    {
        public Button button;
        public Text message;

        Action onClose;
        public static void Show(Action onClose = null)
        {
            var dialog = PrefabPool.Get(nameof(NameEditDialog)).GetComponent<NameEditDialog>();
            dialog.Open();
            dialog.onClose = onClose;
        }

        protected override void OnClosed()
        {
            PrefabPool.Release(nameof(NameEditDialog), this.gameObject);
            base.OnClosed();
            onClose?.Invoke();
        }

        public void OnEditEnd(InputField input)
        {
            // NG ワードのチェック
            if (input.text.Trim().Length < 1)
            {
                message.text = "NAME_EDIT_ERROR".TextData();
                button.interactable = false;
            } else
            {
                message.text = "";

                var player = UserData.cacheHomeRecv.player;
                player.name = input.text;
                button.interactable = true;
            }
        }

        public void OnClickOK()
        {
            var player = UserData.cacheHomeRecv.player;
            player.AddTutorialFlag(Const.TutorialFlag.UserNameInput);

            var c2s = new SyncSend { player = player };
            // 同期する
            CommunicationService.Instance.Request("PlayerSync", JsonUtility.ToJson(c2s), (res) =>
            {
                UserData.Modify(JsonUtility.FromJson<SyncRecv>(res).modified);
                this.Close();
            });
        }
    }
}

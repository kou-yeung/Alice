using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zoo.Auth;
using Zoo.Communication;

namespace Alice
{
    public class Home : MonoBehaviour
    {
        public GameObject[] tab;

        private void Start()
        {
            var flag = (Const.TutorialFlag)UserData.cacheHomeRecv.player.tutorialFlag;
            if (!flag.HasFlag(Const.TutorialFlag.UserNameInput))
            {
                NameEditDialog.Show();
            }
        }

        /// <summary>
        /// フッターのタップをクリックした
        /// </summary>
        /// <param name="target"></param>
        public void OnClickTab(GameObject target)
        {
            foreach (var v in tab)
            {
                if (v == null) continue;
                v.SetActive(v == target);
            }
        }

        /// <summary>
        /// デバッグボタンをクリックした
        /// </summary>
        public void OnClickDebug()
        {
            AuthService.Instance.SignOut();
        }

        /// <summary>
        /// アプリ一時停止
        /// </summary>
        /// <param name="pause"></param>
        private void OnApplicationPause(bool pause)
        {
            if (!pause) return;
            // 実行したバトル履歴を保持する
            UserData.SaveBattleRecord();
            // 同期
            PlayerSync();
        }
        /// <summary>
        /// アプリ終了
        /// </summary>
        /// <param name="pause"></param>
        private void OnApplicationQuit()
        {
            // 実行したバトル履歴を保持する
            UserData.SaveBattleRecord();
        }

        /// <summary>
        /// チュートリアルフラグをチェック
        /// </summary>
        /// <param name="flag"></param>
        public void ShowTutorial(int index)
        {
            var flag = (Const.TutorialFlag)index;
            var flags = (Const.TutorialFlag)UserData.cacheHomeRecv.player.tutorialFlag;
            if (flags.HasFlag(flag)) return;

            switch (flag)
            {
                case Const.TutorialFlag.Room:
                    Dialog.Show("ROOM_TUTORIAL".TextData(), Dialog.Type.SubmitOnly);
                    break;
                case Const.TutorialFlag.Record:
                    Dialog.Show("RECORD_TUTORIAL".TextData(), Dialog.Type.SubmitOnly);
                    break;
                case Const.TutorialFlag.Shadow:
                    Dialog.Show("SHADOW_TUTORIAL".TextData(), Dialog.Type.SubmitOnly);
                    break;
            }
            // フラグ更新
            UserData.cacheHomeRecv.player.tutorialFlag = (int)(flags | flag);
            // 同期する
            PlayerSync();
        }

        /// <summary>
        /// プレイヤーデータ同期する
        /// </summary>
        void PlayerSync()
        {
            var c2s = new SyncSend { player = UserData.cacheHomeRecv.player };
            // 同期する
            CommunicationService.Instance.Request("PlayerSync", JsonUtility.ToJson(c2s), (res) =>
            {
                UserData.Modify(JsonUtility.FromJson<SyncRecv>(res).modified);
            });

        }
    }
}

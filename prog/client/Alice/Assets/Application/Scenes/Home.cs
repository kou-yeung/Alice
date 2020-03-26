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
            // チュートリアル実行確認
            ShowTutorial((int)Const.TutorialFlag.UserNameInput);
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
            var player = UserData.cacheHomeRecv.player;

            var flag = (Const.TutorialFlag)index;
            if (player.HasTutorialFlag(flag)) return;

            switch (flag)
            {
                case Const.TutorialFlag.UserNameInput:
                    NameEditDialog.Show(() =>
                    {
                        var tutorialData = new TutorialData { Desc = "バトルを実行してみよう", TargetButton = "Base/Battle" };
                        TutorialDialog.Show(tutorialData);
                    });
                    break;
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
            player.AddTutorialFlag(flag);
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

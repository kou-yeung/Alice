using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
        /// アプリ一時停止
        /// </summary>
        /// <param name="pause"></param>
        private void OnApplicationPause(bool pause)
        {
            if (!pause) return;
            // 実行したバトル履歴を保持する
            UserData.SaveBattleRecord();

            var c2s = new SyncSend { player = UserData.cacheHomeRecv.player };
            // 同期する
            CommunicationService.Instance.Request("PlayerSync", JsonUtility.ToJson(c2s), (res) =>
            {
                UserData.Modify(JsonUtility.FromJson<SyncRecv>(res).modified);
            });


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
    }
}

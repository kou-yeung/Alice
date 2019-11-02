using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zoo;

namespace Alice
{
    public class Header : MonoBehaviour
    {
        public static Header Instance { get; private set; }
        public InputField Name;
        public Text Rank;
        public Text Rate;
        public Text Alarm;
        public Text Ads;

        void Start()
        {
            Instance = this;
            Observer.AddObserver("HomeRecv", Setup);
            Setup();
        }

        private void OnDestroy()
        {
            Observer.RemoveObserver("HomeRecv", Setup);
        }

        /// <summary>
        /// セットアップ
        /// </summary>
        public void Setup()
        {
            var player = UserData.cacheHomeRecv.player;
            Name.text = player.name;
            Rank.text = $"{player.rank + 1}";

            if(player.todayBattleCount >= 10)
            {
                var max = player.todayBattleCount;
                var win = player.todayWinCount * 100;
                var rate = Mathf.FloorToInt(win/max);
                Rate.text = $"({rate}%)";
            }
            else
            {
                Rate.text = $"(集計中)";
            }

            Alarm.text = $"{player.alarm}";
            Ads.text = $"{player.ads}";
        }

        /// <summary>
        /// 名前が編集した
        /// </summary>
        /// <param name="str"></param>
        public void OnEndEdit(string str)
        {
            // NG ワードのチェック
            if(str.Length < 2)
            {
                PlatformDialog.SetButtonLabel("OK");
                PlatformDialog.Show(
                    "確認",
                    $"最低２文字が必要です",
                    PlatformDialog.Type.SubmitOnly,
                    () => {
                        Debug.Log("OK");
                    }
                );
                var player = UserData.cacheHomeRecv.player;
                Name.text = player.name;
                return;
            }
            UserData.EditPlayerName(str);
        }
    }
}

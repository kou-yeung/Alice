using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zoo;
using Zoo.Communication;
using Zoo.Time;

namespace Alice
{
    public class Header : MonoBehaviour
    {
        public static Header Instance { get; private set; }
        public InputField Name;
        public Text Rank;
        public Text Rate;
        public Image Remain;
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
            if(str.Length < 1)
            {
                Dialog.Show("NAME_EDIT_ERROR".TextData(), Dialog.Type.SubmitOnly);
                var player = UserData.cacheHomeRecv.player;
                Name.text = player.name;
                return;
            }
            UserData.EditPlayerName(str);
        }

        /// <summary>
        /// アラームをクリックした
        /// </summary>
        public void OnClickAlarm()
        {
            PurchasingDialog.Show();
        }

        /// <summary>
        /// 広告の説明文
        /// </summary>
        public void OnClickAds()
        {
            Dialog.Show("ADS_DESC".TextData(), Dialog.Type.SubmitOnly);
        }

        /// <summary>
        /// ランクの説明文
        /// </summary>
        public void OnClickRank()
        {
            var next = UserData.cacheHomeRecv.nextBonusTime;
            var remain = (int)Mathf.Max(0, next - ServerTime.CurrentUnixTime);

            var hh = remain / 3600;
            var mm = (remain % 3600) / 60;
            var ss = (remain % 60);

            var text = string.Format("RANK_DESC".TextData(), hh, mm, ss);
            Dialog.Show(text, Dialog.Type.SubmitOnly);
        }

        /// <summary>
        /// 更新
        /// </summary>
        private void Update()
        {
            // 次のランク判定更新をチェックする
            var next = UserData.cacheHomeRecv.nextBonusTime;
            float remain = Mathf.Max(0, next - ServerTime.CurrentUnixTime);
            // 4時間毎に更新する
            Remain.fillAmount = 1 - (remain / (4 * 3600));
        }
    }
}

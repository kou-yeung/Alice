using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zoo;
using Zoo.Communication;
using Alice.Entities;
using System.Linq;

namespace Alice
{
    public class ChestDialog : BaseDialog
    {
        public Chest chest;
        public Text alarm;

        /// <summary>
        /// 広告ボタンをクリックした
        /// </summary>
        public void OnClickAds()
        {
            var player = UserData.cacheHomeRecv.player;
            if (player.ads > 0)
            {
                // 広告
                Ads.Instance.Show(chest.cacheUserChest, (res) =>
                {
                    // TODO : 表示の情報を更新する?そのまま閉じる
                    // HACK : とりあえず現在は閉じる
                    Close();
                });
            }
            else
            {
                Dialog.Show("明日に使用回数が補充します", Dialog.Type.SubmitOnly);
            }
        }

        /// <summary>
        /// アラームボタンをクリックした
        /// </summary>
        public void OnClickAlarm()
        {
            var player = UserData.cacheHomeRecv.player;
            if (chest.cacheUserChest.NeedAlarmNum() <= player.alarm)
            {
                // アイテムを使って開きます
                var c2s = new ChestSend();
                c2s.chest = chest.cacheUserChest;

                // 開く
                CommunicationService.Instance.Request("Chest", JsonUtility.ToJson(c2s), (res) =>
                {
                    var s2c = JsonUtility.FromJson<ChestRecv>(res);
                    UserData.Modify(s2c.modified);

                    if (s2c.modified.unit.Length != 0)
                    {
                        Close();
                        UnitDialog.Show(s2c.modified.unit[0]);
                    }
                    if (s2c.modified.skill.Length != 0)
                    {
                        Close();
                        SkillDialog.Show(s2c.modified.skill[0]);
                    }
                });

            }
            else
            {
                PurchasingDialog.Show();
            }
        }

        public static void Show(UserChest chest)
        {
            var dialog = PrefabPool.Get("ChestDialog").GetComponent<ChestDialog>();
            dialog.chest.Setup(chest, false);
            dialog.alarm.text = chest.NeedAlarmNum().ToString();
            dialog.Open();
        }

        /// <summary>
        /// 閉じる演出
        /// </summary>
        protected override void OnClosed()
        {
            PrefabPool.Release("ChestDialog", this.gameObject);
            base.OnClosed();
        }
    }
}

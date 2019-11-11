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
    public class ChestDialog : MonoBehaviour
    {
        public Chest chest;
        public RectTransform background;

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
                    Close(this);
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
                        foreach (var unit in s2c.modified.unit)
                        {
                            name = MasterData.Instance.characters.First(v => v.ID == unit.characterId).Name;
                        }
                    }
                    if (s2c.modified.skill.Length != 0)
                    {
                        foreach (var skill in s2c.modified.skill)
                        {
                            name = MasterData.Instance.skills.First(v => v.ID == skill.id).Name;
                        }
                    }

                    Dialog.Show($"{name} を入手しました", Dialog.Type.SubmitOnly,
                        () => {
                            Close(this);
                        }
                    );
                });

            }
            else
            {
                Dialog.Show("TODO:アラーム補充ダイアログ", Dialog.Type.SubmitOnly);
            }
        }

        /// <summary>
        /// 閉じるボタンをクリックした
        /// </summary>
        public void OnClickClose()
        {
            Close(this);
        }

        public static void Show(UserChest chest)
        {
            var dialog = PrefabPool.Get("ChestDialog").GetComponent<ChestDialog>();
            dialog.chest.Setup(chest);
            Open(dialog);
        }
        /// <summary>
        /// 開く演出
        /// </summary>
        static void Open(ChestDialog dialog)
        {
            // Canvasに追加する
            dialog.transform.SetParent(GameObject.Find("Canvas").transform, false);
            dialog.transform.SetAsLastSibling();

            dialog.background.localScale = Vector3.one * .75f;
            dialog.background.LeanScale(Vector3.one, 0.25f).setEaseOutBounce();
        }

        /// <summary>
        /// 閉じる演出
        /// </summary>
        static void Close(ChestDialog dialog)
        {
            dialog.background.LeanScale(Vector3.one * .8f, 0.2f).setEaseInBack().setOnComplete(() =>
            {
                PrefabPool.Release("ChestDialog", dialog.gameObject);
            });
        }

    }
}

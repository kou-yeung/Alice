using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zoo;
using Zoo.Communication;
using Xyz.AnzFactory.UI;
using System;
using UnityEngine.Networking;

namespace Alice
{
    public class VS : MonoBehaviour, ANZListView.IDataSource, ANZListView.IActionDelegate
    {
        public Battle battle;
        public InputField input;
        public Button btnFind;
        public ANZListView shadowTable;
        public GameObject shadowItem;
        public Text roominfo;
        public Thumbnail[] thumbnails;

        ShadowListRecv list;

        private void Start()
        {
            shadowTable.DataSource = this;
            shadowTable.ActionDelegate = this;
            PrefabPool.Regist(shadowItem.name, shadowItem);

            // 初期は非表示する
            foreach (var thum in thumbnails)
            {
                thum.gameObject.SetActive(false);
            }
        }
        /// <summary>
        /// シャドウを生成する
        /// </summary>
        public void OnCreate()
        {
            var c2s = new ShadowCreateSend();
            CommunicationService.Instance.Request("CreateShadow", JsonUtility.ToJson(c2s), res =>
            {
                var s2c = JsonUtility.FromJson<ShadowCreateRecv>(res);
                UserData.editedUnit.Clear();    // 同期しました
                UserData.cacheHomeRecv.player.roomid = s2c.roomId;  // 
                // リスト情報を自前生成する
                list = new ShadowListRecv();
                list.enemies = new ShadowEnemy[] { };
                list.self = s2c.self;
                list.roomid = s2c.roomId;
                list.isActive = true;
                // 更新する
                SetupCreateTab();

                // 共有確認
                var idText = string.Format("{0:D5}", list.roomid);
                Dialog.Show(string.Format("TWITTER_CONFIRM".TextData(), idText), Dialog.Type.OKCancel, () =>
                {
                    string esctext = UnityWebRequest.EscapeURL(string.Format("TWITTER_TEXT".TextData(), idText));
                    string url = "https://twitter.com/intent/tweet?text=" + esctext;
                    //Twitter投稿画面の起動
                    Application.OpenURL(url);
                });
            });
        }

        public void OnBattle()
        {
            int roomid = 0;
            int.TryParse(input.text.TrimStart('0'), out roomid);
            var c2s = new ShadowBattleSend(roomid);
            CommunicationService.Instance.Request("BattleShadow", JsonUtility.ToJson(c2s), res =>
            {
                UserData.editedUnit.Clear();    // 同期しました
                var s2c = JsonUtility.FromJson<BattleStartRecv>(res);
                battle.Exec(s2c);
            });
        }


        /// <summary>
        /// ルームIDを変更しました
        /// </summary>
        /// <param name="input"></param>
        public void OnEndEdit(InputField input)
        {
            // ５文字以上入力したら有効
            btnFind.interactable = input.text.Length >= 5;
        }

        public int NumOfItems()
        {
            if (UserData.cacheHomeRecv.player.roomid == -1) return 0;
            if (list == null) return 0;
            return list.enemies.Length;
        }

        public float ItemSize()
        {
            return shadowItem.GetComponent<RectTransform>().sizeDelta.y;
        }

        public GameObject ListViewItem(int index, GameObject item)
        {
            if (item == null)
            {
                item = PrefabPool.Get(shadowItem.name);
            }
            item.GetComponent<ShadowItem>().Setup(list.enemies[index]);
            return item;
        }
        public void TapListItem(int index, GameObject listItem)
        {
            // バトルの受信データ構築して再生する
            battle.Exec(BattleStartRecv.Conversion(list.self, list.enemies[index]));
        }

        /// <summary>
        /// シャドウ一覧タップを表示するとき
        /// </summary>
        public void OnShowShadowTab(Toggle toggle)
        {
            if (!toggle.isOn) return;
            if (UserData.cacheHomeRecv.player.roomid == -1)
            {
                SetupCreateTab();
                return;
            }

            var c2s = new ShadowListSend();
            c2s.roomid = UserData.cacheHomeRecv.player.roomid;
            CommunicationService.Instance.Request("ShadowList", JsonUtility.ToJson(c2s), (res) =>
            {
                list = JsonUtility.FromJson<ShadowListRecv>(res);
                SetupCreateTab();
            });
        }

        /// <summary>
        /// 
        /// </summary>
        void SetupCreateTab()
        {
            // 現在の情報を一旦保持する
            var deck = UserData.cacheHomeRecv.deck;
            var units = UserData.cacheHomeRecv.units;

            if (list != null)
            {
                deck = list.self.deck;
                units = list.self.unit;
                var active = list.isActive ? "有効" : "無効";
                var roomid = string.Format("{0:D5}", list.roomid);
                roominfo.text = $"シャドウID:<color=red>{roomid}</color> ({active})";
            }
            else
            {
                roominfo.text = "このデッキで登録しましょう";
            }

            // デッキアイコンを設定する
            for (int i = 0; i < deck.ids.Length; i++)
            {
                var id = deck.ids[i];
                if (string.IsNullOrEmpty(id))
                {
                    thumbnails[i].gameObject.SetActive(false);
                }
                else
                {
                    thumbnails[i].gameObject.SetActive(true);
                    thumbnails[i].Setup(Array.Find(units, v => v.characterId == id), true);
                }
            }
            shadowTable.ReloadData();
        }
    }
}

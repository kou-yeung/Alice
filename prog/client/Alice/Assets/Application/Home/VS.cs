using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zoo;
using Zoo.Communication;
using Xyz.AnzFactory.UI;

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

        private void Start()
        {
            shadowTable.DataSource = this;
            shadowTable.ActionDelegate = this;
            PrefabPool.Regist(shadowItem.name, shadowItem);
        }
        /// <summary>
        /// シャドウを生成する
        /// </summary>
        public void OnCreate()
        {
            var c2s = new ShadowCreateSend();
            CommunicationService.Instance.Request("CreateShadow", JsonUtility.ToJson(c2s), res =>
            {
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
            return 20;
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
            return item;
        }
        public void TapListItem(int index, GameObject listItem)
        {
            Debug.Log("タップ:バトル開始");
        }

        /// <summary>
        /// シャドウ一覧タップを表示するとき
        /// </summary>
        public void OnShowShadowTab(Toggle toggle)
        {
            if (!toggle.isOn) return;
            if (UserData.cacheHomeRecv.player.roomid == -1) return;

            var c2s = new ShadowListSend();
            c2s.roomid = UserData.cacheHomeRecv.player.roomid;
            CommunicationService.Instance.Request("ShadowList", JsonUtility.ToJson(c2s), (res) =>
            {
                shadowTable.ReloadData();
            });
            //Debug.Log("サーバーから最新情報:最大50?件取得");
        }
    }
}

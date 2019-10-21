using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Xyz.AnzFactory.UI;
using Zoo;

namespace Alice
{
    public class Record : MonoBehaviour, ANZListView.IDataSource, ANZListView.IActionDelegate
    {
        public Battle battle;
        public ANZListView recordTable;
        public GameObject recordItemPrefab;

        void Start()
        {
            PrefabPool.Regist(RecordItem.PrefabKey, recordItemPrefab);
            recordTable.DataSource = this;
            recordTable.ActionDelegate = this;
        }

        /// <summary>
        /// リスト更新する
        /// </summary>
        public void ReloadData()
        {
            recordTable.ReloadData();
        }

        /// <summary>
        /// 履歴数
        /// </summary>
        /// <returns></returns>
        public int NumOfItems()
        {
            return UserData.GetBattleRecord().Count;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public float ItemSize()
        {
            var rectTransform = recordItemPrefab.GetComponent<RectTransform>();
            return rectTransform.sizeDelta.y;
        }

        /// <summary>
        /// 試合履歴アイテムx1の表示物が要求された
        /// </summary>
        /// <param name="index"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public GameObject ListViewItem(int index, GameObject item)
        {
            if (item == null)
            {
                item = RecordItem.Gen().gameObject;
            }
            var record = UserData.GetBattleRecord();
            item.GetComponent<RecordItem>().Setup(record[index]);
            return item;
        }

        /// <summary>
        /// タップした
        /// </summary>
        /// <param name="index"></param>
        /// <param name="listItem"></param>
        public void TapListItem(int index, GameObject listItem)
        {
            var record = UserData.GetBattleRecord();
            battle.Exec(record[index], true);
        }
    }
}

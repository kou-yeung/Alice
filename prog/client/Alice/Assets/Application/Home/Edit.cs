using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Xyz.AnzFactory.UI;
using Zoo;
using System;
using System.Linq;

namespace Alice
{
    public class Edit : MonoBehaviour, ANZCellView.IDataSource, ANZCellView.IActionDelegate
    {
        public GameObject prefab;
        public ANZCellView cellView;

        int editIndex = -1;

        void Start()
        {
            PrefabPool.Regist(prefab.name, prefab);
            cellView.DataSource = this;
            cellView.ActionDelegate = this;
        }

        /// <summary>
        /// 開く
        /// </summary>
        /// <param name="index">編集対象</param>
        public void Open(int index)
        {
            gameObject.SetActive(true);
            editIndex = index;
            cellView.ReloadData();
        }

        public GameObject CellViewItem(int index, GameObject item)
        {
            if(item == null)
            {
                item = PrefabPool.Get(prefab.name);
            }
            var unit = UserData.cacheHomeRecv.units[index];
            var thumbnail = item.GetComponent<Thumbnail>();
            thumbnail.Setup(unit);
            return item;
        }

        public Vector2 ItemSize()
        {
            var rect = prefab.GetComponent<RectTransform>();
            return rect.sizeDelta;
        }

        public int NumOfItems()
        {
            return UserData.cacheHomeRecv.units.Length;
        }

        /// <summary>
        /// 選択した
        /// </summary>
        /// <param name="index"></param>
        /// <param name="listItem"></param>
        public void TapCellItem(int index, GameObject listItem)
        {
            var unit = UserData.cacheHomeRecv.units[index];
            var decks = UserData.cacheHomeRecv.decks;

            var at = Array.FindIndex(decks, v => v.characterId == unit.characterId);
            if(at != -1)
            {
                // 場所だけ更新
                decks[at].position = editIndex;
            } else
            {
                var add = new UserDeck { characterId = unit.characterId, position = editIndex };
                UserData.cacheHomeRecv.decks = decks.Concat(new[] { add }).ToArray();
            }
            gameObject.SetActive(false);
            Observer.Notify("HomeRecv");
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Xyz.AnzFactory.UI;
using Zoo;
using System;
using System.Linq;
using UnityEngine.Assertions;

namespace Alice
{
    public class Edit : MonoBehaviour, ANZCellView.IDataSource, ANZCellView.IActionDelegate
    {
        public GameObject prefab;
        public ANZCellView cellView;

        public EditItem from;
        public EditItem to;
        public GameObject removeText;
        public Button btnOK;

        int editIndex = -1;
        UserUnit[] sortedUserUnit;  // ソート済のユニット一覧

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

            var deck = UserData.cacheHomeRecv.deck;

            editIndex = index;

            if(!string.IsNullOrEmpty(deck.ids[index]))
            {
                var unit = UserData.cacheHomeRecv.units.First(v => v.characterId == deck.ids[index]);
                from.Setup(unit);
            }
            else
            {
                from.Setup(null);
            }

            to.Setup(null);
            removeText.SetActive(false);
            btnOK.interactable = false;

            // 表示順を作る
            var workUnit = UserData.cacheHomeRecv.units.ToList();
            workUnit.Sort((a, b) =>
            {
                var aPosition = Array.IndexOf(deck.ids, a.characterId);
                var bPosition = Array.IndexOf(deck.ids, b.characterId);

                // セットされなければ、最大値にします
                if (aPosition == -1) aPosition = int.MaxValue;
                if (bPosition == -1) bPosition = int.MaxValue;

                // はずす対象なら最優先
                if (aPosition == editIndex) aPosition = -1;
                if (bPosition == editIndex) bPosition = -1;

                var res = aPosition.CompareTo(bPosition);
                if (res == 0) res = a.characterId.CompareTo(b.characterId); // 場所が同じ場合、IDでソードします(ユーザにソート条件指定を実装かも
                return res;
            });
            sortedUserUnit = workUnit.ToArray();
            cellView.ReloadData();
        }

        public GameObject CellViewItem(int index, GameObject item)
        {
            if(item == null)
            {
                item = PrefabPool.Get(prefab.name);
            }
            var unit = sortedUserUnit[index];
            var thumbnail = item.GetComponent<Thumbnail>();
            thumbnail.Setup(unit, true);

            var deck = UserData.cacheHomeRecv.deck;
            var position = Array.IndexOf(deck.ids, unit.characterId);

            if(position != -1)
            {
                if(position == editIndex)
                {
                    thumbnail.SetDesc("<color=red>はずす</color>");
                }
                else
                {
                    thumbnail.SetDesc("<color=blue>入れ替え</color>");
                }
            }
            else
            {
                thumbnail.SetDesc("");
            }
            return item;
        }

        public Vector2 ItemSize()
        {
            var rect = prefab.GetComponent<RectTransform>();
            return rect.sizeDelta;
        }

        public int NumOfItems()
        {
            return sortedUserUnit.Length;
        }

        /// <summary>
        /// 選択した
        /// </summary>
        /// <param name="index"></param>
        /// <param name="listItem"></param>
        public void TapCellItem(int index, GameObject listItem)
        {
            var unit = sortedUserUnit[index];
            to.Setup(unit);

            if (from.cacheUnit == to.cacheUnit)
            {
                to.gameObject.SetActive(false);
                removeText.SetActive(true);
            } else
            {
                to.gameObject.SetActive(true);
                removeText.SetActive(false);
            }
            btnOK.interactable = true;
        }

        /// <summary>
        /// Backボタン
        /// </summary>
        public void OnBack()
        {
            gameObject.SetActive(false);
        }


        /// <summary>
        /// 指定のキャラIDをセットする
        /// </summary>
        /// <param name="characterId"></param>
        /// <param name="position"></param>
        /// <returns>該当座標があるのであれば元のデッキ情報を POP する</returns>
        string SetCharacterToDeck(string characterId, int position)
        {
            var deck = UserData.cacheHomeRecv.deck;
            var pop = deck.ids[position];
            deck.ids[position] = characterId;
            return pop;
        }

        /// <summary>
        /// OKを押した
        /// </summary>
        public void OnOK()
        {
            // 最低１体セットしないとだめのチェック
            if (UserData.cacheHomeRecv.deck.ids.Count(id => !string.IsNullOrEmpty(id)) <= 1 && from.cacheUnit == to.cacheUnit)
            {
                PlatformDialog.SetButtonLabel("OK");
                PlatformDialog.Show(
                    "警告",
                    "最低１体が必要",
                    PlatformDialog.Type.SubmitOnly,
                    () =>
                    {
                        Debug.Log("OK");
                    }
                );
                return;
            }

            var deck = UserData.cacheHomeRecv.deck;

            if (from.cacheUnit == to.cacheUnit)
            {
                // 外すなので、必ずある想定
                var index = Array.IndexOf(deck.ids, from.cacheUnit.characterId);
                SetCharacterToDeck("", index);
            }  else
            {
                // 元座標
                var beforePosition = Array.IndexOf(deck.ids, to.cacheUnit.characterId);
                // 編集場所にセットする
                var pop = SetCharacterToDeck(to.cacheUnit.characterId, editIndex);
                // 元座標にPOPしたものを設定する
                if(beforePosition != -1) SetCharacterToDeck(pop, beforePosition);
            }
            Observer.Notify("HomeRecv");
            gameObject.SetActive(false);
        }
    }
}

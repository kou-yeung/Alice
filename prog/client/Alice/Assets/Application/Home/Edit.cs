using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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
            editIndex = index;

            var deck = UserData.cacheHomeRecv.decks.FirstOrDefault(v => v.position == index);
            if(deck != null)
            {
                var unit = UserData.cacheHomeRecv.units.First(v => v.characterId == deck.characterId);
                from.Setup(unit);
                from.gameObject.SetActive(true);
            }
            else
            {
                from.gameObject.SetActive(false);
            }

            to.gameObject.SetActive(false);
            removeText.SetActive(false);
            btnOK.interactable = false;

            // 表示順を作る
            var workUnit = UserData.cacheHomeRecv.units.ToList();
            workUnit.Sort((a, b) =>
            {
                var aDeck = UserData.cacheHomeRecv.decks.FirstOrDefault(v => v.characterId == a.characterId);
                var bDeck = UserData.cacheHomeRecv.decks.FirstOrDefault(v => v.characterId == b.characterId);

                var aPosition = aDeck?.position;
                var bPosition = bDeck?.position;

                // はずす対象なら最優先
                if (aPosition == editIndex) aPosition = -1;
                if (bPosition == editIndex) bPosition = -1;

                var res = bPosition.HasValue.CompareTo(aPosition.HasValue);
                if (res == 0 && aPosition.HasValue) res = aPosition.Value.CompareTo(bPosition.Value);
                if (res == 0 && !aPosition.HasValue) res = a.characterId.CompareTo(b.characterId);
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
            thumbnail.Setup(unit);

            var deck = UserData.cacheHomeRecv.decks.FirstOrDefault(v => v.characterId == unit.characterId);

            if(deck != null)
            {
                if(deck.position == editIndex)
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
        /// OKを押した
        /// </summary>
        public void OnOK()
        {
            // 最低１体セットしないとだめのチェック
            if( UserData.cacheHomeRecv.decks.Length <= 1 && from.cacheUnit == to.cacheUnit)
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

            // 指定した場所を空く
            var decks = UserData.cacheHomeRecv.decks.Where(v => v.position != editIndex).ToArray();

            // 入れ替え元がセットされた場合、元の場所から外す
            if (from.cacheUnit != null)
            {
                decks = decks.Where(v => v.characterId != from.cacheUnit.characterId).ToArray();
            }
            // 入れ替え
            if (to.cacheUnit != null)
            {
                //　元の場所から外す
                decks = decks.Where(v => v.characterId != to.cacheUnit.characterId).ToArray();
            }

            // 違う場合、セットする
            if(from.cacheUnit != to.cacheUnit)
            {
                var add = new UserDeck { characterId = to.cacheUnit.characterId, position = editIndex };
                decks = decks.Concat(new[] { add }).ToArray();
            }
            UserData.cacheHomeRecv.decks = decks.ToArray();
            Observer.Notify("HomeRecv");
            gameObject.SetActive(false);
        }
    }
}

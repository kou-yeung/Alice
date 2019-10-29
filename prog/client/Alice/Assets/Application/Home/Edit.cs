using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Xyz.AnzFactory.UI;
using Zoo;
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
            editIndex = index;

            var deck = UserData.cacheHomeRecv.decks.FirstOrDefault(v => v.position == index);
            if(deck != null)
            {
                var unit = UserData.cacheHomeRecv.units.First(v => v.characterId == deck.characterId);
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
            thumbnail.Setup(unit, true);

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
        /// 指定のキャラIDをセットする
        /// </summary>
        /// <param name="characterId"></param>
        /// <param name="position"></param>
        /// <returns>該当座標があるのであれば元のデッキ情報を POP する</returns>
        UserDeck SetCharacterToDeck(string characterId, int position)
        {
            var deck = UserData.cacheHomeRecv.decks.FirstOrDefault(v => v.characterId == characterId);
            var pop = UserData.cacheHomeRecv.decks.FirstOrDefault(v => v.position == position);

            // すでにどこかにセットされたか？
            if (deck != null)
            {
                // セットしたため、場所だけ変更する
                deck.position = position;
            }
            else
            {
                // セットしてないので、追加する
                var add = new UserDeck { characterId = to.cacheUnit.characterId, position = editIndex };
                UserData.cacheHomeRecv.decks = UserData.cacheHomeRecv.decks.Concat(new[] { add }).ToArray();
            }
            if(pop != null)
            {
                // 一旦回避する
                pop.position = -1;
            }
            return pop;
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

            if (from.cacheUnit == to.cacheUnit)
            {
                // 外す
                UserData.cacheHomeRecv.decks = UserData.cacheHomeRecv.decks.Where(v => v.characterId != from.cacheUnit.characterId).ToArray();
            }  else
            {
                // 元座標
                var beforePosition = UserData.cacheHomeRecv.decks.FirstOrDefault(v => v.characterId == to.cacheUnit.characterId)?.position;

                // 編集場所にセットする
                var pop = SetCharacterToDeck(to.cacheUnit.characterId, editIndex);

                // Popしたものがある
                if(pop != null)
                {
                    if (beforePosition.HasValue)
                    {
                        pop = SetCharacterToDeck(pop.characterId, beforePosition.Value);
                        Assert.AreEqual(pop, null); // 入れ替えしたため、何もPOPしないはず
                    }
                }
                // 有効な場所のキャラのみ残す
                UserData.cacheHomeRecv.decks = UserData.cacheHomeRecv.decks.Where(v => v.position >= 0).ToArray();
            }

            Observer.Notify("HomeRecv");
            gameObject.SetActive(false);
        }
    }
}

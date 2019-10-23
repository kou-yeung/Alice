/*********************************
 Like UITableView
*********************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;

namespace Xyz.AnzFactory.UI
{
    [RequireComponent(typeof(ScrollRect))]
    public class ANZListView : MonoBehaviour
    {

        public interface IDataSource
        {
            int NumOfItems();
            float ItemSize();
            GameObject ListViewItem(int index, GameObject item);
        }

        public interface IActionDelegate
        {
            void TapListItem(int index, GameObject listItem);
        }
        public interface IPressDelegate
        {
            void PressListItem(int index, GameObject listItem);
        }

        #region "Fields"
        private ScrollRect scrollRect;
        private float rowSize;
        private int itemCount;
        private int visibleItemCount;
        private List<ListItemData> itemDataList;
        private List<ListItemData> visibleItemDataList;
        private float prevPosition;
        #endregion

        #region "Properties"
        public float RowSize
        {
            get { return this.rowSize; }
        }
        public int ItemCount
        {
            get { return this.itemCount; }
        }
        public IDataSource DataSource { get; set; }
        public IActionDelegate ActionDelegate { get; set; }
        public IPressDelegate PressDelegate { get; set; }
        #endregion

        #region "Events"
        private void Awake()
        {
            this.rowSize = 0;
            this.itemCount = 0;
            this.visibleItemCount = 0;
            this.itemDataList = new List<ListItemData>();
            this.prevPosition = -100f;

            this.Setup();
        }
        #endregion

        #region "Events"
        public void ChangedScrollPosition(Vector2 position)
        {
            var current = this.scrollRect.vertical ? position.y : position.x;
            if (-100f >= this.prevPosition) {
                // なにもしない
            } else if (this.prevPosition > current) {
                List<ListItemData> items = this.VisibleItems();
                if (items.Count > 0) {
                    while (items[items.Count - 1].Position > this.visibleItemDataList[this.visibleItemDataList.Count - 1].Position) {
                        var topItem = this.visibleItemDataList[0];
                        var lastItem = this.visibleItemDataList[this.visibleItemDataList.Count - 1];
                        var targetItem = this.itemDataList[lastItem.Position + 1];

                        if (this.visibleItemDataList.Count >= this.visibleItemCount) {
                            // 外す
                            var recycleItemObject = topItem.PopItem();
                            this.visibleItemDataList.Remove(topItem);
                            // つけかえる
                            targetItem.SetItemObjcet(recycleItemObject);
                        }

                        this.UpdateListItem(targetItem);
                        this.visibleItemDataList.Add(targetItem);
                    }
                }
            } else if (this.prevPosition < current) {
                List<ListItemData> items = this.VisibleItems();
                if (items.Count > 0) {
                    while (this.visibleItemDataList[0].Position > items[0].Position) {
                        var topItem = this.visibleItemDataList[0];
                        var lastItem = this.visibleItemDataList[this.visibleItemDataList.Count - 1];
                        if (topItem.Position > 0 && this.itemDataList.Count > topItem.Position) {
                            var targetItem = this.itemDataList[topItem.Position - 1];

                            if (this.visibleItemDataList.Count >= this.visibleItemCount) {
                                // 外す
                                var recycleItemObject = lastItem.PopItem();
                                this.visibleItemDataList.Remove(lastItem);
                                // つけかえる
                                targetItem.SetItemObjcet(recycleItemObject);
                            }

                            this.UpdateListItem(targetItem);
                            this.visibleItemDataList.Insert(0, targetItem);
                        }
                    }
                }
            }
            this.prevPosition = current;
        }

        public void TapItem(GameObject listItem)
        {
            if (this.ActionDelegate == null) {
                return;
            }

            for (int i = 0; i < this.visibleItemDataList.Count; i++) {
                if (this.visibleItemDataList[i].Item == listItem) {
                    this.ActionDelegate.TapListItem(this.visibleItemDataList[i].Position, listItem);
                    break;
                }
            }
        }
        public void PressItem(GameObject listItem)
        {
            if (this.PressDelegate == null)
            {
                return;
            }

            for (int i = 0; i < this.visibleItemDataList.Count; i++)
            {
                if (this.visibleItemDataList[i].Item == listItem)
                {
                    this.PressDelegate.PressListItem(this.visibleItemDataList[i].Position, listItem);
                    break;
                }
            }
        }
        #endregion

        #region "Public Methods"
        public void ReloadData()
        {
            if (!Application.isPlaying) return;
            StartCoroutine(this._reloadData());
        }
        private IEnumerator _reloadData()
        {
            yield return new WaitForEndOfFrame();

            this.itemCount = this.DataSource.NumOfItems();
            this.rowSize = this.DataSource.ItemSize();
            this.visibleItemCount = Mathf.CeilToInt(this.scrollRect.viewport.rect.height / this.rowSize) + 2;

            this.FillItems();
            this.visibleItemDataList = this.VisibleItems();
            foreach (var listItem in this.visibleItemDataList) {
                this.UpdateListItem(listItem);
            }
        }
        #endregion

        #region "Private Methods"
        private void Setup()
        {
            this.scrollRect = this.gameObject.GetComponent<ScrollRect>();

            if (this.scrollRect.vertical)
            {
                var layout = this.scrollRect.content.gameObject.GetComponent<VerticalLayoutGroup>();
                if (layout == null) layout = this.scrollRect.content.gameObject.AddComponent<VerticalLayoutGroup>();
                layout.childForceExpandHeight = false;
                layout.childForceExpandWidth = true;
                layout.childControlHeight = true;
                layout.childControlWidth = true;
            }
            else
            {
                var layout = this.scrollRect.content.gameObject.GetComponent<HorizontalLayoutGroup>();
                if (layout == null) layout = this.scrollRect.content.gameObject.AddComponent<HorizontalLayoutGroup>();
                layout.childForceExpandHeight = true;
                layout.childForceExpandWidth = false;
                layout.childControlHeight = true;
                layout.childControlWidth = true;
            }

            var contentSizeFitter = this.scrollRect.content.gameObject.GetComponent<ContentSizeFitter>();
            if (contentSizeFitter == null) {
                contentSizeFitter = this.scrollRect.content.gameObject.AddComponent<ContentSizeFitter>();
            }

            if (this.scrollRect.vertical)
            {
                contentSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
                contentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
            }
            else
            {
                contentSizeFitter.verticalFit = ContentSizeFitter.FitMode.Unconstrained;
                contentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            }

            this.scrollRect.onValueChanged.RemoveListener(this.ChangedScrollPosition);
            this.scrollRect.onValueChanged.AddListener(this.ChangedScrollPosition);
        }

        private void FillItems()
        {
            var index = 0;
            // ある分はリサイクル
            while (index < this.itemDataList.Count) {
                var listItemData = this.itemDataList[index];
                if (this.ItemCount > listItemData.Position) {
                    this.BuildItemContainer(listItemData.Position);
                } else {
                    break;
                }
                index++;
            }

            // 不足分の追加
            while (this.itemDataList.Count < this.ItemCount) {
                var listItemData = this.BuildItemContainer(this.itemDataList.Count);
                this.itemDataList.Add(listItemData);
            }

            // 余分なものの削除
            if (this.itemDataList.Count > this.ItemCount) {
                while (this.itemDataList.Count > this.ItemCount) {
                    var lastItem = this.itemDataList[this.itemDataList.Count - 1];
                    if (this.visibleItemDataList.Contains(lastItem)) {
                        this.visibleItemDataList.Remove(lastItem);
                    }
                    this.itemDataList.Remove(lastItem);
                    Destroy(lastItem.Container);
                }
            }
        }

        private List<ListItemData> VisibleItems()
        {
            int index = 0;

            if (this.scrollRect.vertical)
            {
                if (this.scrollRect.content.rect.height > this.scrollRect.viewport.rect.height)
                {
                    var length = (this.scrollRect.content.rect.size.y - this.scrollRect.viewport.rect.size.y);
                    var frameY = length - (length * this.scrollRect.verticalNormalizedPosition);
                    index = Mathf.FloorToInt(frameY / this.rowSize);
                }
            }
            else
            {
                if (this.scrollRect.content.rect.width > this.scrollRect.viewport.rect.width)
                {
                    var length = (this.scrollRect.content.rect.size.x - this.scrollRect.viewport.rect.size.x);
                    var frameX = length - (length * this.scrollRect.horizontalNormalizedPosition);
                    index = Mathf.FloorToInt(frameX / this.rowSize);
                }
            }

            index = Mathf.Max(index, 0);

            int indexLast = index + this.visibleItemCount;
            var items = new List<ListItemData>();
            while (index < indexLast) {
                if (index >= this.itemDataList.Count) {
                    break;
                }
                items.Add(this.itemDataList[index]);
                index++;
            }

            return items;
        }

        private void UpdateListItem(ListItemData listItem)
        {
            GameObject item;
            if (listItem.Item == null) {
                item = this.DataSource.ListViewItem(listItem.Position, null);
                Assert.IsNotNull(item, "ListItem is null!!");
                item.name = "ListItem";
                listItem.SetItemObjcet(item);
                var itemRectTransform = item.GetComponent<RectTransform>();
                itemRectTransform.anchorMin = new Vector2(0, 0);
                itemRectTransform.anchorMax = new Vector2(1, 1);

                var clickHandler = item.GetComponent<PointerHandler>() ?? item.AddComponent<PointerHandler>();
                clickHandler.callback = (gameObject, e) => {
                    switch (e)
                    {
                        case PointerHandler.Event.Click:
                            this.TapItem(gameObject);
                            break;
                        case PointerHandler.Event.Press:
                            this.PressItem(gameObject);
                            break;
                    }
                };
            } else {
                item = this.DataSource.ListViewItem(listItem.Position, listItem.Item);
            }
            item.SetActive(true);
        }

        private GameObject CreateContainer(string name)
        {
            var itemContainer = new GameObject(name);
            var layoutElement = itemContainer.AddComponent<LayoutElement>();
            itemContainer.transform.SetParent(this.scrollRect.content.gameObject.transform, false);
            if (this.scrollRect.vertical)
            {
                layoutElement.preferredHeight = this.RowSize;
            }
            else
            {
                layoutElement.preferredWidth = this.RowSize;
            }

            return itemContainer;
        }

        private ListItemData BuildItemContainer(int position)
        {
            ListItemData listItemData;
            if (this.itemDataList.Count > position) {
                listItemData = this.itemDataList[position];
            } else {
                var newContainer = this.CreateContainer("ItemContainer");
                listItemData = new ListItemData(position, newContainer);
            }
            if (this.scrollRect.vertical)
            {
                listItemData.Layout.preferredHeight = this.rowSize;
                listItemData.ContainerRectTransform.pivot = new Vector2(0, 1);
            }
            else
            {
                listItemData.Layout.preferredWidth = this.rowSize;
                listItemData.ContainerRectTransform.pivot = new Vector2(0, 1);
            }
            return listItemData;
        }

        #endregion

        private class ListItemData
        {
            private int position;
            private GameObject container;
            private RectTransform containerRectTransform;
            private LayoutElement layoutElement;
            private GameObject item;

            public int Position { get { return this.position; } }
            public GameObject Container { get { return this.container; } }
            public RectTransform ContainerRectTransform
            {
                get {
                    if (this.containerRectTransform == null) {
                        this.containerRectTransform = this.container.GetComponent<RectTransform>();
                    }
                    return this.containerRectTransform;
                }
            }
            public LayoutElement Layout { get { return this.layoutElement; } }
            public GameObject Item
            {
                get { return this.item; }
            }

            public ListItemData(int position, GameObject containerGameObject)
            {
                this.position = position;
                this.container = containerGameObject;
                this.layoutElement = containerGameObject.GetComponent<LayoutElement>();
                this.item = null;
            }

            public GameObject PopItem()
            {
                var target = this.item;
                this.item = null;
                return target;
            }

            public void SetItemObjcet(GameObject item)
            {
                this.item = item;
                this.item.transform.SetParent(this.container.transform, false);
                var rectTransform = this.item.GetComponent<RectTransform>();
                rectTransform.offsetMin = new Vector2(0, 0);
                rectTransform.offsetMax = new Vector2(0, 0);
            }
        }

        /// <summary>
        /// EventTriggerでやっちゃうとドラッグイベントも全部もっていっちゃって
        /// スクロールできなくなってしまうので、自分でやーる
        /// </summary>
        private class PointerHandler : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler, IBeginDragHandler
        {
            public enum Event
            {
                Click,  // クリック
                Press,  // 長押し
            }
            private static float PressTime = 0.5f;  // 長押し判定
            public System.Action<GameObject, Event> callback;
            bool blockOnce = false;
            Coroutine coroutine;

            public void OnPointerClick(PointerEventData eventData)
            {
                if (!blockOnce) this.callback(gameObject, Event.Click);
            }

            public void OnPointerDown(PointerEventData eventData)
            {
                blockOnce = false;
                coroutine = StartCoroutine(Press());
            }

            public void OnPointerUp(PointerEventData eventData)
            {
                if (coroutine != null) StopCoroutine(coroutine);
                coroutine = null;
            }
            public void OnBeginDrag(PointerEventData eventData)
            {
                if (coroutine != null) StopCoroutine(coroutine);
                coroutine = null;
            }

            IEnumerator Press()
            {
                yield return new WaitForSecondsRealtime(PressTime);
                blockOnce = true;
                this.callback(gameObject, Event.Press);
                coroutine = null;
            }
        }
    }
}
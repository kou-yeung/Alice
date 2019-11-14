using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Xyz.AnzFactory.UI;
using Zoo;
using UnityEngine.Purchasing;
using System;
using UniRx;
using System.Linq;

namespace Alice
{
    class StoreController : IStoreListener
    {
        public static StoreController Instance { get; private set; }
        public IStoreController controller { get; private set; }
        IExtensionProvider extensions;

        public static void Initialize(Action cb)
        {
            ScreenBlocker.Instance.Push();

            Instance = new StoreController();

            StandardPurchasingModule module = StandardPurchasingModule.Instance();
            var build = ConfigurationBuilder.Instance(module);
            List<ProductDefinition> products = new List<ProductDefinition>();

            // リスト CSV から取得するように対応
            products.Add(new ProductDefinition("alram_15", ProductType.Consumable));

            build.AddProducts(products);
            UnityPurchasing.Initialize(Instance, build);

            Observable.EveryUpdate()
                .Where(_ => Instance.controller != null)
                .Take(1)
                .Subscribe(_ => { }, () =>
                {
                    ScreenBlocker.Instance.Pop();
                    cb?.Invoke();
                });
        }

        public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
        {
            this.controller = controller;
            this.extensions = extensions;
        }

        public void OnInitializeFailed(InitializationFailureReason error)
        {
            Debug.Log($"OnInitializeFailed:{error.ToString()}");
        }

        public void OnPurchaseFailed(Product i, PurchaseFailureReason p)
        {
            Debug.Log($"OnPurchaseFailed[{i}]:{p.ToString()}");
        }

        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs e)
        {
            Debug.Log($"ProcessPurchase:{e.ToString()} : {e.purchasedProduct.receipt}");
            return PurchaseProcessingResult.Complete;   // とりあえず完了とします
        }
    }

    public class PurchasingDialog : BaseDialog, ANZListView.IDataSource
    {
        public ANZListView list;
        public GameObject itemPrefab;

        void Start()
        {
            PrefabPool.Regist(itemPrefab.name, itemPrefab);
            list.DataSource = this;
        }

        public static void Show()
        {
            var dialog = PrefabPool.Get(nameof(PurchasingDialog)).GetComponent<PurchasingDialog>();
            if (StoreController.Instance == null)
            {
                StoreController.Initialize(() =>
                {
                    dialog.list.ReloadData();
                    dialog.Open();
                });
            } else
            {
                dialog.list.ReloadData();
                dialog.Open();
            }
        }

        protected override void OnClosed()
        {
            PrefabPool.Release(nameof(PurchasingDialog), this.gameObject);
            base.OnClosed();
        }

        public float ItemSize()
        {
            return itemPrefab.GetComponent<RectTransform>().sizeDelta.y;
        }

        public GameObject ListViewItem(int index, GameObject item)
        {
            if(item == null)
            {
                item = PrefabPool.Get(itemPrefab.name);
            }
            var data = StoreController.Instance.controller.products.all[index];
            var purchasingItem = item.GetComponent<PurchasingItem>();
            purchasingItem.Setup(data);
            purchasingItem.OnClickEvent = OnClickItem;
            return item;
        }

        public int NumOfItems()
        {
            return StoreController.Instance.controller.products.all.Length;
        }

        public void OnClickItem(Product product)
        {
            StoreController.Instance.controller.InitiatePurchase(product);
        }
    }
}

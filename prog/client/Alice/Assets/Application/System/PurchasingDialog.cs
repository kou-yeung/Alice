using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Xyz.AnzFactory.UI;
using Zoo;
using Zoo.Communication;
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

        public static Const.Platform Platform
        {
            get
            {
#if UNITY_EDITOR
                return Const.Platform.UnityEditor;
#elif UNITY_ANDROID
                return Const.Platform.Android;
#elif UNITY_IPHONE
                return Const.Platform.iOS;
#else
                return Const.Platform.Unknown;
#endif
            }
        }
        static Entities.Product[] Products
        {
            get
            {
                return Entities.MasterData.Instance.products.Where(v => v.Platform == Platform).ToArray();
            }
        }

        public static void Initialize(Action cb)
        {
            ScreenBlocker.Instance.Push();

            Instance = new StoreController();

            StandardPurchasingModule module = StandardPurchasingModule.Instance();
            var build = ConfigurationBuilder.Instance(module);
            List<ProductDefinition> products = new List<ProductDefinition>();

            // 起動中のストアの課金アイテムで初期化する
            foreach (var item in Products)
            {
                products.Add(new ProductDefinition(item.ID, ProductType.Consumable));
            }
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
            ScreenBlocker.Instance.Pop();
            Dialog.Show(error.ToString(), Dialog.Type.SubmitOnly);
        }

        public void OnPurchaseFailed(Product i, PurchaseFailureReason p)
        {
            Debug.Log($"OnPurchaseFailed[{i}]:{p.ToString()}");
            ScreenBlocker.Instance.Pop();
            Dialog.Show(p.ToString(), Dialog.Type.SubmitOnly);
        }

        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs e)
        {
            //Dialog.Show(e.purchasedProduct.receipt, Dialog.Type.SubmitOnly);
            Debug.Log($"ProcessPurchase:{e.ToString()} : {e.purchasedProduct.receipt}");
            var c2s = new PurchasingSend();
            c2s.id = e.purchasedProduct.definition.id;
            c2s.platform = StoreController.Platform.ToString();
            c2s.receipt = e.purchasedProduct.receipt;
            CommunicationService.Instance.Request("Purchasing", JsonUtility.ToJson(c2s), (res) =>
            {
                var data = JsonUtility.FromJson<PurchasingRecv>(res);
                UserData.Modify(data.modified);
                controller.ConfirmPendingPurchase(e.purchasedProduct);
            });
            return PurchaseProcessingResult.Pending;   // 確認中
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
            Close();
            StoreController.Instance.controller.InitiatePurchase(product);
        }
    }
}

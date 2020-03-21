using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Purchasing;
using System;

namespace Alice
{
    public class PurchasingItem : MonoBehaviour
    {
        public Text num;
        public Text bonus;
        public Text price;
        Product product;
        public Action<Product> OnClickEvent;

        public void Setup(Product product)
        {
            this.product = product;
            var mst = Entities.MasterData.Instance.FindProductByID(product.definition.id);
            this.num.text = $"<size=16>x</size>{mst.Alarm}";
            this.bonus.text = $"Bonus +{mst.Bonus}";
            this.bonus.gameObject.SetActive(mst.Bonus > 0);
            this.price.text = $"{product.metadata.localizedPriceString}";
        }
        public void OnClick()
        {
            OnClickEvent?.Invoke(product);
        }
    }
}

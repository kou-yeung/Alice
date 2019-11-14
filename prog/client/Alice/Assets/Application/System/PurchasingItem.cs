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
        public Text text;
        Product product;
        public Action<Product> OnClickEvent;

        public void Setup(Product product)
        {
            this.product = product;
        }
        public void OnClick()
        {
            OnClickEvent?.Invoke(product);
        }
    }
}

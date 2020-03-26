using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zoo;
using System;

namespace Alice
{
    public class UnitDialog : BaseDialog
    {
        public EditItem editItem;
        Action onClose;
    
        public static void Show(UserUnit unit, Action onClose = null)
        {
            var dialog = PrefabPool.Get("UnitDialog").GetComponent<UnitDialog>();
            dialog.editItem.Setup(unit);
            dialog.Open();
            dialog.onClose = onClose;
        }

        protected override void OnClosed()
        {
            PrefabPool.Release("UnitDialog", this.gameObject);
            base.OnClosed();
            onClose?.Invoke();
        }
    }
}

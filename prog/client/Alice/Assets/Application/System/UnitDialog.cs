using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zoo;

namespace Alice
{
    public class UnitDialog : BaseDialog
    {
        public EditItem editItem;

        public static void Show(UserUnit unit)
        {
            var dialog = PrefabPool.Get("UnitDialog").GetComponent<UnitDialog>();
            dialog.editItem.Setup(unit);
            dialog.Open();
        }

        protected override void OnClosed()
        {
            PrefabPool.Release("UnitDialog", this.gameObject);
            base.OnClosed();
        }
    }
}

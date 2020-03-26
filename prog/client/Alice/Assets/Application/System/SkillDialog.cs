using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zoo;
using System;

namespace Alice
{
    public class SkillDialog : BaseDialog
    {
        public SkillItem skillItem;
        Action onClose;

        public static void Show(UserSkill skill, Action onClose = null)
        {
            var dialog = PrefabPool.Get("SkillDialog").GetComponent<SkillDialog>();
            dialog.skillItem.Setup(skill);
            dialog.Open();
            dialog.onClose = onClose;
        }

        protected override void OnClosed()
        {
            PrefabPool.Release("SkillDialog", this.gameObject);
            base.OnClosed();
            onClose?.Invoke();
        }
    }
}

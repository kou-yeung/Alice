using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zoo;

namespace Alice
{
    public class SkillDialog : BaseDialog
    {
        public SkillItem skillItem;

        public static void Show(UserSkill skill)
        {
            var dialog = PrefabPool.Get("SkillDialog").GetComponent<SkillDialog>();
            dialog.skillItem.Setup(skill);
            dialog.Open();
        }

        protected override void OnClosed()
        {
            PrefabPool.Release("SkillDialog", this.gameObject);
            base.OnClosed();
        }
    }
}

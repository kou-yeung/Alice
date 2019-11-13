using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Alice.Entities;
using Zoo.Assets;
using System;

namespace Alice
{
    public class SkillItem : MonoBehaviour
    {
        [Serializable]
        public class Effect
        {
            public GameObject gameObject;
            public Image accession; // 継承アイコン
            public Image side;      // サイト
            public Text desc;       // 説明
        }

        public Text Name;
        public Text Num;
        public Sprites sprites;
        public Effect[] effects;

        public void Setup(UserSkill skill)
        {
            var data = MasterData.Instance.Find(skill);
            Name.text = data.Name;

            var remain = UserData.RemainSkill(skill.id);
            var count = skill.count;
            if (remain <= 0)
            {
                Num.text = $"<color=red>{remain}</color>/{count}";
            }
            else
            {
                Num.text = $"{remain}/{count}";
            }

            BattleConst.Target? target = null;
            for (int i = 0; i < effects.Length; i++)
            {
                if (i < data.Effects.Length)
                {
                    effects[i].gameObject.SetActive(true);
                    var effect = MasterData.Instance.FindEffectByID(data.Effects[i]);
                    // 継承以外は更新する
                    if (effect.Target != BattleConst.Target.Accession)
                    {
                        target = effect.Target;
                    }
                    switch (target.Value)
                    {
                        case BattleConst.Target.Self:
                            effects[i].accession.sprite = sprites["self_accession"];
                            effects[i].side.sprite = sprites["self"];
                            break;
                        case BattleConst.Target.Friend:
                            effects[i].accession.sprite = sprites["friend_accession"];
                            effects[i].side.sprite = sprites[$"friend{effect.Count}"];
                            break;
                        case BattleConst.Target.Enemy:
                            effects[i].accession.sprite = sprites["enemy_accession"];
                            effects[i].side.sprite = sprites[$"enemy{effect.Count}"];
                            break;
                    }
                    effects[i].accession.SetNativeSize();
                    effects[i].accession.gameObject.SetActive(effect.Target == BattleConst.Target.Accession);
                    effects[i].side.SetNativeSize();
                    effects[i].desc.text = effect.Desc();
                }
                else
                {
                    effects[i].gameObject.SetActive(false);
                }
            }
        }
    }
}

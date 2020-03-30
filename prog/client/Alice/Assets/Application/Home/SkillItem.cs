using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Alice.Entities;
using Zoo.Assets;
using System;
using Alice.Logic;

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

        public Image Background;
        public Text Name;
        public Text Num;
        public Text CT;
        public Sprites sprites;
        public Effect[] effects;

        public void Setup(UserSkill skill)
        {
            var data = MasterData.Instance.Find(skill);
            Name.text = data.NameWithInfo;

            var remain = UserData.RemainSkill(skill.id);
            var count = skill.count;
            if (remain <= 0)
            {
                Num.text = $"{"SkillDesc_04".TextData()}<color=red>{remain}</color>/{count}";
            }
            else
            {
                Num.text = $"{"SkillDesc_04".TextData()}{remain}/{count}";
            }

            Background.color = ColorGen.Rare(data.Rare);

            // CT
            CT.text = string.Format("SkillDesc_03".TextData(),data.CoolTime);
            CT.gameObject.SetActive(!data.Passive);

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

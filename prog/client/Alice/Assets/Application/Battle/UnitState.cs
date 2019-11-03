using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Alice
{
    public class UnitState : MonoBehaviour
    {
        [SerializeField]
        Slider damage = null;
        [SerializeField]
        Slider hp = null;
        [SerializeField]
        Slider barrier = null;
        [SerializeField]
        Animation Animation = null;
        [SerializeField]
        Text value = null;
        [SerializeField]
        Text currentHP = null;

        [SerializeField]
        GameObject buffIcon = null;
        [SerializeField]
        Text buffCount = null;
        [SerializeField]
        GameObject debuffIcon = null;
        [SerializeField]
        Text debuffCount = null;

        [SerializeField]
        Image[] skill = null;

        int maxHP;
        /// <summary>
        /// セットアップ
        /// </summary>
        public void Setup(BattleUnit unit)
        {
            maxHP = unit.current.HP;
            currentHP.text = maxHP.ToString();
        }

        /// <summary>
        /// HPを設定する
        /// </summary>
        /// <param name="hp"></param>
        /// <param name="max"></param>
        public void SetHP(int hp)
        {
            var ratio = hp / (float)maxHP;
            this.hp.value = ratio;
            var sub = Mathf.Abs(this.damage.value - this.hp.value);
            LeanTween.value(this.damage.value, this.hp.value, 0.5f).setOnUpdate((float value) =>
            {
                currentHP.text = Mathf.FloorToInt(value* maxHP).ToString();
                this.damage.value = value;
            }).setOnComplete(()=>
            {
                currentHP.text = hp.ToString();
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public void PlayDamage(int value)
        {
            this.value.text = value.ToString();
            Animation.Play("Damage");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public void PlayRecovery(int value)
        {
            this.value.text = value.ToString();
            Animation.Play("Recovery");
        }

        /// <summary>
        /// パフ、デバフの数を設定する
        /// </summary>
        /// <param name="buff"></param>
        /// <param name="debuff"></param>
        public void UpdateCondition(BattleUnit unit)
        {
            var buff = unit.BuffCount();
            var debuff = unit.DebuffCount();

            buffIcon.SetActive(buff > 0);
            buffCount.text = buff.ToString();
            debuffIcon.SetActive(debuff > 0);
            debuffCount.text = debuff.ToString();
        }

        /// <summary>
        /// スキルのクールタイム状況を更新する
        /// </summary>
        /// <param name="unit"></param>
        public void UpdateCooltime(BattleUnit unit)
        {
            for (int i = 0; i < skill.Length; i++)
            {
                if(i < unit.skills.Count)
                {
                    if(unit.CanUseSkill(unit.skills[i]))
                    {
                        skill[i].color = Color.red;// new Color(.4f, 1f, .45f);
                    } else if(unit.skills[i].Passive)
                    {
                        // パッシブスキル別色？
                        skill[i].color = Color.gray;
                    } else
                    {
                        skill[i].color = Color.gray;
                    }
                } else
                {
                    skill[i].color = Color.black;
                }
            }
        }
    }
}

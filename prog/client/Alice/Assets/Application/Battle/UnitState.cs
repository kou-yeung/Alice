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

        int maxHP;
        /// <summary>
        /// セットアップ
        /// </summary>
        public void Setup(BattleUnit unit)
        {
            maxHP = unit.characterData.HP;
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

    }
}

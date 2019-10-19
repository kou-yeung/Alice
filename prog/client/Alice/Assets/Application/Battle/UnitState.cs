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


        /// <summary>
        /// HPを設定する
        /// </summary>
        /// <param name="hp"></param>
        /// <param name="max"></param>
        public void SetHP(int hp, int max)
        {
            var ratio = hp / (float)max;
            this.hp.value = ratio;
            var sub = Mathf.Abs(this.damage.value - this.hp.value);
            LeanTween.value(this.damage.value, this.hp.value, 0.5f).setOnUpdate((float value) =>
            {
                this.damage.value = value;
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

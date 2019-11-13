using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UniRx;
using Zoo.IO;

namespace Alice
{
    public class Versus : MonoBehaviour
    {
        [SerializeField]
        Text leftSide;
        [SerializeField]
        Text rightSide;
        [SerializeField]
        Animation Animation;

        public void Show(string leftSide, string rightSide, Action cb)
        {
            this.leftSide.text = leftSide;
            this.rightSide.text = rightSide;

            this.gameObject.SetActive(true);
            Animation.Play("Start");
            Observable
                .EveryUpdate()
                .Where(_ => !Animation.isPlaying)
                .Take(1)
                .Subscribe(_ => { }, ()=>
                {
                    this.gameObject.SetActive(false);
                    cb?.Invoke();
                });
        }
        /// <summary>
        /// 後始末
        /// </summary>
        public void Destory()
        {
            GameObject.Destroy(this.gameObject);
        }
        public static Versus Gen(Transform parent)
        {
            var prefab = LoaderService.Instance.Load<GameObject>("Prefab/Versus.prefab");
            var go = GameObject.Instantiate(prefab);
            go.transform.SetParent(parent);
            go.transform.localPosition = Vector3.zero;
            return go.GetComponent<Versus>();
        }
    }
}

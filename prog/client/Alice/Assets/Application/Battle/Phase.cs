using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zoo.IO;
using System;
using UniRx;

namespace Alice
{
    public class Phase : MonoBehaviour
    {
        [SerializeField]
        Animation Animation = null;
        [SerializeField]
        Text text = null;

        public void Change(string phase, Action cb = null)
        {
            text.text = phase;
            Animation.Play("Change");
            if (cb != null)
            {
                Observable
                    .EveryUpdate()
                    .Where(_ => !Animation.isPlaying)
                    .Take(1)
                    .Subscribe(_ => { }, cb);
            }
        }

        /// <summary>
        /// 後始末
        /// </summary>
        public void Destory()
        {
            GameObject.Destroy(this.gameObject);
        }
        public static Phase Gen(Transform parent)
        {
            var prefab = LoaderService.Instance.Load<GameObject>("Prefab/Phase.prefab");
            var go = GameObject.Instantiate(prefab);
            go.transform.SetParent(parent);
            go.transform.localPosition = Vector3.zero;
            return go.GetComponent<Phase>();
        }
    }
}

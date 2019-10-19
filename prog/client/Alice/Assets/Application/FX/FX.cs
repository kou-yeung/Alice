using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using Zoo.Assets;
using Zoo.IO;
using Zoo;
using UniRx;

namespace Alice
{
    /// <summary>
    /// 画面エフェクト
    /// </summary>
    public class FX : MonoBehaviour
    {
        const string PrefabPoolKey = "FX";

        [SerializeField]
        Image image;

        [SerializeField]
        Sprites sprites;

        void Awake()
        {
            image = GetComponent<Image>();
        }

        /// <summary>
        /// 再生する
        /// </summary>
        /// <param name="fn"></param>
        /// <param name="cb"></param>
        public void Play(string fn, Action cb = null)
        {
            sprites = LoaderService.Instance.Load<Sprites>($"Effect/{fn}.asset");
            UpdateImage(0);

            Observable
                .Timer(TimeSpan.Zero, TimeSpan.FromMilliseconds(20))
                .Take(sprites.Count)
                .Subscribe(index => UpdateImage((int)index),
                ()=>
                {
                    PrefabPool.Release("FX", gameObject);
                    cb?.Invoke();
                });
        }

        /// <summary>
        /// イメージセットする
        /// </summary>
        /// <param name="index"></param>
        void UpdateImage(int index)
        {
            image.sprite = sprites[index];
            image.SetNativeSize();
        }
        /// <summary>
        /// 再生
        /// </summary>
        /// <param name="fn"></param>
        /// <returns></returns>
        public static FX Play(string fn, Transform parent, Action cb = null)
        {
            return Play(fn, parent, Vector3.zero, cb);
        }

        /// <summary>
        /// 再生
        /// </summary>
        /// <param name="fn"></param>
        /// <returns></returns>
        public static FX Play(string fn, Transform parent, Vector3 position, Action cb = null)
        {
            if(!PrefabPool.Has(PrefabPoolKey))
            {
                // Pool 登録
                PrefabPool.Regist(PrefabPoolKey, LoaderService.Instance.Load<GameObject>($"Prefab/FX.prefab"));
            }

            var go = PrefabPool.Get(PrefabPoolKey);
            var fx = go.GetComponent<FX>();
            fx.Play(fn, cb);
            go.transform.SetParent(parent);
            go.transform.localPosition = position;
            return fx;
        }
    }
}

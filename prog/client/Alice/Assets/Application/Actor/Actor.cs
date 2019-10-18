using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zoo.Assets;
using UnityEngine.UI;
using System;
using UniRx;

namespace Alice
{
    [ExecuteInEditMode]
    public class Actor : MonoBehaviour
    {
        public Sprites sprites;
        public int spriteIndex;

        Animation Animation;
        Image image;

        void Awake()
        {
            Animation = GetComponent<Animation>();
            image = GetComponent<Image>();
            setAnimation("Attack", () =>
            {
                setAnimation("Hit");
            });
        }

        void Update()
        {
            if(sprites)
            {
                if(spriteIndex >= 0 && spriteIndex < sprites.Count)
                {
                    image.sprite = sprites[spriteIndex];
                }
            }
        }

        public void FrameEvent(string name)
        {
            Debug.Log($"FrameEvent:{name}");
        }

        public void setAnimation(string name, Action cb = null)
        {
            Animation.Play(name);
            if(cb != null)
            {
                Observable
                    .EveryUpdate()
                    .Where(_ => !Animation.isPlaying)
                    .Take(1)
                    .Subscribe(_ => { }, cb);
            }
        }
    }
}

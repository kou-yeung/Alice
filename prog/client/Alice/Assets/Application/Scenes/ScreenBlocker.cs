using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Alice
{
    public class ScreenBlocker : MonoBehaviour
    {
        public static ScreenBlocker Instance { get; private set; }
        int count = 0;
        [SerializeField]
        Image[] images;

        void Start()
        {
            Instance = this;
            Setup();
        }
        void Update()
        {
            var dt = Time.deltaTime;
            foreach (var image in images)
            {
                var color = image.color;
                color.a = color.a - dt;
                if (color.a < 0.0f) color.a = .75f;
                image.color = color;
            }
        }

        public void Push()
        {
            ++count;
            Setup();
        }
        public void Pop()
        {
            count = Mathf.Max(0, count - 1);
            Setup();
        }

        void Setup()
        {
            gameObject.SetActive(count > 0);
            gameObject.transform.SetAsLastSibling();

            // 色はランダム
            if(count <= 0)
            {
                foreach(var image in images)
                {
                    image.color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, .75f));
                }
            }
        }
    }
}


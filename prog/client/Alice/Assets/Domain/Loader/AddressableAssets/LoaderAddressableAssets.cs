using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UniRx;

namespace Zoo.IO
{
    public class LoaderAddressableAssets : ILoader
    {
        string rootPath;
        Dictionary<string, object> caches = new Dictionary<string, object>();

        public LoaderAddressableAssets(string rootPath = "")
        {
            this.rootPath = rootPath;
        }

        public T Load<T>(string path) where T : class
        {
            object o;
            if(caches.TryGetValue(path, out o))
            {
                return o as T;
            }

            Debug.LogError(($"not preloaded asset!! [{path}]"));
            return null;
        }

        public void LoadAsync<T>(string path, Action<T> onloaded) where T : class
        {
            Observable
                .FromCoroutine<object>(o => _Preload(o, path))
                .Subscribe(o =>
                {
                    caches[path] = o;
                    onloaded?.Invoke(o as T);
                });
        }

        public void Preload(string[] paths, Action onloaded)
        {
            var function = Async.Passive(onloaded, paths.Length);
            foreach(var path in paths)
            {
                LoadAsync<object>(path, (_) => function());
            }
        }

        IEnumerator _Preload(IObserver<object> observer, string path)
        {
            var handle = Addressables.LoadAssetAsync<object>(rootPath + path);
            yield return handle;
            observer.OnNext(handle.Result);
            observer.OnCompleted();
        }
    }
}



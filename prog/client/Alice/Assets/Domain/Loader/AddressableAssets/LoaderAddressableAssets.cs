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
            throw new Exception($"not preloaded asset!! [{path}]");
        }

        public void LoadAsync<T>(string path, Action<T> onloaded) where T : class
        {
            if(caches.ContainsKey(path))
            {
                // すでにキャッシュしたため使い回す
                onloaded(caches[path] as T);
            }
            else
            {
                // ストレージから非同期ロードして返す
                Observable.FromCoroutine(() => _Preload<T>(path, (res) =>
                {
                    caches[path] = res;
                    onloaded(res as T);
                })).Subscribe();
            }
        }

        public void Preload(string[] paths, Action onloaded)
        {
            var function = Async.Passive(onloaded, paths.Length);
            foreach(var path in paths)
            {
                LoadAsync<object>(path, (_) => function());
            }
        }
        IEnumerator _Preload<T>(string path, Action<T> cb) where T : class
        {
            var handle = Addressables.LoadAssetAsync<object>(rootPath + path);
            yield return handle;
            cb(handle.Result as T);
        }
    }
}



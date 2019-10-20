using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace Zoo
{
    /// <summary>
    /// Prefabから生成したい場合のプール
    /// </summary>
    public class PrefabPool
    {
        static Dictionary<string, GameObject> prefabs = new Dictionary<string, GameObject>();
        static Dictionary<string, Stack<GameObject>> pools = new Dictionary<string, Stack<GameObject>>();

        /// <summary>
        /// 登録
        /// </summary>
        /// <param name="key"></param>
        /// <param name="prefab"></param>
        public static void Regist(string key, GameObject prefab)
        {
            prefabs[key] = prefab;
            pools[key] = new Stack<GameObject>();
        }

        /// <summary>
        /// 指定したキーが登録されたか？
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool Has(string key)
        {
            return prefabs.ContainsKey(key);
        }
        /// <summary>
        /// キーを指定してオブジェクトを取得する
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static GameObject Get(string key)
        {
            Stack<GameObject> stack = pools[key];

            GameObject res;
            while(stack.Count > 0)
            {
                res = stack.Pop();
                // シーン切り替えによる破棄される可能性があるため、nullチェックします
                if (res != null)
                {
                    res.SetActive(true);
                    return res;
                }
            }
            res = GameObject.Instantiate(prefabs[key]);
            res.SetActive(true);
            return res;
        }


        /// <summary>
        /// Poolに返す
        /// </summary>
        /// <param name="key"></param>
        /// <param name="gameObject"></param>
        public static void Release(string key, GameObject gameObject)
        {
#if UNITY_EDITOR
            Assert.IsFalse(pools[key].Contains(gameObject));
#endif
            gameObject.SetActive(false);
            pools[key].Push(gameObject);
        }
    }
}

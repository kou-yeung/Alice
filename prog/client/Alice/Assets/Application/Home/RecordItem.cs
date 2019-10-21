using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zoo;

namespace Alice
{
    /// <summary>
    /// 試合履歴アイテムx1
    /// </summary>
    public class RecordItem : MonoBehaviour
    {
        public static readonly string PrefabKey = "RecordItem";
        public Text title;

        BattleStartRecv recv;
        /// <summary>
        /// 
        /// </summary>
        public void Setup(BattleStartRecv recv)
        {
            this.recv = recv;
            title.text = this.recv.seed.ToString();
        }

        /// <summary>
        /// RecordItem生成
        /// </summary>
        /// <returns></returns>
        public static RecordItem Gen()
        {
            var go = PrefabPool.Get(PrefabKey);
            var res = go.GetComponent<RecordItem>();
            return res;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zoo;
using System.Text;

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
        static StringBuilder sb = new StringBuilder();
        public void Setup(BattleStartRecv recv)
        {
            this.recv = recv;

            sb.Clear();
            if(this.recv.names != null)
            {
                sb.AppendLine($"{this.recv.names[0]} vs. {this.recv.names[1]}");
            }
            switch (this.recv.result)
            {
                case BattleConst.Result.Unknown:
                case BattleConst.Result.Draw:
                    sb.AppendLine($"{this.recv.result}");
                    break;
                case BattleConst.Result.Win:
                    sb.AppendLine($"<color=red>WIN</color>");
                    break;
                case BattleConst.Result.Lose:
                    sb.AppendLine($"<color=blue>LOSE</color>");
                    break;
            }
            sb.AppendLine($"----------------------------");
            sb.AppendLine($"ID:{this.recv.seed}");

            title.text = sb.ToString();
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

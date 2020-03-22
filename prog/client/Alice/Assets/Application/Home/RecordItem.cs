using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zoo;
using System.Text;
using System;
using System.Linq;

namespace Alice
{
    /// <summary>
    /// 試合履歴アイテムx1
    /// </summary>
    public class RecordItem : MonoBehaviour
    {
        public static readonly string PrefabKey = "RecordItem";
        public Text result; // バトル結果

        [Serializable]
        public class PlayInfo
        {
            public Text name;
            public Thumbnail[] thumbnails;
        }

        public PlayInfo self;
        public PlayInfo enemy;

        BattleStartRecv recv;
        /// <summary>
        /// 
        /// </summary>
        static StringBuilder sb = new StringBuilder();
        public void Setup(BattleStartRecv recv)
        {
            this.recv = recv;

            sb.Clear();

            self.name.text = this.recv.names[0];
            enemy.name.text = this.recv.names[1];

            switch (this.recv.result)
            {
                case BattleConst.Result.Unknown:
                case BattleConst.Result.Draw:
                    result.text = $"{this.recv.result}";
                    break;
                case BattleConst.Result.Win:
                    result.text = $"<color=red>WIN</color>";
                    break;
                case BattleConst.Result.Lose:
                    result.text = $"<color=blue>LOSE</color>";
                    break;
            }

            // 自分のユニット情報設定
            for (int i = 0; i < self.thumbnails.Length; i++)
            {
                var id = this.recv.playerDeck.ids.ElementAtOrDefault(i);
                var data = this.recv.playerUnit.FirstOrDefault(v => v.characterId == id);
                SetupThumbnail(self.thumbnails[i], data);
            }

            // 相手のユニット情報設定
            for (int i = 0; i < enemy.thumbnails.Length; i++)
            {
                string id = this.recv.enemyDeck.ids.ElementAtOrDefault(i);
                var data = this.recv.enemyUnit.FirstOrDefault(v => v.characterId == id);
                SetupThumbnail(enemy.thumbnails[i], data);
            }
        }

        /// <summary>
        /// ユニットアイコンを設定します
        /// </summary>
        /// <param name="thumbnail"></param>
        /// <param name="unit"></param>
        void SetupThumbnail(Thumbnail thumbnail, UserUnit unit)
        {
            thumbnail.gameObject.SetActive(unit != null);
            if (unit != null) thumbnail.Setup(unit, true);
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

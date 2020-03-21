using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

namespace Alice
{
    /// <summary>
    /// シャドウバトル１個
    /// </summary>
    public class ShadowItem : MonoBehaviour
    {
        public Text enemyName;
        public Thumbnail[] thumbnails;
        ShadowEnemy cacheShadowEnemy;

        public void Setup(ShadowEnemy shadowEnemy)
        {
            this.cacheShadowEnemy = shadowEnemy;

            enemyName.text = shadowEnemy.name;
            // 相手のユニット情報設定
            for (int i = 0; i < shadowEnemy.deck.ids.Length; i++)
            {
                var id = shadowEnemy.deck.ids[i];
                var data = shadowEnemy.unit.FirstOrDefault(v => v.characterId == id);
                SetupThumbnail(thumbnails[i], data);
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
    }
}

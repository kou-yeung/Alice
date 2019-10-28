using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Alice
{
    /// <summary>
    /// デッキ編成アイテム
    /// </summary>
    public class EditItem : MonoBehaviour
    {
        [SerializeField]
        Thumbnail thumbnail;

        public UserUnit cacheUnit { get; private set; }
        public void Setup(UserUnit unit)
        {
            this.cacheUnit = unit;
            thumbnail.Setup(unit);
        }
    }
}

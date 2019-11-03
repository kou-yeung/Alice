using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Alice
{
    /// <summary>
    /// シャドウバトル１個
    /// </summary>
    public class ShadowItem : MonoBehaviour
    {
        public Text debugInfo;
        ShadowEnemy cacheShadowEnemy;

        public void Setup(ShadowEnemy shadowEnemy)
        {
            this.cacheShadowEnemy = shadowEnemy;
            debugInfo.text = shadowEnemy.name;
        }
    }
}

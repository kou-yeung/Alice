using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zoo.Communication;

namespace Alice
{
    public class VS : MonoBehaviour
    {
        public Battle battle;

        /// <summary>
        /// シャドウを生成する
        /// </summary>
        public void OnCreate()
        {
            var c2s = new ShadowCreateSend();
            CommunicationService.Instance.Request("CreateShadow", JsonUtility.ToJson(c2s), res =>
            {
            });
        }

        public void OnBattle()
        {
            var c2s = new ShadowBattleSend(630);
            CommunicationService.Instance.Request("BattleShadow", JsonUtility.ToJson(c2s), res =>
            {
                UserData.editedUnit.Clear();    // 同期しました
                battle.Exec(JsonUtility.FromJson<BattleStartRecv>(res));
            });
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zoo.Communication;

namespace Alice
{
    public class Home : MonoBehaviour
    {
        public Battle battle;
        public void OnBattle()
        {
            // バトル情報を取得する
            CommunicationService.Instance.Request("Battle", "", (res) =>
            {
                battle.Exec(JsonUtility.FromJson<BattleStartRecv>(res));
            });
        }
    }

}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zoo.Communication;

namespace Alice
{
    public class Base : MonoBehaviour
    {
        public Chest[] chests;
        public Battle battle;

        void Start()
        {
            Setup();
        }

        public void Setup()
        {
            var recv = UserData.cacheHomeRecv;
            for (int i = 0; i < chests.Length; i++)
            {
                var chest = i < recv.chests.Length ? recv.chests[i] : null;
                chests[i].Setup(chest);
                chests[i].CliceEvent += () => ClickChest(chest);
            }
        }

        /// <summary>
        /// 宝箱をクリックした
        /// </summary>
        /// <param name="chest"></param>
        void ClickChest(UserChest chest)
        {
            Debug.Log(JsonUtility.ToJson(chest));
        }
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

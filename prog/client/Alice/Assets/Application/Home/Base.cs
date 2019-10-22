using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zoo.Communication;
using Zoo;

namespace Alice
{
    public class Base : MonoBehaviour
    {
        public Chest[] chests;
        public Card[] cards;
        public Battle battle;

        void Start()
        {
            Observer.AddObserver("HomeRecv", Setup);
            Setup();
        }

        public void Setup()
        {
            var recv = UserData.cacheHomeRecv;

            // ユニット
            for (int i = 0; i < cards.Length; i++)
            {
                var unit = recv.units.FirstOrDefault(v => v.position == i);
                cards[i].Setup(unit);
            }

            // 宝箱
            for (int i = 0; i < chests.Length; i++)
            {
                var chest = i < recv.chests.Length ? recv.chests[i] : null;
                chests[i].Setup(chest);
                chests[i].CliceEvent = () => ClickChest(chest);
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

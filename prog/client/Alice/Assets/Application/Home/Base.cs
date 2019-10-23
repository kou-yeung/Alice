using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zoo.Communication;
using Zoo;
using UnityEngine.Advertisements;

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
            //Debug.Log(JsonUtility.ToJson(chest));
            //Advertisement.Show(new ShowOptions { resultCallback = ResultCallback });
            Ads.Instance.Show(chest, (res) =>
            {
            });
        }
        public void OnBattle()
        {
            // バトル情報を取得する
            CommunicationService.Instance.Request("Battle", "", (res) =>
            {
                battle.Exec(JsonUtility.FromJson<BattleStartRecv>(res));
            });
        }

        /// <summary>
        /// 広告の表示結果
        /// </summary>
        /// <param name="result"></param>
        void ResultCallback(ShowResult result)
        {
            Debug.Log(result);
            if (result == ShowResult.Finished)
            {
                //Protocol.Send(new AdsEndSend { id = receive.id }, (AdsEndReceive end) =>
                //{
                //    Entity.Instance.HatchList.Modify(end.hatch);
                //    Entity.Instance.UnitList.Modify(end.unit);
                //    Close();
                //});
            }
            else
            {
                //Close();
            }
        }

    }
}

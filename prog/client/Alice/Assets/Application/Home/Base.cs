using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zoo.Communication;
using Zoo;
using UnityEngine.Advertisements;
using System;
using Alice.Entities;

namespace Alice
{
    public class Base : MonoBehaviour
    {
        public Chest[] chests;
        public Card[] cards;
        public Battle battle;
        public Edit edit;

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
                var deck = recv.decks.FirstOrDefault(v => v.position == i);
                if (deck == null)
                {
                    cards[i].Setup(null);
                } else
                {
                    var unit = recv.units.First(v => v.characterId == deck.characterId);
                    cards[i].Setup(unit);
                }

                cards[i].OnEditEvent = OnEditEvent;
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
            if (Application.internetReachability == NetworkReachability.NotReachable) return;

            var remain = Math.Max(0, chest.end - DateTime.Now.Ticks);
            if (remain <= 0)
            {
                var c2s = new ChestSend();
                c2s.chest = chest;
                // 開く
                CommunicationService.Instance.Request("Chest", JsonUtility.ToJson(c2s), (res) =>
                {
                    var s2c = JsonUtility.FromJson<ChestRecv>(res);
                    UserData.Modify(s2c.modified);

                    var name = "新キャラクタ";

                    //foreach(var unit in s2c.modified.unit)
                    //{
                    //    name = MasterData.characters.First(v => v.ID == unit.characterId).Name;
                    //}

                    PlatformDialog.SetButtonLabel("OK");
                    PlatformDialog.Show(
                        "おめでとう",
                        $"{name} を入手しました",
                        PlatformDialog.Type.SubmitOnly,
                        () => {
                            Debug.Log("OK");
                        }
                    );
                });
            }
            else
            {
                PlatformDialog.SetButtonLabel("Yes", "No");
                PlatformDialog.Show(
                    "確認(仮)",
                    "広告を観て時間短縮しますか？",
                    PlatformDialog.Type.OKCancel,
                    () => {
                        // 広告
                        Ads.Instance.Show(chest, (res) =>
                        {
                        });
                    }
                );
            }
        }

        /// <summary>
        /// 編集したい
        /// </summary>
        void OnEditEvent(int index)
        {
            edit.Open(index);
        }

        public void OnBattle()
        {
            var cache = UserData.cacheHomeRecv;
            var c2v = new BattleStartSend();
            c2v.player = cache.player;
            c2v.decks = cache.decks;
            c2v.units = cache.units.Where(v => Array.Exists(c2v.decks, deck => deck.characterId == v.characterId)).ToArray();

            // バトル情報を取得する
            CommunicationService.Instance.Request("Battle", JsonUtility.ToJson(c2v), (res) =>
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

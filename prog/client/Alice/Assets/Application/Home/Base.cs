using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zoo.Communication;
using Zoo;
using UnityEngine.Advertisements;
using System;
using Alice.Entities;
using Zoo.Time;

namespace Alice
{
    public class Base : MonoBehaviour
    {
        public Chest[] chests;
        public Card[] cards;
        public Battle battle;
        public Edit edit;
        public SkillView skill;

        void Start()
        {
            Observer.AddObserver("HomeRecv", Setup);
            Setup();
        }
        private void OnDestroy()
        {
            Observer.RemoveObserver("HomeRecv", Setup);
        }
        public void Setup()
        {
            var recv = UserData.cacheHomeRecv;

            // ユニット
            for (int i = 0; i < cards.Length; i++)
            {
                var id = recv.deck.ids[i];
                if (string.IsNullOrEmpty(id))
                {
                    cards[i].Setup(null);
                } else
                {
                    var unit = recv.units.First(v => v.characterId == id);
                    cards[i].Setup(unit);
                }

                cards[i].OnEditEvent = OnEditEvent;
                cards[i].OnSkillEvent = OnSkillEvent;
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
            var remain = Math.Max(0, chest.end - ServerTime.CurrentUnixTime);
            if (remain <= 0)
            {
                var c2s = new ChestSend();
                c2s.chest = chest;
                // 開く
                CommunicationService.Instance.Request("Chest", JsonUtility.ToJson(c2s), (res) =>
                {
                    var s2c = JsonUtility.FromJson<ChestRecv>(res);
                    UserData.Modify(s2c.modified);

                    if (s2c.modified.unit.Length != 0)
                    {
                        foreach (var unit in s2c.modified.unit)
                        {
                            name = MasterData.Instance.characters.First(v => v.ID == unit.characterId).Name;
                        }
                    }
                    if (s2c.modified.skill.Length != 0)
                    {
                        foreach (var skill in s2c.modified.skill)
                        {
                            name = MasterData.Instance.skills.First(v => v.ID == skill.id).Name;
                        }
                    }

                    Dialog.Show($"{name} を入手しました", Dialog.Type.SubmitOnly,
                        () => {
                            Debug.Log("OK");
                        }
                    );
                });
            }
            else
            {
                ChestDialog.Show(chest);
                //var player = UserData.cacheHomeRecv.player;

                //if (player.ads > 0)
                //{
                //    Dialog.Show("広告を観て時間短縮しますか？", Dialog.Type.OKCancel, () =>
                //    {
                //        // 広告
                //        Ads.Instance.Show(chest, (res) =>
                //        {
                //        });
                //    });
                //} else
                //{
                //    Dialog.Show( "広告回数は制限されますが、将来は時短アイテム購入可能にします",
                //        Dialog.Type.SubmitOnly,
                //        () => {
                //            Debug.Log("OK");
                //        }
                //    );
                //}
            }
        }

        /// <summary>
        /// 編集したい
        /// </summary>
        void OnEditEvent(int index)
        {
            edit.Open(index);
        }

        /// <summary>
        /// スキルをセットしたい
        /// </summary>
        /// <param name="unit"></param>
        /// <param name="index"></param>
        void OnSkillEvent(UserUnit unit, int index)
        {
            skill.Open(unit, index);
        }

        public void OnBattle()
        {
            var c2v = new BattleStartSend();

            // バトル情報を取得する
            CommunicationService.Instance.Request("Battle", JsonUtility.ToJson(c2v), (res) =>
            {
                UserData.editedUnit.Clear();
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

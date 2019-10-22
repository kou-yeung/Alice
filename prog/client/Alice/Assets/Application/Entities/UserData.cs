﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using Zoo;

namespace Alice
{
    public static class UserData
    {
        /// <summary>
        /// ホーム情報をキャッシュする
        /// </summary>
        public static HomeRecv cacheHomeRecv { get; private set; }
        public static UserUnit[] cacheUserDeck { get; private set; }
        public static void CacheHomeRecv(HomeRecv homeRecv)
        {
            cacheHomeRecv = homeRecv;

            // 編成されたユニットをキャッシュしておく
            UserUnit[] deck = new UserUnit[4];
            foreach (var unit in cacheHomeRecv.units)
            {
                if (unit.position == -1) continue;
                deck[unit.position] = unit;
            }
            cacheUserDeck = deck.Where(v => v != null).ToArray();

            Observer.Notify("HomeRecv");
        }

        static BattleRecord battleRecord;
        public static BattleRecord GetBattleRecord()
        {
            if(battleRecord == null)
            {
                battleRecord = new BattleRecord();
            }
            return battleRecord;
        }
        /// <summary>
        /// ストレージに試合結果を保持する
        /// </summary>
        public static void SaveBattleRecord()
        {
            // 初期化されなかったら弾く
            if (battleRecord == null) return;
            battleRecord.Save();
        }

        /// <summary>
        /// 更新あり
        /// </summary>
        public static void Modify(GameSetRecv recv)
        {
            cacheHomeRecv.player = recv.player;
            Modify(recv.modifiedUnit);
            Modify(recv.modifiedChest);
            Observer.Notify("HomeRecv");
        }

        /// <summary>
        /// ユニット更新
        /// </summary>
        /// <param name="units"></param>
        public static void Modify(UserUnit[] units)
        {
            foreach (var unit in units)
            {
                var index = Array.FindIndex(cacheHomeRecv.units, v => v.characterId == unit.characterId);
                if (index != -1)
                {
                    cacheHomeRecv.units[index] = unit;
                }
                else
                {
                    cacheHomeRecv.units = new[] { unit }.Concat(cacheHomeRecv.units).ToArray();
                }
            }
        }
        /// <summary>
        /// 宝箱更新
        /// </summary>
        /// <param name="units"></param>
        public static void Modify(UserChest[] chests)
        {
            foreach (var chest in chests)
            {
                var index = Array.FindIndex(cacheHomeRecv.chests, v => v.uniq == chest.uniq);
                if (index != -1)
                {
                    cacheHomeRecv.chests[index] = chest;
                }
                else
                {
                    cacheHomeRecv.chests = new[] { chest }.Concat(cacheHomeRecv.chests).ToArray();
                }
            }
        }
    }
}

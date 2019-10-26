using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using Zoo;

namespace Alice
{
    public static class UserData
    {
        public static Dictionary<string, UserUnit> editedUnit = new Dictionary<string, UserUnit>();

        /// <summary>
        /// ホーム情報をキャッシュする
        /// </summary>
        public static HomeRecv cacheHomeRecv { get; private set; }
        public static void CacheHomeRecv(HomeRecv homeRecv)
        {
            cacheHomeRecv = homeRecv;
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
        /// 差分を更新する
        /// </summary>
        public static void Modify(Modified modified)
        {
            if (modified == null) return;
            foreach (var player in modified.player)
            {
                cacheHomeRecv.player = player;
            }
            Modify(modified.unit);
            Modify(modified.skill);
            Modify(modified.chest);
            Remove(modified.remove);
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
        /// スキル更新
        /// </summary>
        /// <param name="units"></param>
        public static void Modify(UserSkill[] skills)
        {
            foreach (var skill in skills)
            {
                var index = Array.FindIndex(cacheHomeRecv.skills, v => v.id == skill.id);
                if (index != -1)
                {
                    cacheHomeRecv.skills[index] = skill;
                }
                else
                {
                    cacheHomeRecv.skills = new[] { skill }.Concat(cacheHomeRecv.skills).ToArray();
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
                    cacheHomeRecv.chests = cacheHomeRecv.chests.Concat(new[] { chest }).ToArray();
                }
            }
        }
        /// <summary>
        /// 宝箱削除
        /// </summary>
        /// <param name="units"></param>
        public static void Remove(UserChest[] chests)
        {
            cacheHomeRecv.chests = cacheHomeRecv.chests.Where(v => !Array.Exists(chests, c => c.uniq == v.uniq)).ToArray();
        }

        /// <summary>
        /// 編集したユニットを登録する
        /// </summary>
        /// <param name="unit"></param>
        public static void EditUnit(UserUnit unit)
        {
            editedUnit[unit.characterId] = unit;
        }
    }
}

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
        public static Dictionary<string, UserUnit> editedUnit = new Dictionary<string, UserUnit>();
        static Dictionary<string, int> skillCollections = new Dictionary<string, int>();

        /// <summary>
        /// 所持情報からスキルの装備状態を割り出す
        /// </summary>
        static void SkillCollections()
        {
            skillCollections.Clear();
            // まずは最大数を保持
            foreach (var skill in cacheHomeRecv.skills)
            {
                skillCollections[skill.id] = skill.count;
            }
            // ユニットに装備した分を減らす
            foreach (var unit in cacheHomeRecv.units)
            {
                foreach (var skill in unit.skill)
                {
                    --skillCollections[skill];
                }
            }
        }

        /// <summary>
        /// 指定したスキルの残り数
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static int RemainSkill(string id)
        {
            return skillCollections[id];
        }
        /// <summary>
        /// スキル変更
        /// </summary>
        /// <param name="setToUnit">ユニットに装備するスキルID</param>
        /// <param name="releaseFromUnit">ユニットから外したスキルID</param>
        public static bool ChangeSkill(string setToUnit, string releaseFromUnit)
        {
            if(skillCollections[setToUnit] <= 0)
            {
                return false;
            }
            if(skillCollections.ContainsKey(setToUnit))
            {
                --skillCollections[setToUnit];
            }
            if (skillCollections.ContainsKey(releaseFromUnit))
            {
                ++skillCollections[releaseFromUnit];
            }
            return true;
        }
        /// <summary>
        /// ホーム情報をキャッシュする
        /// </summary>
        public static HomeRecv cacheHomeRecv { get; private set; }
        public static void CacheHomeRecv(HomeRecv homeRecv)
        {
            cacheHomeRecv = homeRecv;
            SkillCollections();
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Xyz.AnzFactory.UI;
using Zoo;
using Alice.Entities;
using System;
using System.Linq;

namespace Alice
{
    public class SkillView : MonoBehaviour, ANZCellView.IDataSource, ANZCellView.IActionDelegate
    {
        [Flags]
        enum Filter
        {
            None = 0,               
            Damage = 1 << 0,     // ダメージ
            Recovery = 1 << 1,     // 回復
            Buff = 1 << 2,     // バフ
            Debuff = 1 << 3,     // デバフ
            Cancel = 1 << 4,     // 効果取消

            All = 0xFFFF,     // すべて
        };

        public ANZCellView cellView;
        public GameObject prefab;

        UserUnit cacheUnit;
        int cacheIndex;

        List<UserSkill> sortedSkill = new List<UserSkill>();
        Filter filter = Filter.All;

        void Start()
        {
            PrefabPool.Regist(prefab.name, prefab);
            cellView.DataSource = this;
            cellView.ActionDelegate = this;
        }

        bool FilterCheck(UserSkill skill)
        {
            var data = MasterData.Find(skill);
            Filter filter = Filter.None;
            foreach (var effect in data.Effects)
            {
                var type = MasterData.FindEffectByID(effect).Type;
                switch(type)
                {
                    case BattleConst.Effect.Damage:
                    case BattleConst.Effect.DamageRatio:
                        filter |= Filter.Damage;
                        break;
                    case BattleConst.Effect.Recovery:
                    case BattleConst.Effect.RecoveryRatio:
                        filter |= Filter.Recovery;
                        break;
                    case BattleConst.Effect.Buff_Atk:
                    case BattleConst.Effect.Buff_Def:
                    case BattleConst.Effect.Buff_MAtk:
                    case BattleConst.Effect.Buff_MDef:
                    case BattleConst.Effect.Buff_Wait:
                        filter |= Filter.Buff;
                        break;
                    case BattleConst.Effect.Debuff_Atk:
                    case BattleConst.Effect.Debuff_Def:
                    case BattleConst.Effect.Debuff_MAtk:
                    case BattleConst.Effect.Debuff_MDef:
                    case BattleConst.Effect.Debuff_Wait:
                        filter |= Filter.Debuff;
                        break;
                    default:
                        filter |= Filter.Cancel;
                        break;
                }
            }
            return (filter & this.filter) != 0;
        }

        void Setup()
        {
            sortedSkill.Clear();
            foreach (var skill in UserData.cacheHomeRecv.skills)
            {
                if (FilterCheck(skill))
                {
                    sortedSkill.Add(skill);
                }
            }
            sortedSkill.Sort((a, b) =>
            {
                return a.id.CompareTo(b.id);
            });
            cellView.ReloadData();
        }

        public void Open(UserUnit unit, int index)
        {
            this.gameObject.SetActive(true);
            this.cacheUnit = unit;
            this.cacheIndex = index;
            Setup();
        }

        public GameObject CellViewItem(int index, GameObject item)
        {
            if(item == null)
            {
                item = PrefabPool.Get(prefab.name);
            }
            var data = sortedSkill[index];
            item.GetComponent<SkillItem>().Setup(data);
            return item;
        }

        public Vector2 ItemSize()
        {
            return prefab.GetComponent<RectTransform>().sizeDelta;
        }

        public int NumOfItems()
        {
            return sortedSkill.Count;
        }

        public void TapCellItem(int index, GameObject listItem)
        {
            var data = sortedSkill[index];
            if (UserData.RemainSkill(data.id) <= 0) return;

            if (cacheUnit.skill.Length < 2)
            {
                Array.Resize(ref cacheUnit.skill, 2);
            }
            var before = cacheUnit.skill[cacheIndex];
            var after = data.id;

            if(UserData.ChangeSkill(after, before))
            {
                cacheUnit.skill[cacheIndex] = after;
                UserData.EditUnit(cacheUnit);
                Observer.Notify("HomeRecv");
            }
            this.gameObject.SetActive(false);
        }

        void ChangeFilter(Toggle toggle, Filter filter)
        {
            if(toggle.isOn)
            {
                this.filter = this.filter | filter;
            } else
            {
                this.filter = this.filter & ~filter;
            }
            Setup();
        }
        public void OnDamage(Toggle toggle)
        {
            ChangeFilter(toggle, Filter.Damage);
        }
        public void OnRecovery(Toggle toggle)
        {
            ChangeFilter(toggle, Filter.Recovery);
        }
        public void OnBuff(Toggle toggle)
        {
            ChangeFilter(toggle, Filter.Buff);
        }
        public void OnDebuff(Toggle toggle)
        {
            ChangeFilter(toggle, Filter.Debuff);
        }
        public void OnCancel(Toggle toggle)
        {
            ChangeFilter(toggle, Filter.Cancel);
        }
    }
}


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Xyz.AnzFactory.UI;
using Zoo;
using Alice.Entities;
using System;
using System.Linq;

namespace Alice
{
    public class SkillView : MonoBehaviour, ANZCellView.IDataSource, ANZCellView.IActionDelegate
    {
        public ANZCellView cellView;
        public GameObject prefab;

        UserUnit cacheUnit;
        int cacheIndex;

        UserSkill[] sortedSkill;

        void Start()
        {
            PrefabPool.Regist(prefab.name, prefab);
            cellView.DataSource = this;
            cellView.ActionDelegate = this;
        }

        public void Open(UserUnit unit, int index)
        {
            this.gameObject.SetActive(true);
            this.cacheUnit = unit;
            this.cacheIndex = index;

            sortedSkill = UserData.cacheHomeRecv.skills.ToArray();
            Array.Sort(sortedSkill, (a, b) =>
            {
                return a.id.CompareTo(b.id);
            });

            cellView.ReloadData();
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
            return sortedSkill.Length;
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
    }
}


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
            cellView.ReloadData();
        }

        public GameObject CellViewItem(int index, GameObject item)
        {
            if(item == null)
            {
                item = PrefabPool.Get(prefab.name);
            }
            var data = UserData.cacheHomeRecv.skills[index];
            item.GetComponent<SkillItem>().Setup(data);
            return item;
        }

        public Vector2 ItemSize()
        {
            return prefab.GetComponent<RectTransform>().sizeDelta;
        }

        public int NumOfItems()
        {
            return UserData.cacheHomeRecv.skills.Length;
        }

        public void TapCellItem(int index, GameObject listItem)
        {
            var data = UserData.cacheHomeRecv.skills[index];
            if(cacheUnit.skill.Length < 2)
            {
                Array.Resize(ref cacheUnit.skill, 2);
            }
            cacheUnit.skill[cacheIndex] = data.id;
            this.gameObject.SetActive(false);

            UserData.EditUnit(cacheUnit);

            Observer.Notify("HomeRecv");
        }
    }
}


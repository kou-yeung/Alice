using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Alice.Entities;

namespace Alice
{
    public class SkillItem : MonoBehaviour
    {
        public Text Name;
        public Text Desc;


        public void Setup(UserSkill skill)
        {
            var data = MasterData.Find(skill);
            Name.text = data.Name;
            Desc.text = Generic.Message.Desc(skill);
        }
    }
}

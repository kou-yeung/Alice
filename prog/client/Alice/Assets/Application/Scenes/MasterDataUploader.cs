using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zoo.Auth;
using Zoo.Communication;
using Alice.Entities;
using System;

namespace Alice.Tools
{
    [Serializable]
    class MasterDataSkill
    {
        public int rare;
        public string id;
    }
    [Serializable]
    class MasterDataCharacter
    {
        public int rare;
        public string id;
    }
    [Serializable]
    class MasterDataProduct
    {
        public string id;
        public string platform;
        public int alarm;
        public int bonus;
        public bool admin;
    }

    [Serializable]
    class MasterDataSend
    {
        public MasterDataSkill[] skills;
        public MasterDataCharacter[] characters;
        public MasterDataProduct[] products;
    }
    public class MasterDataUploader : MonoBehaviour
    {
        private void Start()
        {
            AuthService.Instance.SignInAnonymously();
            MasterData.Initialize(() => { });
        }

        public void OnSkill()
        {
            var c2s = new MasterDataSend();
            List<MasterDataSkill> skills = new List<MasterDataSkill>();
            foreach(var skill in MasterData.Instance.skills)
            {
                skills.Add(new MasterDataSkill { id = skill.ID, rare = skill.Rare });
            }

            c2s.skills = skills.ToArray();
            CommunicationService.Instance.Request("MasterData", JsonUtility.ToJson(c2s));
        }

        public void OnCharacter()
        {
            var c2s = new MasterDataSend();
            List<MasterDataCharacter> characters = new List<MasterDataCharacter>();
            foreach (var character in MasterData.Instance.characters)
            {
                characters.Add(new MasterDataCharacter { id = character.ID, rare = character.Rare });
            }

            c2s.characters = characters.ToArray();
            CommunicationService.Instance.Request("MasterData", JsonUtility.ToJson(c2s));
        }

        public void OnProduct()
        {
            var c2s = new MasterDataSend();
            List<MasterDataProduct> products = new List<MasterDataProduct>();
            foreach (var product in MasterData.Instance.products)
            {
                products.Add(new MasterDataProduct
                {
                    id = product.ID,
                    platform = product.Platform.ToString(),
                    alarm = product.Alarm,
                    bonus = product.Bonus,
                    admin = product.AdminOnly,
                });
            }
            c2s.products = products.ToArray();
            CommunicationService.Instance.Request("MasterData", JsonUtility.ToJson(c2s));
        }

        /// <summary>
        /// 部屋IDを初期化する
        /// </summary>
        public void OnGenRoomIds()
        {
            // 確認します
            Dialog.Show("サーバ負荷が高いですが、本当に実行しますか？", Dialog.Type.OKCancel, () =>
            {
                CommunicationService.Instance.Request("GenRoomIds", "");
            });
        }
    }
}

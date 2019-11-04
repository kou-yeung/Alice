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
    class MasterDataSend
    {
        public MasterDataSkill[] skills;
        public MasterDataCharacter[] characters;
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
            foreach(var skill in MasterData.skills)
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
            foreach (var character in MasterData.characters)
            {
                characters.Add(new MasterDataCharacter { id = character.ID, rare = character.Rare });
            }

            c2s.characters = characters.ToArray();
            CommunicationService.Instance.Request("MasterData", JsonUtility.ToJson(c2s));
        }

        /// <summary>
        /// 部屋IDを初期化する
        /// </summary>
        public void OnGenRoomIds()
        {
            // 確認します
            PlatformDialog.Show("サーバ負荷が高いですが、本当に実行しますか？", PlatformDialog.Type.OKCancel, () =>
            {
                CommunicationService.Instance.Request("GenRoomIds", "");
            });
        }
    }
}

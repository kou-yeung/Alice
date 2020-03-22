using Alice.Entities;
using UnityEditor;
using System.Linq;
using System.Collections.Generic;
using Alice;
using System;
using Zoo.Communication;
using UnityEngine;

namespace Alice.Admin
{
    /// <summary>
    /// 管理者コマンド : c2s
    /// </summary>
    [Serializable]
    public class AdminCommandSend
    {
        public string command;
        public string[] param;
    }
    /// <summary>
    /// 管理者コマンド : s2c
    /// </summary>
    [Serializable]
    public class AdminCommandRecv
    {
        public Modified modified;       // 更新したデータ
    }

    public class Command
    {
        [MenuItem("デバッグコマンド/キャラすべて入手")]
        public static void GetAllCharacter()
        {
            var ids = new List<string>();

            foreach(var data in MasterData.Instance.characters)
            {
                if (UserData.cacheHomeRecv.units.Any(v => v.characterId == data.ID)) continue;
                ids.Add(data.ID);
                ids.Add(data.Rare.ToString());
            }

            if (ids.Any())
            {
                var c2s = new AdminCommandSend { command = "AddCharacter", param = ids.ToArray() };
                CommunicationService.Instance.Request("AdminCommand", JsonUtility.ToJson(c2s), res =>
                {
                    UserData.Modify(JsonUtility.FromJson<AdminCommandRecv>(res).modified);
                });
            }
        }

        [MenuItem("デバッグコマンド/スキルすべて入手")]
        public static void GetAllSkill()
        {
            var ids = new List<string>();

            foreach (var data in MasterData.Instance.skills)
            {
                ids.Add(data.ID);
            }

            if (ids.Any())
            {
                var c2s = new AdminCommandSend { command = "AddSkill", param = ids.ToArray() };
                CommunicationService.Instance.Request("AdminCommand", JsonUtility.ToJson(c2s), res =>
                {
                    UserData.Modify(JsonUtility.FromJson<AdminCommandRecv>(res).modified);
                });
            }
        }

    }
}

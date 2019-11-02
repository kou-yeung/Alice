using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zoo.Communication;

namespace Alice
{
    public class VS : MonoBehaviour
    {
        public Battle battle;
        public InputField input;
        public Button btnFind;

        /// <summary>
        /// シャドウを生成する
        /// </summary>
        public void OnCreate()
        {
            var c2s = new ShadowCreateSend();
            CommunicationService.Instance.Request("CreateShadow", JsonUtility.ToJson(c2s), res =>
            {
            });
        }

        public void OnBattle()
        {
            int roomid = 0;
            int.TryParse(input.text.TrimStart('0'), out roomid);
            var c2s = new ShadowBattleSend(roomid);
            CommunicationService.Instance.Request("BattleShadow", JsonUtility.ToJson(c2s), res =>
            {
                UserData.editedUnit.Clear();    // 同期しました
                var s2c = JsonUtility.FromJson<BattleStartRecv>(res);
                battle.Exec(s2c);
            });
        }

        /// <summary>
        /// ルームIDを変更しました
        /// </summary>
        /// <param name="input"></param>
        public void OnEndEdit(InputField input)
        {
            // ５文字以上入力したら有効
            btnFind.interactable = input.text.Length >= 5;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Alice
{
    public class Record : MonoBehaviour
    {
        public Battle battle;

        // 仮
        System.Random random = new System.Random();
        public void OnClick()
        {
            var record = UserData.GetBattleRecord();
            if (record.Count <= 0) return;
            var index = random.Next(0, record.Count);
            // 履歴再生する
            battle.Exec(record[index], true);
        }
    }
}

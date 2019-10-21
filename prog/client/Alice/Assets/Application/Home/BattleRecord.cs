using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;

namespace Alice
{
    public class BattleRecord
    {
        static readonly int RecordMax = 500;
        static readonly string fn = Path.Combine(Application.persistentDataPath, "BattleRecord.txt");

        List<string> recordStrings;
        List<BattleStartRecv> records;
        bool needSave = false;

        /// <summary>
        /// レコード数
        /// </summary>
        public int Count{ get { return recordStrings.Count; } }

        public BattleStartRecv this[int index]
        {
            get
            {
                if(records[index] == null)
                {
                    records[index] = JsonUtility.FromJson<BattleStartRecv>(recordStrings[index]);
                }
                return records[index];
            }
            set
            {
                records[index] = value;
                recordStrings[index] = JsonUtility.ToJson(value);
                needSave = true;
            }
        }


        /// <summary>
        /// ファイルがあればロードし、なければ空きリストで初期化する
        /// </summary>
        public BattleRecord()
        {
            Debug.Log($"BattleRecord Path : {fn}");
            if(File.Exists(fn))
            {
                recordStrings = File.ReadLines(fn).ToList();
            }
            else
            {
                recordStrings = new List<string>();
            }
            // パース済みのデータを保持するリストの初期化
            records = new List<BattleStartRecv>(recordStrings.Count);
            records.AddRange(Enumerable.Repeat<BattleStartRecv>(null, recordStrings.Count));
        }

        /// <summary>
        /// バトル内容を保持する
        /// </summary>
        /// <param name="recv"></param>
        public void AddRecord(BattleStartRecv recv)
        {
            records.Insert(0, recv);
            recordStrings.Insert(0, JsonUtility.ToJson(recv));
            needSave = true;
        }

        /// <summary>
        /// ストレージに最新状態を保存する
        /// </summary>
        public void Save()
        {
            // 更新の必要なし
            if (!needSave) return;
            // 最新の {RecordMax} 件のみ書き出す
            File.WriteAllLines(fn, recordStrings.Take(RecordMax));
            needSave = false;
        }
    }
}


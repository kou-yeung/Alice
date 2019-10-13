using UnityEngine;
using System;
using System.Linq;
using Zoo.IO;
using System.IO;
using CsvHelper;
using Zoo;

namespace Alice.Entities
{
    /// <summary>
    /// マスタデータを管理するクラス
    /// </summary>
    public class MasterData
    {
        public static Character[] characters { get; private set; }

        public static void Initialize(Action cb)
        {
            Async.Parallel( cb,
                (end) => Load<Character>("Entities/Character.csv", (res) =>
                {
                    characters = res;
                }, end)
            );
        }
        static void Load<T>(string path, Action<T[]> cb, Action end)
        {
            LoaderService.Instance.Preload(new[] { path }, ()=>
            {
                var asset = LoaderService.Instance.Load<TextAsset>(path);
                using (var csv = new CsvReader(new StringReader(asset.text), CsvHelperRegister.configuration))
                {
                    cb?.Invoke(csv.GetRecords<T>().ToArray());
                }
                end?.Invoke();
            });
        }
    }
}

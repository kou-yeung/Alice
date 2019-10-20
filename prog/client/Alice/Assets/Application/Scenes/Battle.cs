using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Zoo;
using Zoo.IO;
using Zoo.Communication;
using Alice.Entities;

namespace Alice
{
    public class Battle : MonoBehaviour
    {
        public static Battle Instance { get; private set; }
        // ロジック抽選用乱数
        public System.Random random { get; private set; }
        public Button buttonAction;
        public BattleController controller { get; private set; }

        [Serializable]
        public struct Timeline
        {
            public Transform root;
            public Transform[] nodes;
        }
        public Timeline timeline;

        void Start()
        {
            Instance = this;
            controller = new BattleController(this);

            // バトル情報を取得する
            CommunicationService.Instance.Request("Battle", "", (res) =>
            {
                var battleRecv = JsonUtility.FromJson<BattleStartRecv>(res);
                this.random = new System.Random(battleRecv.seed);

                // 必要なリソースをプリロード
                List<string> resourcePaths = new List<string>();

                // 
                resourcePaths.Add("Prefab/Actor.prefab");
                resourcePaths.Add("Prefab/FX.prefab");
                resourcePaths.Add("Prefab/Phase.prefab");
                resourcePaths.Add("Prefab/UnitState.prefab");
                resourcePaths.Add("Prefab/TimelineIcon.prefab");
                resourcePaths.Add($"Effect/{Effect.Empty.FX}.asset");

                // 味方ユニットに必要なリソースをロード
                foreach(var unit in battleRecv.player)
                {
                    resourcePaths.AddRange(UserUnitPreloadPaths(unit));
                }
                // 相手ユニットに必要なリソースをロード
                foreach (var unit in battleRecv.enemy)
                {
                    resourcePaths.AddRange(UserUnitPreloadPaths(unit));
                }

                LoaderService.Instance.Preload(resourcePaths.Distinct().ToArray(), () =>
                {
                    // コントローラ初期化
                    controller.Setup(battleRecv);
                    // ステート開始
                    controller.ChangeState(BattleConst.State.Init);
                });
            });
        }

        void OnDestroy()
        {
            controller?.Dispose();
        }

        List<string> UserUnitPreloadPaths(UserUnit unit)
        {
            List<string> paths = new List<string>();

            // キャラマスタデータ
            var character = MasterData.characters.First(v => v.ID == unit.characterId);
            // キャラセルアニメーション
            paths.Add($"Character/{character.Image}/walk.asset");
            // アイコン
            paths.Add($"Character/{character.Image}/icon.asset");
            // エフェクト
            foreach (var skill in unit.skill)
            {
                // スキルマスタデータ
                var skillData = MasterData.skills.First(v => v.ID == skill);
                foreach(var effect in skillData.Effects)
                {
                    var effectData = MasterData.effects.First(v => v.ID == effect);
                    paths.Add($"Effect/{effectData.FX}.asset");
                }
            }
            return paths;
        }
    }
}

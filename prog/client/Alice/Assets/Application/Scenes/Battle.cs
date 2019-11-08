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
        public BattleController controller { get; private set; }
        public BattleStartRecv recv { get; private set; }
        public bool fromRecord { get; private set; }

        [Serializable]
        public struct Timeline
        {
            public Transform root;
            public Transform[] nodes;
        }
        public Timeline timeline;
        public Button btnSkip;

        void Awake()
        {
            Instance = this;
        }

        /// <summary>
        /// 実行する
        /// </summary>
        public void Exec(BattleStartRecv recv, bool fromRecord = false)
        {
            gameObject.SetActive(true);

            controller = new BattleController(this);

            this.recv = recv;
            this.fromRecord = fromRecord;
            this.random = new System.Random(this.recv.seed);

            // 必要なリソースをプリロード
            List<string> resourcePaths = new List<string>();

            // 
            resourcePaths.Add("Prefab/Actor.prefab");
            resourcePaths.Add("Prefab/FX.prefab");
            resourcePaths.Add("Prefab/Phase.prefab");
            resourcePaths.Add("Prefab/Versus.prefab");
            resourcePaths.Add("Prefab/UnitState.prefab");
            resourcePaths.Add("Prefab/TimelineIcon.prefab");
            resourcePaths.Add($"Effect/{Effect.Empty.FX}.asset");

            // 味方ユニットに必要なリソースをロード
            foreach(var unit in this.recv.playerUnit)
            {
                resourcePaths.AddRange(UserUnitPreloadPaths(unit));
            }
            // 相手ユニットに必要なリソースをロード
            foreach (var unit in this.recv.enemyUnit)
            {
                resourcePaths.AddRange(UserUnitPreloadPaths(unit));
            }

            LoaderService.Instance.Preload(resourcePaths.Distinct().ToArray(), () =>
            {
                // コントローラ初期化
                controller.Setup(this.recv);
                // ステート開始
                controller.ChangeState(BattleConst.State.Init);
                // スキップボタン
                btnSkip.gameObject.SetActive(this.fromRecord);
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
            foreach (var skill in unit.skill.Where(v => !string.IsNullOrEmpty(v)))
            {
                // スキルマスタデータ
                var skillData = MasterData.FindSkillByID(skill);
                foreach(var effect in skillData.Effects)
                {
                    var effectData = MasterData.effects.First(v => v.ID == effect);
                    paths.Add($"Effect/{effectData.FX}.asset");
                }
            }
            return paths;
        }

        /// <summary>
        /// 実行した試合結果に更新する
        /// </summary>
        /// <param name="result"></param>
        public void SetBattleResult(BattleConst.Result result)
        {
            this.recv.result = result;
        }


        /// <summary>
        /// 演出スキップボタン
        /// </summary>
        public void OnSkip()
        {
            controller.skip = true;
            btnSkip.gameObject.SetActive(false);
        }
    }
}

/// <summary>
/// バトル全体を制御するクラス
/// </summary>
using System;
using System.Collections.Generic;
using Zoo.StateMachine;

namespace Alice
{
    public class BattleController : IDisposable
    {
        Battle owner;
        StateBehaviour<Battle, BattleConst.State> stateBehaviour;
        BattleStartRecv recv;
        string[] sortedUniqs { get; set; }

        public Dictionary<string, BattleUnit> units { get; private set; } = new Dictionary<string, BattleUnit>();
        public string currentActionUniq { get { return sortedUniqs[0]; } }
        public BattleAction currentAction { get; private set; }

        public BattleController(Battle owner)
        {
            this.owner = owner;
            stateBehaviour = new StateBehaviour<Battle, BattleConst.State>(owner);
            stateBehaviour.AddState(BattleConst.State.Init, new BattleInitState());
            stateBehaviour.AddState(BattleConst.State.Start, new BattleStartState());
            stateBehaviour.AddState(BattleConst.State.Action, new BattleActionState());
            stateBehaviour.AddState(BattleConst.State.Playback, new BattlePlaybackState());
            stateBehaviour.AddState(BattleConst.State.Timeline, new BattleTimelineState());
        }

        public void Dispose()
        {
            stateBehaviour?.Dispose();
        }

        public void DoAction()
        {
            ChangeState(BattleConst.State.Playback);
        }

        /// <summary>
        /// ステートマシンの遷移
        /// </summary>
        /// <param name="state"></param>
        public void ChangeState(BattleConst.State state)
        {
            stateBehaviour.ChangeState(state);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Setup(BattleStartRecv recv)
        {
            this.recv = recv;
            CreatePlayerUnit(recv.player);
            CreateEnemyUnit(recv.enemy);
        }

        /// <summary>
        /// ソート済みのUniq配列更新
        /// </summary>
        /// <param name="uniqs"></param>
        public void UpdateSortedUniqs(string[] sortedUniqs)
        {
            this.sortedUniqs = sortedUniqs;
        }

        /// <summary>
        /// 行動を保持する
        /// </summary>
        /// <param name="action"></param>
        public void CurrentAction(BattleAction action)
        {
            this.currentAction = action;
        }

        /// <summary>
        /// プレイヤーユニット生成
        /// </summary>
        void CreatePlayerUnit(string[] ids)
        {
            for (int i = 0; i < ids.Length; i++)
            {
                var uniq = $"PLAYER:{i}";// Guid.NewGuid().ToString();
                var unit = new BattleUnit(uniq, ids[i], BattleConst.Side.Player);
                units.Add(uniq, unit);

                var transform = unit.image.transform;
                transform.SetParent(this.owner.transform);
                transform.localPosition = BattleConst.PlayerUnitPositions[i];
            }
        }
        /// <summary>
        /// エネミーユニット生成
        /// </summary>
        void CreateEnemyUnit(string[] ids)
        {
            for (int i = 0; i < ids.Length; i++)
            {
                var uniq = $"ENEMY:{i}";//Guid.NewGuid().ToString();
                var unit = new BattleUnit(uniq, ids[i], BattleConst.Side.Enemy);
                units.Add(uniq, unit);

                var transform = unit.image.transform;
                transform.SetParent(this.owner.transform);
                transform.localPosition = BattleConst.EnemyUnitPositions[i];
            }
        }
    }
}


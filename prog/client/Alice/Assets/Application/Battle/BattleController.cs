/// <summary>
/// バトル全体を制御するクラス
/// </summary>
using System;
using System.Collections.Generic;
using Zoo.StateMachine;
using UnityEngine;
using Zoo.IO;

namespace Alice
{
    public class BattleController : IDisposable
    {
        Battle owner;
        StateBehaviour<Battle, BattleConst.State> stateBehaviour;
        BattleStartRecv recv;
        List<BattleUnit> sortedBattleUnits { get; set; }

        public Dictionary<string, BattleUnit> units { get; private set; } = new Dictionary<string, BattleUnit>();
        public BattleUnit currentActionBattleUnit { get { return sortedBattleUnits[0]; } }
        public BattleAction currentAction { get; private set; }
        public Phase phase { get; private set; }
        public Dictionary<string, TimelineIcon> timeline { get; private set; } = new Dictionary<string, TimelineIcon>();

        public BattleController(Battle owner)
        {
            this.owner = owner;
            stateBehaviour = new StateBehaviour<Battle, BattleConst.State>(owner);
            stateBehaviour.AddState(BattleConst.State.Init, new BattleInitState());
            stateBehaviour.AddState(BattleConst.State.Start, new BattleStartState());
            stateBehaviour.AddState(BattleConst.State.Action, new BattleActionState());
            stateBehaviour.AddState(BattleConst.State.Timeline, new BattleTimelineState());
            stateBehaviour.AddState(BattleConst.State.TurnEnd, new BattleTurnEndState());
            stateBehaviour.AddState(BattleConst.State.GameSet, new BattleGameSetState());
            stateBehaviour.AddState(BattleConst.State.Passive, new BattlePassiveState());
            stateBehaviour.AddState(BattleConst.State.Finally, new BattleFinallyState());
        }

        public void Dispose()
        {
            stateBehaviour?.Dispose();
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
            CreatePlayerUnit(recv);
            CreateEnemyUnit(recv);
            phase = Phase.Gen(this.owner.transform);
        }

        /// <summary>
        /// タイムラインによるソート済みのBattleUnit配列更新
        /// </summary>
        /// <param name="uniqs"></param>
        public void UpdateSortedBattleUnits(List<BattleUnit> sortedBattleUnits)
        {
            this.sortedBattleUnits = sortedBattleUnits;
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
        void CreatePlayerUnit(BattleStartRecv recv)
        {
            foreach(var data in recv.player)
            {
                var uniq = $"PLAYER:{data.position}";// Guid.NewGuid().ToString();
                var unit = new BattleUnit(uniq, data, BattleConst.Side.Player);
                units.Add(uniq, unit);

                var transform = unit.root.transform;
                transform.SetParent(this.owner.transform, false);
                transform.localPosition = BattleConst.PlayerUnitPositions[data.position];

                // タイムラインアイコン登録
                var icon = TimelineIcon.Gen(unit);
                timeline[uniq] = icon;
                icon.transform.SetParent(owner.timeline.root, false);
                icon.transform.localPosition = Battle.Instance.timeline.nodes[0].localPosition;
            }
        }
        /// <summary>
        /// エネミーユニット生成
        /// </summary>
        void CreateEnemyUnit(BattleStartRecv recv)
        {
            foreach (var data in recv.enemy)
            {
                var uniq = $"ENEMY:{data.position}";//Guid.NewGuid().ToString();
                var unit = new BattleUnit(uniq, data, BattleConst.Side.Enemy);
                units.Add(uniq, unit);

                var transform = unit.root.transform;
                transform.SetParent(this.owner.transform, false);
                transform.localPosition = BattleConst.EnemyUnitPositions[data.position];

                // タイムラインアイコン登録
                var icon = TimelineIcon.Gen(unit);
                timeline[uniq] = icon;
                icon.transform.SetParent(owner.timeline.root, false);
                icon.transform.localPosition = Battle.Instance.timeline.nodes[0].localPosition;
            }
        }
    }
}


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
        Dictionary<string, BattleUnit> units = new Dictionary<string, BattleUnit>();

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

        //public void Setup()
        //{
        //    var uniq = Guid.NewGuid().ToString();
        //    units.Add(uniq, new BattleUnit(uniq));
        //}

        //public BattleUnit CreateUnit(string uniq)
        //{
        //    var res = new BattleUnit(uniq);
        //    units[uniq] = res;
        //    return res;
        //}

        /// <summary>
        /// プレイヤーユニット生成
        /// </summary>
        public void CreatePlayerUnit()
        {
            for (int i = 0; i < 4; i++)
            {
                var uniq = Guid.NewGuid().ToString();
                var unit = new BattleUnit(uniq, i + 1, BattleConst.Side.Player);
                units.Add(uniq, unit);

                var transform = unit.image.transform;
                transform.SetParent(this.owner.transform);
                transform.localPosition = BattleConst.PlayerUnitPositions[i];
            }
        }
        /// <summary>
        /// エネミーユニット生成
        /// </summary>
        public void CreateEnemyUnit()
        {
            for (int i = 0; i < 4; i++)
            {
                var uniq = Guid.NewGuid().ToString();
                var unit = new BattleUnit(uniq, i + 5, BattleConst.Side.Enemy);
                units.Add(uniq, unit);

                var transform = unit.image.transform;
                transform.SetParent(this.owner.transform);
                transform.localPosition = BattleConst.EnemyUnitPositions[i];
            }
        }
    }
}


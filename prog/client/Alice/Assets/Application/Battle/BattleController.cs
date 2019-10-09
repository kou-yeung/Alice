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
        StateBehaviour<Battle, BattleConst.State> stateBehaviour;
        Dictionary<string, BattleUnit> units = new Dictionary<string, BattleUnit>();

        public BattleController(Battle owner)
        {
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

        public void Setup()
        {
            var uniq = Guid.NewGuid().ToString();
            units.Add(uniq, new BattleUnit(uniq));
        }

        public void CreateUnit(string uniq)
        {
            units[uniq] = new BattleUnit(uniq);
        }
    }
}


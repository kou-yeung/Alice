using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zoo.StateMachine;

namespace Alice
{
    public class Battle : MonoBehaviour
    {
        public Button buttonAction;

        StateBehaviour<Battle, BattleConst.State> stateBehaviour;

        void Start()
        {
            stateBehaviour = new StateBehaviour<Battle, BattleConst.State>(this);
            stateBehaviour.AddState(BattleConst.State.Start, new BattleStartState());
            stateBehaviour.AddState(BattleConst.State.Action, new BattleActionState());
            stateBehaviour.AddState(BattleConst.State.Playback, new BattlePlaybackState());
            stateBehaviour.AddState(BattleConst.State.Timeline, new BattleTimelineState());

            // 開始
            stateBehaviour.ChangeState(BattleConst.State.Start);
        }

        void OnDestroy()
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


        public void OnAction()
        {
            ChangeState(BattleConst.State.Playback);
        }

        public void EnableAction(bool enable)
        {
            buttonAction.interactable = enable;
        }
    }
}

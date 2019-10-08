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
        public BattleController controller { get; private set; }

        void Start()
        {
            controller = new BattleController(this);
            controller.ChangeState(BattleConst.State.Start);
        }

        void OnDestroy()
        {
            controller?.Dispose();
        }

        public void OnAction()
        {
            controller.DoAction();
            //ChangeState(BattleConst.State.Playback);
        }

        public void EnableAction(bool enable)
        {
            buttonAction.interactable = enable;
        }
    }
}

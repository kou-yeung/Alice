using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zoo.IO;

namespace Alice
{
    public class Battle : MonoBehaviour
    {
        public Button buttonAction;
        public BattleController controller { get; private set; }

        void Start()
        {
            controller = new BattleController(this);
            List<string> resourcePaths = new List<string>();
            for (int i = 1; i <= 10; i++)
            {
                resourcePaths.Add(string.Format("Character/$yuhinamv{0:D3}.asset", i));
            }
            resourcePaths.Add("Character.prefab");
            resourcePaths.Add("MasterData/Character.csv");

            LoaderService.Instance.Preload(resourcePaths.ToArray(), ()=>
            {
                //foreach(var path in resourcePaths)
                //{
                //    var o = LoaderService.Instance.Load<object>(path);
                //}
                // ユニット初期化
                controller.CreatePlayerUnit();
                controller.CreateEnemyUnit();
                // ステート開始
                controller.ChangeState(BattleConst.State.Init);
            });
        }

        void OnDestroy()
        {
            controller?.Dispose();
        }

        public void OnAction()
        {
            controller.DoAction();
        }

        public void EnableAction(bool enable)
        {
            buttonAction.interactable = enable;
        }
    }
}

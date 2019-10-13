using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zoo.IO;
using Zoo.Communication;

namespace Alice
{
    public class Battle : MonoBehaviour
    {
        public Button buttonAction;
        public BattleController controller { get; private set; }

        void Start()
        {
            controller = new BattleController(this);

            // バトル情報を取得する
            CommunicationService.Instance.Request("Battle", "", (res) =>
            {
                Debug.Log(res);
                // 必要なリソースをプリロード
                List<string> resourcePaths = new List<string>();
                for (int i = 1; i <= 10; i++)
                {
                    resourcePaths.Add(string.Format("Character/$yuhinamv{0:D3}.asset", i));
                }
                resourcePaths.Add("Character.prefab");
                LoaderService.Instance.Preload(resourcePaths.ToArray(), () =>
                {
                    // コントローラ初期化
                    controller.Setup(JsonUtility.FromJson<BattleStartRecv>(res));
                    // ステート開始
                    controller.ChangeState(BattleConst.State.Init);
                });
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

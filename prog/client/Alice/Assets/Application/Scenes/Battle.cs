using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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

        void Start()
        {
            Instance = this;
            controller = new BattleController(this);

            // バトル情報を取得する
            CommunicationService.Instance.Request("Battle", "", (res) =>
            {
                Debug.Log(res);
                this.random = new System.Random();

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
    }
}

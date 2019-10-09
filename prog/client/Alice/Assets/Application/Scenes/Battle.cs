﻿using System.Collections;
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
            resourcePaths.Add("Character/$yuhinamv001.png");
            resourcePaths.Add("Character.prefab");

            LoaderService.Instance.Preload(resourcePaths.ToArray(), ()=>
            {
                foreach(var path in resourcePaths)
                {
                    var o = LoaderService.Instance.Load<object>(path);
                    Debug.Log($"{path}:{o.ToString()}");
                }
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

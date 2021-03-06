﻿using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Zoo.StateMachine;
using Zoo.Sound;

namespace Alice
{
    public class BattleInitState : IState<Battle>
    {
        public override void Begin(Battle owner)
        {
            // バトル開始時はスリープしないように
            Screen.sleepTimeout = SleepTimeout.NeverSleep;

            SoundService.Instance.PlayBGM(Const.BGM.Battle);
            owner.controller.versus.Show(owner.recv.names[0], owner.recv.names[1], () =>
            {
                owner.controller.ChangeState(BattleConst.State.Passive);
            });
        }
    }
}

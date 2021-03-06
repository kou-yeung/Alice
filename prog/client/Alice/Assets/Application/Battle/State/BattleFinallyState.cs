﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zoo.StateMachine;
using System.Linq;
using Zoo.Sound;
using Zoo.Time;
using Zoo.Communication;

namespace Alice
{
    public class BattleFinallyState : IState<Battle>
    {
        public override void Begin(Battle owner)
        {
            SoundService.Instance.PlayBGM(Const.BGM.Home);

            // 新規バトルの場合、試合履歴に追加
            if (!owner.fromRecord)
            {
                UserData.GetBattleRecord().AddRecord(owner.recv);
            }

            // タイムラインアイコンを回収する
            foreach (var kv in owner.controller.timeline)
            {
                kv.Value.Destroy();
            }
            owner.controller.timeline.Clear();

            // 必要なくなったものの後始末
            foreach (var unit in owner.controller.units)
            {
                unit.Value.Destory();
            }
            owner.controller.units.Clear();

            // 次のバトルを備えてタイムライン非表示
            owner.timeline.root.gameObject.SetActive(false);
            // バトルを非表示する
            owner.gameObject.SetActive(false);
            // フェーズを破棄
            owner.controller.phase.Destory();
            // VS.を破棄
            owner.controller.versus.Destory();

            // 自動スリープはシステム値に戻す
            Screen.sleepTimeout = SleepTimeout.SystemSetting;

            var player = UserData.cacheHomeRecv.player;
            // ランクの更新時間
            if (UserData.cacheHomeRecv.nextBonusTime < ServerTime.CurrentUnixTime)
            {
                player.Sync(() =>
                {
                    player.ExecTutorial(Const.TutorialFlag.HeaderRank);
                });
            } else
            {
                player.ExecTutorial(Const.TutorialFlag.HeaderRank);
            }
        }
    }
}

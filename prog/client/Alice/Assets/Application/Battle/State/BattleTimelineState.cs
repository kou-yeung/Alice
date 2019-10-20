using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zoo.StateMachine;
using System.Linq;
using Zoo;

namespace Alice
{
    public class BattleTimelineState : IState<Battle>
    {
        public override void Begin(Battle owner)
        {
            var units = owner.controller.units.Values.ToList();
            units.Sort((a, b) =>
            {
                var res = a.current.Wait.CompareTo(b.current.Wait);
                if (res == 0) res = a.side.CompareTo(b.side);
                if (res == 0) res = a.Position.CompareTo(b.Position);
                return res;
            });
            // 並び順更新
            owner.controller.UpdateSortedBattleUnits(units);

            // アイコンを並ぶ
            var function = Async.Passive(() =>
            {
                owner.controller.ChangeState(BattleConst.State.Action);
            }, units.Count);

            // 先頭ユニットの残り待ち時間
            var wait = units[0].current.Wait;
            for (int i = 0; i < units.Count; i++)
            {
                // 時間を進む:先頭UnitのWait時間分を減らす
                units[i].current.Wait -= wait;
                // タイムラインアイコンの補間移動
                var icon = owner.controller.timeline[units[i].uniq];
                icon.transform.SetAsLastSibling();
                LeanTween.moveLocal(icon.gameObject, owner.timeline.nodes[i].localPosition, 0.2f).setOnComplete(function);
            }
        }
    }
}

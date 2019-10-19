using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zoo.StateMachine;
using System.Linq;
using Zoo;
using System;

namespace Alice
{
    public class BattlePassiveState : IState<Battle>
    {
        public override void Begin(Battle owner)
        {
            List<BattleAction> passive = new List<BattleAction>();
            var units = owner.controller.units.Values.ToList();

            // 味方 > 相手 , 先頭 > 末尾で並ぶ
            units.Sort((a, b) =>
            {
                var res = a.side.CompareTo(b.side);
                if (res == 0) res = a.Position.CompareTo(b.Position);
                return res;
            });

            // パッシブスキルを実行する
            foreach (var unit in units)
            {
                foreach (var skill in unit.skills.Where(v => v.Passive))
                {
                    passive.Add(BattleLogic.Instance.Exec(new BattleAction(unit, skill)));
                }
            }

            if(passive.Count <= 0)
            {
                owner.controller.ChangeState(BattleConst.State.Start);
            } else
            {
                // 順番実行する
                Exec(passive, ()=>
                {
                    owner.controller.ChangeState(BattleConst.State.Start);
                });
            }
        }

        /// <summary>
        /// 再帰で実行する
        /// </summary>
        /// <param name="passive"></param>
        /// <param name="cb"></param>
        void Exec(List<BattleAction> passive, Action cb)
        {
            if (passive.Count <= 0)
            {
                cb();
                return;
            }
            var action = passive[0];
            passive.RemoveAt(0);
            BattlePlayback.Play(action, () => Exec(passive, cb));
        }
    }
}

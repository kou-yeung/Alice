using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

namespace Zoo
{
    public static class Async
    {
        /// <summary>
        /// 被動的非同期実行
        /// 返された Action を指定した回数実行すると end が実行されます
        /// </summary>
        /// <param name="end"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static Action Passive(Action end, int count)
        {
            var subject = new Subject<Unit>();
            subject.Take(count).Subscribe(_ => { }, () => end?.Invoke());
            return new Action(() => subject.OnNext(Unit.Default));
        }

        /// <summary>
        /// 並列非同期実行
        /// </summary>
        /// <param name="end"></param>
        /// <param name="tasks"></param>
        public static void Parallel(Action end, params Action<Action>[] tasks)
        {
            var function = Passive(end, tasks.Length);
            foreach (var task in tasks) task(function);
        }

        /// <summary>
        /// 直列非同期実行
        /// </summary>
        /// <param name="end"></param>
        /// <param name="tasks"></param>
        public static void Waterflow(Action end, params Action<Action>[] tasks)
        {
            /// MEMO : 実装があまりよくないのであとで修正します
            List<Action<Action>> actions = new List<Action<Action>>(tasks);
            var subject = new Subject<int>();
            subject
                .Take(tasks.Length + 1)
                .Subscribe(x =>
                {
                    if (x < tasks.Length) tasks[x](() => subject.OnNext(x + 1));
                    else subject.OnCompleted();
                }, () => end?.Invoke());
            subject.OnNext(0);
        }
    }
}


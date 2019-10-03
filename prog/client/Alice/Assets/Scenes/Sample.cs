using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;
using System.Linq;
using Zoo.State;

public class Sample : MonoBehaviour
{
    StateMachine<Sample> machine;
    enum State
    {
        Command,
        Playback,
        TurnEnd,
    }
    class CommandState : IState<Sample>
    {
        float dt = 0;
        public override void Begin(Sample owner)
        {
            dt = 0;
            Debug.Log("CommandState : Begin");
        }
        public override void Update(Sample owner, float deltaTime)
        {
            dt += deltaTime;
            if(dt >= 3)
            {
                owner.OnCommand();
            }
            Debug.Log("CommandState : Update");
        }
    }
    class PlaybackState : IState<Sample>
    {
        public override void Begin(Sample owner)
        {
            owner.Para(
                () => owner.OnPlaybackEnd(),
                (cb) => owner._Task(5, cb),
                (cb) => owner._Task(4, cb),
                (cb) => owner._Task(2, cb),
                (cb) => owner._Task(3, cb)
                );
        }
    }
    class TurnEndState : IState<Sample>
    {
        public override void Begin(Sample owner)
        {
            Debug.Log("TurnEnd : Begin");
        }
        public override void Update(Sample owner, float deltaTime)
        {
            Debug.Log("TurnEnd : Update");
        }
        public override void End(Sample owner)
        {
            Debug.Log("TurnEnd : End");
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        machine = new StateMachine<Sample>(this);
        machine.AddState(State.Command, new CommandState());
        machine.AddState(State.Playback, new PlaybackState());
        machine.AddState(State.TurnEnd, new TurnEndState());
        machine.ChangeState(State.Command);

        // 並列
        //Para(
        //    ()=> Debug.Log("End!!"),
        //    (cb) => _Task(5, cb),
        //    (cb) => _Task(4, cb),
        //    (cb) => _Task(2, cb),
        //    (cb) => _Task(3, cb)
        //    );

        WF(
            () => Debug.Log("End!!"),
            (cb) => _Task(1, cb),
            (cb) => _Task(4, cb),
            (cb) => _Task(2, cb),
            (cb) => _Task(3, cb));
        //Observable
        //    .WhenAll
        //    (
        //        Observable.FromCoroutine(() => Task(3)),
        //        Observable.FromCoroutine(() => Task(2)),
        //        Observable.FromCoroutine(() => Task(4))
        //    ).Subscribe(_ => Debug.Log("End"));

        //// 直列
        //Observable
        //    .FromCoroutine(() => Task(3))
        //    .SelectMany(() => Task(2))
        //    .SelectMany(() => Task(4))
        //    .Subscribe(_ => Debug.Log("End"));
    }

    // Update is called once per frame
    void Update()
    {
        machine.Update(Time.deltaTime);
    }

    public void OnCommand()
    {
        machine.ChangeState(State.Playback);
    }

    public void OnPlaybackEnd()
    {
        machine.ChangeState(State.TurnEnd);
    }

    IEnumerator Task(float sec)
    {
        Debug.Log($"Task Start : {sec}");
        yield return new WaitForSeconds(sec);
        Debug.Log($"Task End : {sec}");
    }
    IEnumerator TaskWith(float sec, Action cb)
    {
        Debug.Log($"Task Start : {sec}");
        yield return new WaitForSeconds(sec);
        Debug.Log($"Task End : {sec}");
        cb();
    }

    public void _Task(float sec, Action action)
    {
        StartCoroutine(TaskWith(sec, action));
    }
    void OnCompleted(IObserver<Unit> o)
    {
        o.OnCompleted();
    }

    void Para(Action end, params Action<Action>[] tasks)
    {
        // 並列
        List<IObservable<Unit>> array = new List<IObservable<Unit>>();
        for (int i = 0; i < tasks.Length; i++)
        {
            var task = tasks[i];
            array.Add(Observable.FromCoroutine<Unit>((o) => Exec(task, () => OnCompleted(o))));
        }
        Observable.WhenAll(array).Subscribe(_ => end?.Invoke());
    }

    void WF(Action end, params Action<Action>[] tasks)
    {
        //IObservable<Unit> observable = null;

        //for (int i = 0; i < tasks.Length; i++)
        //{
        //    var task = tasks[i];
        //    if(observable == null)
        //    {
        //        observable = Observable.FromCoroutine<Unit>((o) => Exec(task, () => OnCompleted(o)));
        //    }
        //    else
        //    {
        //        observable = observable.SelectMany(Observable.FromCoroutine<Unit>((o) => Exec(task, () => OnCompleted(o))));
        //    }
        //}
        //observable.Subscribe(_ => end?.Invoke());

        /*
        // 直列
        Observable
            .FromCoroutine(() => tasks)
            .SelectMany(() => Task(2))
            .SelectMany(() => Task(4))
            .Subscribe(_ => Debug.Log("End"));
            */
    }

    IEnumerator Exec(Action<Action> action, Action cb)
    {
        action(cb);
        yield break;
    }
}

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
            owner.Parallel(
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
        //machine.ChangeState(State.Command);

        // 並列
        Parallel(
            () => Debug.Log("Parallel End!!"),
            (cb) => _Task(5, cb),
            (cb) => _Task(4, cb),
            (cb) => _Task(2, cb),
            (cb) => _Task(3, cb));

        Waterflow(
            () => Debug.Log("Waterflow End!!"),
            (cb) => _Task(1, cb),
            (cb) => _Task(4, cb),
            (cb) => _Task(2, cb),
            (cb) => _Task(3, cb));

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



    // 並列
    void Parallel(Action end, params Action<Action>[] tasks)
    {
        var function = Passivity(end, tasks.Length);
        foreach (var task in tasks) task(function);
    }

    // 直列
    void Waterflow(Action end, params Action<Action>[] tasks)
    {
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

    // 被動的
    Action Passivity(Action end, int count)
    {
        var subject = new Subject<Unit>();
        subject.Take(count).Subscribe(_ => { }, () => end?.Invoke());
        return new Action(() => subject.OnNext(Unit.Default));
    }

    IEnumerator Exec(Action<Action> action, Action cb)
    {
        action(cb);
        yield break;
    }
}

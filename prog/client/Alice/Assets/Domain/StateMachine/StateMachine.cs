using System.Collections.Generic;
using UniRx;
using System;

namespace Zoo.StateMachine
{
    /// <summary>
    /// ステート
    /// </summary>
    /// <typeparam name="Owner"></typeparam>
    public class IState<Owner>
    {
        public virtual void Begin(Owner owner) { }
        public virtual void Update(Owner owner, float deltaTime) { }
        public virtual void End(Owner owner) { }
    }

    /// <summary>
    /// ステートマシンの振る舞い管理する
    /// </summary>
    /// <typeparam name="Owner"></typeparam>
    public class StateBehaviour<Owner, Key> : IDisposable
    {
        Dictionary<Key, IState<Owner>> stateDic = new Dictionary<Key, IState<Owner>>();
        IState<Owner> currentState;
        Owner owner;
        IDisposable task;

        public StateBehaviour(Owner owner)
        {
            this.owner = owner;
            this.task = Observable.EveryUpdate().Subscribe(_ => Update(UnityEngine.Time.deltaTime));
        }

        void Update(float deltaTime)
        {
            if (currentState != null) currentState.Update(owner, deltaTime);
        }

        /// <summary>
        /// ステートを追加する
        /// </summary>
        /// <typeparam name="Key"></typeparam>
        /// <param name="key"></param>
        /// <param name="state"></param>
        public void AddState(Key key, IState<Owner> state)
        {
            stateDic.Add(key, state);
        }

        /// <summary>
        /// ステートを削除する
        /// </summary>
        /// <typeparam name="Key"></typeparam>
        /// <param name="key"></param>
        public void RemoveState(Key key)
        {
            stateDic.Remove(key);
        }

        /// <summary>
        /// ステート変更
        /// </summary>
        /// <typeparam name="Key"></typeparam>
        /// <param name="key"></param>
        public void ChangeState(Key key)
        {
            if (currentState != null) currentState.End(owner);
            currentState = stateDic[key];
            currentState.Begin(owner);
        }

        /// <summary>
        /// 破棄
        /// </summary>
        public void Dispose()
        {
            stateDic.Clear();
            task?.Dispose();
        }
    }
}


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Zoo.State
{

    //public class StateSystem : MonoBehaviour
    //{
    //    static StateSystem instance;
    //    static List<StateMachine> machines = new List<StateMachine>();

    //    static StateSystem()
    //    {
    //        var go = new GameObject("StateSystem");
    //        instance = go.AddComponent<StateSystem>();
    //        DontDestroyOnLoad(go);
    //    }

    //    public static StateMachine Gen()
    //    {
    //        var res = new StateMachine();
    //        machines.Add(res);
    //        return res;
    //    }
    //}

    public class IState<Owner>
    {
        public virtual void Begin(Owner owner) { }
        public virtual void Update(Owner owner, float deltaTime) { }
        public virtual void End(Owner owner) { }
    }

    public class StateMachine<Owner>
    {
        Dictionary<object, IState<Owner>> stateDic = new Dictionary<object, IState<Owner>>();
        IState<Owner> currentState;
        Owner owner;

        public StateMachine(Owner owner)
        {
            this.owner = owner;
        }

        public void Update(float deltaTime)
        {
            if (currentState != null) currentState.Update(owner, deltaTime);
        }

        public void AddState<Key>(Key key, IState<Owner> state)
        {
            stateDic.Add(key, state);
        }
        public void RemoveState<Key>(Key key)
        {
            stateDic.Remove(key);
        }

        public void ChangeState<Key>(Key key)
        {
            if (currentState != null) currentState.End(owner);
            currentState = stateDic[key];
            currentState.Begin(owner);
        }

    }
}


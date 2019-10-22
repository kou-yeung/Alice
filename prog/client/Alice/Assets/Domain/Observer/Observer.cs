using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Zoo
{
    /// <summary>
    /// 
    /// </summary>
    public class Observer
    {

        static Dictionary<string, Action> observer = new Dictionary<string, Action>();

        public static void AddObserver(string name, Action cb)
        {
            if(observer.ContainsKey(name))
            {
                observer[name] += cb;
            } else
            {
                observer[name] = cb;
            }
        }

        public static void Notify(string name)
        {
            Action action;
            if (observer.TryGetValue(name, out action))
            {
                action.Invoke();
            }
        }
    }
}

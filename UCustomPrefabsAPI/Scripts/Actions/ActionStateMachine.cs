using System;
using System.Collections.Generic;
namespace UCustomPrefabsAPI
{
    //TODO Reorganize a few things, to make it easier to add custom "events"
    public class ActionStateMachine
    {
        private class StateActions
        {
            public List<Action<string>> OnEnter = new();
            public List<Action> OnUpdate = new();
            public List<Action<string>> OnExit = new();
        }
        public string State { get; private set; } = string.Empty;
        private Dictionary<string, StateActions> _StatesActions = new();
        private List<Action<string, string>> OnStateChanged = new();
        private List<Action> OnUpdate = new();
        private List<Action> OnDestroy = new();
        private StateActions _CurrentActions = null;
        public bool HasState(string name)
        {
            return _StatesActions.ContainsKey(name);
        }
        public void AddState(string name)
        {
            Internal_AddState(name);
        }
        private StateActions Internal_AddState(string name, bool addState = true)
        {
            if (!_StatesActions.TryGetValue(name, out var actions) && addState)
            {
                actions = new();
                _StatesActions.Add(name, actions);
            }
            return actions;
        }
        public void AddOnEnter(string name, Action<string> action, bool addState = false)
        {
            var actions = Internal_AddState(name, addState);
            actions?.OnEnter.Add(action);
        }
        public void AddOnUpdate(string name, Action action, bool addState = false)
        {
            var actions = Internal_AddState(name, addState);
            actions?.OnUpdate.Add(action);
        }
        public void AddOnExit(string name, Action<string> action, bool addState = false)
        {
            var actions = Internal_AddState(name, addState);
            actions?.OnExit.Add(action);
        }
        public void AddOnStateChanged(Action<string, string> action)
        {
            OnStateChanged.Add(action);
        }
        public void AddOnUpdate(Action action)
        {
            OnUpdate.Add(action);
        }
        public void AddOnDestroy(Action action)
        {
            OnDestroy.Add(action);
        }
        public void Do_OnEnter()
        {
            _CurrentActions?.OnEnter.ForEach
            (
                (action) =>
                {
                    try
                    {
                        action?.Invoke(State);
                    }
                    catch (Exception e)
                    {
                        UnityEngine.Debug.LogError(e);
                    }
                }
            );
        }
        public void Do_OnExit()
        {
            _CurrentActions?.OnExit.ForEach
            (
                (action) =>
                {
                    try
                    {
                        action?.Invoke(State);
                    }
                    catch (Exception e)
                    {
                        UnityEngine.Debug.LogError(e);
                    }
                }
            );
        }
        public void Do_OnStateChanged(string lastState)
        {
            OnStateChanged.ForEach
            (
                (action) =>
                {
                    try
                    {
                        action?.Invoke(lastState, State);
                    }
                    catch (Exception e)
                    {
                        UnityEngine.Debug.LogError(e);
                    }
                }
            );
        }
        public void SetState(string name)
        {
            if (State == name)
                return;
            //<--Do On Exit?
            _StatesActions.TryGetValue(name, out _CurrentActions);
            State = name;
            //<--Do On State Changed?
            //<--Do On Enter?
        }
        public void Update()
        {
            _CurrentActions?.OnUpdate.ForEach((action) => action?.Invoke());
            OnUpdate.ForEach
            (
                (action) =>
                {
                    try
                    {
                        action?.Invoke();
                    }
                    catch (Exception e)
                    {
                        UnityEngine.Debug.LogError(e);
                    }
                }
            );
        }
        public void Destroy()
        {
            OnDestroy.ForEach
            (
                (action) =>
                {
                    try
                    {
                        action?.Invoke();
                    }
                    catch (Exception e)
                    {
                        UnityEngine.Debug.LogError(e);
                    }
                }
            );
        }
    }
}

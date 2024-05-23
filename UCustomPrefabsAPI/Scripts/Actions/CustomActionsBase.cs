using System;
namespace UCustomPrefabsAPI
{
    public abstract class CustomActionsBase : HandlerReferencer, IComparable<CustomActionsBase>
    {
        private ActionStateMachine StateMachine { get { return Handler?.StateMachine; } }
        /// <summary>
        /// Order of which Actions are registered to StateMachine.
        /// </summary>
        public int Priority { get; set; }
        /// <summary>
        /// Adds Action to State OnEnter
        /// </summary>
        public void AddOnEnter(string name, Action<string> action, bool addState = false)
        {
            StateMachine.AddOnEnter(name, action, addState);
        }
        /// <summary>
        /// Adds Action to State OnUpdate
        /// </summary>
        public void AddOnUpdate(string name, Action action, bool addState = false)
        {
            StateMachine.AddOnUpdate(name, action, addState);
        }
        /// <summary>
        /// Adds Action to State OnExit
        /// </summary>
        public void AddOnExit(string name, Action<string> action, bool addState = false)
        {
            StateMachine.AddOnExit(name, action, addState);
        }
        /// <summary>
        /// Adds Action to StateMachine OnStateChanged
        /// </summary>
        public void AddOnStateChanged(Action<string, string> action)
        {
            StateMachine.AddOnStateChanged(action);
        }
        /// <summary>
        /// Adds Action to StateMachine OnUpdate
        /// </summary>
        public void AddOnUpdate(Action action)
        {
            StateMachine.AddOnUpdate(action);
        }
        /// <summary>
        /// Adds Action to StateMachine OnDestroy
        /// </summary>
        public void AddOnDestroy(Action action)
        {
            StateMachine.AddOnDestroy(action);
        }
        /// <summary>
        /// Called when Actions are Registered to StateMachine
        /// </summary>
        public abstract void RegisterActions();
        /// <summary>
        /// Handles the data from the CustomActionsTemplate.
        /// </summary>
        public virtual void HandleTemplateData(object[] data) { }
        public int CompareTo(CustomActionsBase other)
        {
            return Priority.CompareTo(other.Priority);
        }
    }
}

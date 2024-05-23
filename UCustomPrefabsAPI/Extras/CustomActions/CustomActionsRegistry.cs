using System;
using System.Collections.Generic;
using UnityEngine;
namespace UCustomPrefabsAPI.Extras.CustomActions
{
    //TODO Possibly register customActions via Abstraction + Attributes ?
    public static partial class CustomActionsRegistry
    {
        private static Dictionary<string, Type> RegisteredCustomActions = new Dictionary<string, Type>();
        /// <summary>
        /// Tries to get CustomActions Type via name.
        /// </summary>
        public static bool TryGetActions(string uid, out Type actionsType)
        {
            return RegisteredCustomActions.TryGetValue(uid, out actionsType);
        }
        /// <summary>
        /// Registers a CustomActions Type to be accessible via name. For use with LooseReferenceAction.
        /// </summary>
        public static void Register<T>(string uid) where T : CustomActionsBase
        {
            if (RegisteredCustomActions.ContainsKey(uid))
            {
                Debug.LogWarning($"CustomActions uid : \"{uid}\" already registered.");
                return;
            }
            RegisteredCustomActions.Add(uid, typeof(T));
        }
    }
}

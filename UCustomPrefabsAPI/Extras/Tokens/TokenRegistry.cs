using System;
using System.Collections.Generic;
using UnityEngine;
//WIP Temperary Class To Handle Data Syncing.//
namespace UCustomPrefabsAPI.Extras.Tokens
{
    public static class TokenRegistry
    {
        private static Dictionary<string, string> _tokens = new Dictionary<string, string>();
        private static Dictionary<string, HashSet<Action<string>>> _listeners = new Dictionary<string, HashSet<Action<string>>>();
        public static void SetToken(string token, string data)
        {
            if (!_tokens.ContainsKey(token))
                _tokens.Add(token, data);
            else
                _tokens[token] = data;
        }
        public static string GetToken(string token)
        {
            if (!_tokens.TryGetValue(token, out var data))
                data = string.Empty;
            return data;
        }
        public static void InvokeListeners(string token)
        {
            if (!_listeners.TryGetValue(token, out var actions))
                return;
            foreach (var action in actions)
            {
                try
                {
                    action?.Invoke(token);
                }
                catch (Exception ex)
                {
                    Debug.LogError(ex);
                }
            }
        }
        public static void Listen(string token, Action<string> action)
        {
            if (!_listeners.TryGetValue(token, out var actions))
            {
                actions = new HashSet<Action<string>>();
                _listeners.Add(token, actions);
            }
            actions.Add(action);
        }
        public static void StopListening(string token, Action<string> action)
        {
            if (!_listeners.TryGetValue(token, out var actions))
                return;
            actions.Remove(action);
        }
        public static void RemoveListeners(string token)
        {
            if (!_listeners.TryGetValue(token, out var actions))
                return;
            actions.Clear();
        }
    }
}

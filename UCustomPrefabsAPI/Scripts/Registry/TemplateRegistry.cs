using System;
using System.Collections.Generic;
using UCustomPrefabsAPI.Extras.CustomActions;
using UnityEngine;
namespace UCustomPrefabsAPI
{
    public static partial class TemplateRegistry
    {
        //TODO allow register "Paired" templates, that always get created with another template.
        private static Dictionary<string, TemplateData> TemplateDatas = new Dictionary<string, TemplateData>();
        private static readonly List<string> ReservedNames = new List<string> { "none" };
        /// <summary>
        /// Template Container to hold template assets.
        /// </summary>
        public static string Default { get; private set; } = string.Empty;
        private static GameObject _templateContainer;
        /// <summary>
        /// Template Container to hold template assets. 
        /// If this doesn't exist it will automatically create a new container.
        /// </summary>
        public static GameObject TemplateContainer
        {
            get
            {
                if (_templateContainer == null)
                {
                    _templateContainer = new GameObject("UCustomPrefabsAPI_Templates");
                    GameObject.DontDestroyOnLoad(_templateContainer);
                    _templateContainer.hideFlags = HideFlags.HideAndDontSave;
                    _templateContainer.SetActive(false);
                }
                return _templateContainer;
            }
        }
        /// <summary>
        /// Sets Default Template.
        /// </summary>
        public static void SetDefault(string uid)
        {
            if (TemplateDatas.ContainsKey(uid))
                Default = uid;
            else
                Debug.LogWarning($"Tempate uid : \"{uid}\" is not registered.");
        }
        /// <summary>
        /// Tries to get Template.
        /// </summary>
        public static bool TryGetTemplate(string uid, out TemplateData template)
        {
            var result = TemplateDatas.TryGetValue(uid, out template);
            if (!result)
            {
                result = TemplateDatas.TryGetValue(Default, out template);
                Debug.LogWarning($"Default Template will be used instead of Template uid : \"{uid}\"");
            }
            return result;
        }
        /// <summary>
        /// Registers a Template from a prefab.
        /// </summary>
        public static void Register(string uid, GameObject prefab)
        {
            if (ReservedNames.Contains(uid.ToLower()))
            {
                Debug.LogWarning($"Template uid : \"{uid}\" is reserved.");
                return;
            }
            if (TemplateDatas.ContainsKey(uid))
            {
                Debug.LogWarning($"Template uid : \"{uid}\" already registered.");
                return;
            }
            if (!prefab)
            {
                Debug.LogWarning($"Template uid : \"{uid}\" prefab is invalid.");
                return;
            }
            TemplateData template = new TemplateData(uid);
            template.InstantiatePrefab(prefab);
            TemplateDatas.Add(uid, template);
        }
        /// <summary>
        /// Registers a Empty Template.
        /// </summary>
        public static void RegisterEmpty(string uid)
        {
            if (TemplateDatas.ContainsKey(uid))
            {
                Debug.LogWarning($"Template uid : \"{uid}\" already registered.");
                return;
            }
            TemplateData template = new TemplateData(uid);
            template.InstantiateContainer();
            TemplateDatas.Add(uid, template);
        }
        public static List<string> GetTemplatesWithCustomActions<T>() where T : CustomActionsBase
        {
            return GetTemplatesWithCustomActions(typeof(T));
        }
        public static List<string> GetTemplatesWithCustomActions(string customAction_uid)
        {
            if (!CustomActionsRegistry.TryGetActions(customAction_uid, out var type))
                return new List<string>();
            return GetTemplatesWithCustomActions(type);
        }
        private static List<string> GetTemplatesWithCustomActions(Type customAction_type)
        {
            var templates = new List<string>();
            foreach (var pair in TemplateDatas)
            {
                foreach (var customActions in pair.Value.CustomActionsTemplates)
                {
                    var type = customActions.RegisterCustomActionsBaseType();
                    if (type == null)
                        continue;
                    if (type == customAction_type)
                        templates.Add(pair.Key);
                }
            }
            return templates;
        }
    }
}

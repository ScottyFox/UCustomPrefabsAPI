using System;
using System.Collections.Generic;
using System.Linq;
using UCustomPrefabsAPI.Extras.Utility;
using UnityEngine;
//WIP//
namespace UCustomPrefabsAPI
{
    /*Info & Initialization*/
    public partial class UCustomPrefabHandler : MonoBehaviour
    {
        private InstanceReferencer _instanceReferencer = new InstanceReferencer();
        /// <summary>
        /// Attached Instance to this Handler.
        /// </summary>
        public InstanceInfo Instance
        {
            get => _instanceReferencer.Instance;
            set => _instanceReferencer.Instance = value;
        }
        /// <summary>
        /// Initializes the StateMachine and Templates.
        /// </summary>
        public void Initialize()
        {
            AddState("init");
            AddState("default");
            RegisterTemplate();
            //Begin Intialization
            InitializeAndRegisterCustomActions();
            SetState("default");
        }
    }
    /*Actions & StateMachine*/
    public partial class UCustomPrefabHandler : MonoBehaviour
    {
        public ActionStateMachine StateMachine { get; private set; } = new();
        private Dictionary<Type, CustomActionsBase> RegisteredCustomActions = new();
        /// <summary>
        /// Sets the current state of the StateMachine and Templates.
        /// </summary>
        public void SetState(string name, bool reset = true)
        {
            var lastState = StateMachine.State;
            StateMachine.Do_OnExit();
            ResetTemplates();
            InstantiateStateTemplates(name);
            StateMachine.SetState(name);
            StateMachine.Do_OnEnter();
            StateMachine.Do_OnStateChanged(lastState);
        }
        /// <summary>
        /// Adds state to StateMachine and Templates.
        /// </summary>
        public void AddState(string name)
        {
            if (StateMachine.HasState(name))
                return;
            StateMachine.AddState(name);
            Templates.Add(name, new Dictionary<string, PrefabTemplate>());
        }
        //TODO Add way to get customAction via name.
        /// <summary>
        /// Tries to get CustomActions.
        /// </summary>
        public bool TryGetCustomActions<T>(out T customActions) where T : CustomActionsBase
        {
            customActions = null;
            if (RegisteredCustomActions.TryGetValue(typeof(T), out var actions))
                customActions = (T)actions;
            return customActions != null;
        }
        /// <summary>
        /// Registers CustomActions
        /// </summary>
        public T RegisterCustomActions<T>(object[] data = null, int priority = -1) where T : CustomActionsBase
        {
            return (T)RegisterCustomActions(typeof(T), data, priority);
        }
        /// <summary>
        /// Registers CustomActions via Type
        /// </summary>
        public CustomActionsBase RegisterCustomActions(Type type, object[] data = null, int priority = -1)
        {
            if (RegisteredCustomActions.TryGetValue(type, out var customActions))
                return customActions;
            customActions = (CustomActionsBase)Activator.CreateInstance(type);
            if (customActions == null)
                return null;
            customActions.Priority = priority;
            customActions.HandleTemplateData(data);
            RegisteredCustomActions.Add(type, customActions);
            return customActions;
        }
        /// <summary>
        /// Internal : Initializes and Registers CustomActions.
        /// </summary>
        public void InitializeAndRegisterCustomActions()
        {
            foreach (var pair in RegisteredCustomActions.OrderBy(x => x.Value.Priority))
            {
                var action = pair.Value;
                action.Handler = this;
                action.RegisterActions();
            }
        }
    }
    /*Templates*/
    public partial class UCustomPrefabHandler : MonoBehaviour
    {
        private Dictionary<string, Dictionary<string, PrefabTemplate>> Templates = new();
        /// <summary>
        /// Internal : Registers Templates from Instance Information.
        /// </summary>
        public void RegisterTemplate()
        {
            if (Instance == null || !TemplateRegistry.TryGetTemplate(Instance.TemplateUID, out var templateData))
                return;
            for (var i = 0; i < templateData.Templates.Count; i++)
            {
                var template = templateData.Templates[i];
                if (!Templates.ContainsKey(template.State))
                    AddState(template.State);
                Templates[template.State].Add($"{templateData.UID}:{i}", template);
            }
            var customActions = templateData.GetCustomActions();
            foreach (var pair in customActions)
            {
                //Register CustomActions with Data
                RegisterCustomActions(pair.Key, (object[])pair.Value[1], (int)pair.Value[0]);
            }
        }
    }
    /*Prefabs*/
    public partial class UCustomPrefabHandler : MonoBehaviour
    {
        /// <summary>
        /// Dictionay of loaded Templates, uid:template
        /// </summary>
        public Dictionary<string, PrefabTemplate> LoadedTemplates { get; private set; } = new();
        /// <summary>
        /// Internal : Resets Loaded Templates.
        /// </summary>
        /// <param name="includePersistent">
        /// removes persistent.
        /// </param>
        /// <param name="includePersistent">
        /// internal removes immediately.
        /// </param>
        public void ResetTemplates(bool includePersistent = false)
        {
            var removedTemplates = new List<string>();
            foreach (var template in LoadedTemplates)
                if (template.Value == null || (!template.Value.Persistent || includePersistent))
                {
                    removedTemplates.Add(template.Key);
                }
            foreach (var key in removedTemplates)
            {
                var template = LoadedTemplates[key];
                if (template != null)
                    Destroy(template.gameObject);
                LoadedTemplates.Remove(key);
            }
        }
        /// <summary>
        /// Internal : Instantiates Template
        /// </summary>
        public void InstantiateTemplate(string uid, PrefabTemplate template)
        {
            if (LoadedTemplates.ContainsKey(uid) || template == null)
                return;
            template = Instantiate(template.gameObject, transform).GetComponent<PrefabTemplate>();
            LoadedTemplates.Add(uid, template);
        }
        /// <summary>
        /// Internal : Instantiates state Templates
        /// </summary>
        public void InstantiateStateTemplates(string state)
        {
            if (!Templates.TryGetValue(state, out var templates))
                return;
            foreach (var template in templates)
                InstantiateTemplate(template.Key, template.Value);
        }
    }
    /*Tags*/
    public partial class UCustomPrefabHandler : MonoBehaviour
    {
        /// <summary>
        /// Get first template with tags.
        /// ex: <code>GetTemplateWithTag("tag1","tag2","tag3")</code>
        /// </summary>
        public PrefabTemplate GetTemplateWithTag(params string[] tags)
        {
            foreach (var template in LoadedTemplates)
                if (template.Value.HasTag(tags))
                    return template.Value;
            return null;
        }
        /// <summary>
        /// Get templates with tags.
        /// ex: <code>GetTemplatesWithTag("tag1","tag2","tag3")</code>
        /// </summary>
        public List<PrefabTemplate> GetTemplatesWithTag(params string[] tags)
        {
            var templates = new List<PrefabTemplate>();
            foreach (var template in LoadedTemplates)
                if (template.Value.HasTag(tags))
                    templates.Add(template.Value);
            return templates;
        }
        /// <summary>
        /// Get first tag within templates.
        /// ex: <code>GetTemplateWithTag("tag1","tag2","tag3")</code>
        /// </summary>
        public TaggedBehaviour GetTagInTemplates(params string[] tags)
        {
            foreach (var template in LoadedTemplates)
            {
                if (template.Value.HasTag(tags))
                    return template.Value;
                foreach (var child in template.Value.FindTagsInChildren(tags))
                {
                    if (child != null)
                        return child;
                }
            }
            return null;
        }
        /// <summary>
        /// Get tags within templates.
        /// ex: <code>GetTemplateWithTag("tag1","tag2","tag3")</code>
        /// </summary>
        public List<TaggedBehaviour> GetTagsInTemplates(params string[] tags)
        {
            var tagged = new List<TaggedBehaviour>();
            foreach (var template in LoadedTemplates)
            {
                if (template.Value.HasTag(tags))
                    tagged.Add(template.Value);
                tagged.AddRange(template.Value.FindTagsInChildren(tags));
            }
            return tagged;
        }
    }
    /*Unity MonoBehaviour Methods*/
    public partial class UCustomPrefabHandler : MonoBehaviour
    {
        private bool IsDestroyed = false;
        private void Update()
        {
            if (!IsDestroyed)
                StateMachine.Update();
        }
        private void FixedUpdate()
        {
            //TODO StateMachine.FixedUpdate();
        }
        private void LateUpdate()
        {
            //TODO StateMachine.LateUpdate();
        }
        private void OnDestroy()
        {
            HandleDestroy();
        }
        public void HandleDestroy(bool reset = true)
        {
            if (IsDestroyed)
                return;
            enabled = false;
            IsDestroyed = true;
            StateMachine.Destroy();
            ResetTemplates(true);
            if (Instance != null)
            {
                Instance.DestroyHandler(false, false);
                if (reset)
                    Instance.Reset();
                else
                    InstanceManager.Verify();
            }
            Destroy(this);
        }
    }
}

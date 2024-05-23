using System;
using UnityEngine;
namespace UCustomPrefabsAPI
{
    public class InstanceInfo
    {
        public string ID { get; private set; } = string.Empty;
        public Transform Target { get; private set; } = null;
        public string TemplateUID { get; private set; } = string.Empty;
        //TODO find more appropiate naming convention
        public UCustomPrefabHandler Handler { get; private set; } = null;
        public InstanceInfo(string id, string template_uid)
        {
            ID = id;
            TemplateUID = template_uid;
        }
        public bool SetTarget(Transform target)
        {
            if (target == null)
            {
                Debug.LogWarning("Target Doesn't Exist!");
                return false;
            }
            Target = target;
            Reset();
            return true;
        }
        public bool IsValid()
        {
            return Handler != null && Target != null;
        }
        public void Reset()
        {
            if (Target == null)
                return;
            if (!DestroyHandler())
            {
                try
                {
                    Handler = Target.gameObject.AddComponent<UCustomPrefabHandler>();
                    Handler.Instance = this;
                    Handler.Initialize();
                }
                catch (Exception e) { Debug.LogError(e); }
            }
        }
        public bool DestroyHandler(bool reset = true, bool destroy = true)
        {
            if (Handler == null)
                return false;
            var temp = Handler;
            Handler = null;
            if (destroy)
                temp.HandleDestroy(reset);
            return true;
        }
        public void PrepareRemove()
        {
            DestroyHandler(false);
        }
    }
}

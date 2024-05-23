using System.Collections.Generic;
using UnityEngine;
namespace UCustomPrefabsAPI
{
    public static class InstanceManager
    {
        private static Dictionary<string, InstanceInfo> Instances = new Dictionary<string, InstanceInfo>();
        /// <summary>
        /// Tries to get Instance
        /// </summary>
        public static bool TryGetInstance(string uid, out InstanceInfo instance)
        {
            instance = null;
            if (string.IsNullOrWhiteSpace(uid))
                return false;
            Verify();
            return Instances.TryGetValue(uid, out instance) && instance != null;
        }
        /// <summary>
        /// Gets Instances in target
        /// </summary>
        public static InstanceInfo[] GetInstancesInTarget(Transform target)
        {
            if (target == null) return new InstanceInfo[0];

            var handlers = target.gameObject.GetComponents<UCustomPrefabHandler>();
            var instances = new InstanceInfo[handlers.Length];
            for (int i = 0; i < handlers.Length; i++)
                instances[i] = handlers[i]?.Instance;

            return instances;
        }
        /// <summary>
        /// Gets Instances in target's children.
        /// </summary>
        public static InstanceInfo[] GetInstancesInTargetChildren(Transform target)
        {
            if (target == null) return new InstanceInfo[0];

            var handlers = target.gameObject.GetComponentsInChildren<UCustomPrefabHandler>();
            var instances = new InstanceInfo[handlers.Length];
            for (int i = 0; i < handlers.Length; i++)
                instances[i] = handlers[i]?.Instance;

            return instances;
        }
        /// <summary>
        /// Gets Instances in target's parent.
        /// </summary>
        public static InstanceInfo[] GetInstancesInTargetParent(Transform target)
        {
            if (target == null) return new InstanceInfo[0];

            var handlers = target.gameObject.GetComponentsInParent<UCustomPrefabHandler>();
            var instances = new InstanceInfo[handlers.Length];
            for (int i = 0; i < handlers.Length; i++)
                instances[i] = handlers[i]?.Instance;

            return instances;
        }
        /// <summary>
        /// Registers a new Instance
        /// </summary>
        public static bool Register(string uid, string template_uid, Transform target)
        {
            if (string.IsNullOrWhiteSpace(uid) || string.IsNullOrWhiteSpace(template_uid) || target == null)
                return false;

            Verify();

            if (Instances.ContainsKey(uid))
                return false;

            var newInstance = new InstanceInfo(uid, template_uid);
            if (!newInstance.SetTarget(target))
                return false;

            Instances.Add(uid, newInstance);
            return true;
        }
        /// <summary>
        /// Verifies Instances
        /// </summary>
        public static void Verify()
        {
            var invalidUids = new List<string>();
            foreach (var pair in Instances)
                if (pair.Value == null || !pair.Value.IsValid())
                    invalidUids.Add(pair.Key);
            foreach (var uid in invalidUids)
                Remove(uid);
        }
        /// <summary>
        /// Removes Instances From Target
        /// </summary>
        public static void RemoveInstancesFromTarget(Transform target)
        {
            foreach (var instance in GetInstancesInTarget(target))
            Remove(instance?.ID);
        }
        /// <summary>
        /// Removes Instances From Target Children
        /// </summary>
        public static void RemoveInstancesFromTargetChildren(Transform target)
        {
            foreach (var instance in GetInstancesInTargetChildren(target))
            Remove(instance?.ID);
        }
        /// <summary>
        /// Removes Instances From Target Parent
        /// </summary>
        public static void RemoveInstancesFromTargetParent(Transform target)
        {
            foreach (var instance in GetInstancesInTargetParent(target))
            Remove(instance?.ID);
        }
        /// <summary>
        /// Removes Instance
        /// </summary>
        public static void Remove(string uid)
        {
            if (string.IsNullOrWhiteSpace(uid))
                return;
            if (Instances.TryGetValue(uid, out var instance))
            {
                instance?.PrepareRemove();
                Instances.Remove(uid);
            }
        }
        /// <summary>
        /// Resets Instance
        /// </summary>
        public static void ResetInstance(string uid)
        {
            if (string.IsNullOrWhiteSpace(uid))
                return;
            if (Instances.TryGetValue(uid, out var instance))
            instance?.Reset();
        }
        /// <summary>
        /// Clears all Instances.
        /// </summary>
        public static void Reset()
        {
            foreach (var uid in new List<string>(Instances.Keys))
            Remove(uid);
            Instances.Clear();
        }
    }
}

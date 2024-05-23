using System;
//Utility//
namespace UCustomPrefabsAPI
{
    public class InstanceReferencer
    {
        private WeakReference<InstanceInfo> _Instance_Reference = new WeakReference<InstanceInfo>(null);
        public InstanceInfo Instance
        {
            get
            {
                _Instance_Reference.TryGetTarget(out var target);
                return target;
            }
            set
            {
                _Instance_Reference.SetTarget(value);
            }
        }
    }
}

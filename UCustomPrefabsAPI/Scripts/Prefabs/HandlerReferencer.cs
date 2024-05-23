using System;
//Utility//
namespace UCustomPrefabsAPI
{
    public class HandlerReferencer
    {
        private WeakReference<UCustomPrefabHandler> _Handler_Reference = new WeakReference<UCustomPrefabHandler>(null);
        public UCustomPrefabHandler Handler
        {
            get
            {
                _Handler_Reference.TryGetTarget(out var target);
                return target;
            }
            set
            {
                _Handler_Reference.SetTarget(value);
            }
        }
    }
}

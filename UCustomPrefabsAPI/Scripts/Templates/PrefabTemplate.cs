using UCustomPrefabsAPI.Extras.Utility;
using UnityEngine;
namespace UCustomPrefabsAPI
{
    public class PrefabTemplate : TaggedBehaviour
    {
        [Tooltip("Determines if this template get's removed on state change.")]
        public bool Persistent = false;
        [Tooltip("State this template is instantiated.")]
        public string State = "default";
    }
}
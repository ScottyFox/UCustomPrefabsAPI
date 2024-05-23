using System;
using UnityEngine;
namespace UCustomPrefabsAPI.Extras.CustomActions
{
    public class LooseReferenceAction : CustomActionsTemplate
    {
        [SerializeField]
        public string CustomActionName;
        [SerializeField]
        public string Data;
        public override Type RegisterCustomActionsBaseType()
        {
            Type type;
            CustomActionsRegistry.TryGetActions(CustomActionName, out type);
            return type;
        }
        public override object[] PrepareTemplateData()
        {
            return new object[] { Data };
        }
    }
}

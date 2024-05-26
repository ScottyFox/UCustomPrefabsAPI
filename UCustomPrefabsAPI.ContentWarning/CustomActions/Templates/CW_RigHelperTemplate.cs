using System;
namespace UCustomPrefabsAPI.ContentWarning.CustomActions
{
    public class CW_RigHelperTemplate : CustomActionsTemplate
    {
        public override Type RegisterCustomActionsBaseType() => typeof(CW_RigHelper);
    }
}
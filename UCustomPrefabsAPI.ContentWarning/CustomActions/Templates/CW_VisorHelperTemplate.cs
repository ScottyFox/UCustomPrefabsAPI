using System;
namespace UCustomPrefabsAPI.ContentWarning.CustomActions
{
    public class CW_VisorHelperTemplate : CustomActionsTemplate
    {
        public override Type RegisterCustomActionsBaseType() => typeof(CW_VisorHelper);
    }
}
using System;
namespace UCustomPrefabsAPI.ContentWarning.CustomActions
{
    public class CW_VisorTextHelperTemplate : CustomActionsTemplate
    {
        public override Type RegisterCustomActionsBaseType() => typeof(CW_VisorTextHelper);
    }
}
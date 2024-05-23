using System;
using UnityEngine;
namespace UCustomPrefabsAPI
{
    public abstract class CustomActionsTemplate : MonoBehaviour
    {
        public int Priority = 0;
        public abstract Type RegisterCustomActionsBaseType();
        public virtual object[] PrepareTemplateData() { return null; }
    }
}

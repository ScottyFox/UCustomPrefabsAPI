using HarmonyLib;
using UnityEngine;

namespace UCustomPrefabsAPI.ContentWarning.Patches
{
    [HarmonyPatch]
    internal class MirrorPatch
    {
        [HarmonyPatch(typeof(PlayerCustomizer), "Awake")]
        [HarmonyPostfix]
        static void Awake_Postfix_Patch(ref PlayerCustomizer __instance)
        {
            var mirror = GameObject.Find("Mirror");
            if (mirror)
            {
                if (Plugin.UseMirrorSelector.Value) ;
                InstanceManager.Register("CW_MirrorSelector", "CW_MirrorSelector", mirror.transform);
            }
            else
                Debug.Log("Couldnt Find Mirror!");
        }
    }
}

using HarmonyLib;
using UCustomPrefabsAPI.ContentWarning.Networking;
namespace UCustomPrefabsAPI.ContentWarning
{
    [HarmonyPatch]
    internal static class PlayerPatch
    {
        [HarmonyPatch(typeof(Player), "DoInits")]
        [HarmonyPostfix]
        static void DoInits_Postfix_Patch(ref Player __instance)
        {
            if (CW_Utilities.IsPlayer(__instance))
            {
                __instance.gameObject.AddComponent<PlayerConfigHelper>();
            }
            
        }
    }
}

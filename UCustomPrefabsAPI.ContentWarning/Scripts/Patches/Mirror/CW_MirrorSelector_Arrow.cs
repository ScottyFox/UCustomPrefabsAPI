using Photon.Pun;
using UCustomPrefabsAPI.ContentWarning.CustomActions;
using UnityEngine;

namespace UCustomPrefabsAPI.ContentWarning.Patches.Mirror
{
    public class CW_MirrorSelector_Arrow : Interactable
    {
        public CW_MirrorSelector CustomAction;
        public override void Interact(Player player)
        {
            if (CustomAction == null)
                return;
            CustomAction.ArrowInteracted(this, player);
        }
    }
}

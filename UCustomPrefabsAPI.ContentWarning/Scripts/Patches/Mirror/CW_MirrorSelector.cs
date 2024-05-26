using UnityEngine;
using TMPro;
using UCustomPrefabsAPI.ContentWarning.Patches.Mirror;
using System.Collections.Generic;
using UCustomPrefabsAPI.ContentWarning.CustomActions;
using UCustomPrefabsAPI.ContentWarning.Networking;
namespace UCustomPrefabsAPI.ContentWarning.Patches.Mirror
{
    public class CW_MirrorSelector : CustomActionsBase
    {
        private string CurrentOption = string.Empty;
        private List<string> Options = new List<string>();
        private int OptionIndex = 0;
        private TextMeshPro Display;
        private CW_MirrorSelector_Arrow LeftArrow;
        private CW_MirrorSelector_Arrow RightArrow;
        public override void RegisterActions()
        {
            AddOnStateChanged(AttachTargets);
        }
        public void AttachTargets(string last, string current)
        {
            var display = Handler.GetTagInTemplates("Display");
            var left = Handler.GetTagInTemplates("LeftArrow");
            var right = Handler.GetTagInTemplates("RightArrow");
            if (display == null || left == null || right == null)
            {
                Debug.LogWarning("Unable to setup Mirror Selector.");
                return;
            }
            Display = display.GetComponent<TextMeshPro>();
            LeftArrow = left.gameObject.AddComponent<CW_MirrorSelector_Arrow>();
            RightArrow = right.gameObject.AddComponent<CW_MirrorSelector_Arrow>();
            LeftArrow.gameObject.layer = LayerMask.NameToLayer("Interactable");
            RightArrow.gameObject.layer = LeftArrow.gameObject.layer;
            LeftArrow.hoverText = "Last";
            RightArrow.hoverText = "Next";
            LeftArrow.CustomAction = this;
            RightArrow.CustomAction = this;
            Options = TemplateRegistry.GetTemplatesWithCustomActions<CW_RigHelper>();
            Options.Add("None");
            SwitchToOption(PlayerConfigHelper.GetPlayerData());
            PlayerConfigHelper.SetPlayerData(CurrentOption);
        }
        public void ArrowInteracted(CW_MirrorSelector_Arrow Arrow, Player Player)
        {
            if (Arrow == LeftArrow)
            {
                OptionIndex--;
            }
            else if (Arrow == RightArrow)
            {
                OptionIndex++;
            }
            if (OptionIndex < 0)
                OptionIndex = Options.Count - 1;
            else
            if (OptionIndex >= Options.Count)
                OptionIndex = 0;
            UpdateOption(OptionIndex);
            var configHelper = Player.GetComponent<PlayerConfigHelper>();
            if (configHelper != null)
            {
                PlayerConfigHelper.SetPlayerData(CurrentOption);
                configHelper.UpdatePlayerData();
            }
        }
        public void UpdateOption(int Index)
        {
            if (Index < 0 || Index >= Options.Count || Display == null)
                return;
            CurrentOption = Options[Index];
            Display.text = CurrentOption;
        }
        public void SwitchToOption(string Option)
        {
            var index = Options.IndexOf(Option);
            if (index == -1)
                UpdateOption(Options.Count - 1);
            else
                UpdateOption(index);
        }
    }
}
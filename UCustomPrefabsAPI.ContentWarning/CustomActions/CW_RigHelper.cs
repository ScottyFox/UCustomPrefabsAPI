using System.Collections.Generic;
using UCustomPrefabsAPI.Extras.Animation;
using UnityEngine;
using UnityEngine.Rendering;
namespace UCustomPrefabsAPI.ContentWarning.CustomActions
{
    public class CW_RigHelper : CustomActionsBase
    {
        private Player player = null;
        private Dictionary<RigBuilder, BoneRigTracker> Rig_Targets = new Dictionary<RigBuilder, BoneRigTracker>();
        private static LayerMask Layer_LocalDontSee = LayerMask.NameToLayer("LocalDontSee");
        public override void RegisterActions()
        {
            AddOnStateChanged(AttachTargets);
            AddOnUpdate(Update);
            AddOnDestroy(Reset);
        }
        public void AttachTargets(string last, string state)
        {
            player = Handler.GetComponent<Player>();
            if (!player)
                return;
            HideVanilla(false);
            var hips = player.transform.Find("RigCreator/Rig/Armature");
            if (!hips)
                return;
            foreach (var template in Handler.LoadedTemplates)
            {
                SetUpRig(template.Value.GetComponent<RigBuilder>(), hips);
                var rigs = template.Value.GetComponentsInChildren<RigBuilder>();
                foreach (var rig in rigs)
                    SetUpRig(rig, hips);
            }
            HideVanilla(true);
            FixVisuals();
            Update();
        }
        public void Reset()
        {
            HideVanilla(false);
        }
        public void SetUpRig(RigBuilder rig, Transform hips)
        {
            if (!rig) return;
            if (Rig_Targets.TryGetValue(rig, out var tracker))
                return;
            tracker = new BoneRigTracker(rig.transform, hips);
            tracker.SetUpRig(rig.Rig);
            Rig_Targets.Add(rig, tracker);
        }
        public void HideVanilla(bool hide)
        {
            if (!Handler)
                return;
            var visuals = Handler.transform.Find("CharacterModel")?.GetComponentsInChildren<Renderer>();
            if (visuals != null)
                foreach (var renderer in visuals) { 
                    renderer.forceRenderingOff = hide;
                }
            var face = Handler.transform.Find("HeadPosition/FACE")?.GetComponent<Renderer>();
            if (face)
                face.forceRenderingOff = hide;

        }
        public void FixVisuals()
        {
            if (player.photonView.IsMine)
            {
                foreach (var template in Handler.GetTemplatesWithTag("LocalDontSee"))
                    foreach (var renderer in template.GetComponentsInChildren<Renderer>())
                        renderer.gameObject.layer = Layer_LocalDontSee;
                foreach (var target in Handler.GetTagsInTemplates("LocalShinkBone"))
                    target.transform.localScale = Vector3.zero;
            }
            else
            {
                foreach (var template in Handler.GetTemplatesWithTag("FirstPersonOnly"))
                    foreach (var renderer in template.GetComponentsInChildren<Renderer>())
                        renderer.forceRenderingOff = true;
            }
        }
        public void Update()
        {
            bool valid;
            do
            {
                valid = true;
                foreach (var pair in Rig_Targets)
                {
                    if (!pair.Key || pair.Value == null)
                    {
                        valid = false;
                        break;
                    }
                    pair.Value.Update();
                }
                if (!valid)
                    Verify();
            } while (!valid);
        }
        public void Verify()
        {
            Debug.LogWarning("Tracker seems to be invalid, Verifying.");
            var newTrackers = new Dictionary<RigBuilder, BoneRigTracker>();
            foreach (var pair in Rig_Targets)
                if (pair.Key && pair.Value != null)
                    newTrackers.Add(pair.Key, pair.Value);
                else
                    Debug.LogWarning("Removing Tracker from RigHelper!");
            Rig_Targets = newTrackers;
        }
    }
}

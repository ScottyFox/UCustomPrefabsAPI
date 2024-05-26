using System.Collections.Generic;
using UnityEngine;
using UCustomPrefabsAPI.Extras.Utility;
namespace UCustomPrefabsAPI.ContentWarning.CustomActions
{
    public class CW_VisorHelper : CustomActionsBase
    {
        private Player Player = null;
        private Dictionary<Renderer, List<int>> Renderer_Targets = new Dictionary<Renderer, List<int>>();
        private MaterialPropertyBlock propertyBlock = new MaterialPropertyBlock();
        public override void RegisterActions()
        {
            AddOnStateChanged(AttachTargets);
            AddOnUpdate(Update);
        }
        //TODO Condense into functions
        public void AttachTargets(string last, string current)
        {
            Player = Handler.GetComponent<Player>();
            if (!Player)
                return;
            var VisorMaterial = GetCurrentVisorMaterial();
            foreach (var target in Handler.GetTagsInTemplates("UseVanillaVisor"))
            {
                var taggedData = target.GetComponent<TaggedData>();
                var renderer = target.GetComponent<Renderer>();
                if (!taggedData || !renderer)
                    continue;
                if (!taggedData.TryGetTagData("UseVanillaVisor", out var data))
                    continue;
                var materials = renderer.sharedMaterials;
                foreach (var token in data.Split(','))
                {
                    if (!int.TryParse(token, out var index) || index < 0 || index >= materials.Length)
                        continue;
                    materials[index] = new Material(VisorMaterial);
                    if (!Renderer_Targets.TryGetValue(renderer, out var indexes))
                    {
                        indexes = new List<int>();
                        Renderer_Targets.Add(renderer, indexes);
                    }
                    indexes.Add(index);
                }
                renderer.sharedMaterials = materials;
            }
            foreach (var target in Handler.GetTagsInTemplates("UseCustomVisor"))
            {
                var taggedData = target.GetComponent<TaggedData>();
                var renderer = target.GetComponent<Renderer>();
                if (!taggedData || !renderer)
                    continue;
                if (!taggedData.TryGetTagData("UseCustomVisor", out var data))
                    continue;
                var materials = renderer.sharedMaterials;
                foreach (var token in data.Split(','))
                {
                    if (!int.TryParse(token, out var index) || index < 0 || index >= materials.Length)
                        continue;
                    if (!Renderer_Targets.TryGetValue(renderer, out var indexes))
                    {
                        indexes = new List<int>();
                        Renderer_Targets.Add(renderer, indexes);
                    }
                    indexes.Add(index);
                }
            }
            Update();
        }
        public void Verify()
        {
            Debug.LogWarning("Visor Renderer seems to be invalid, Verifying.");
            var Refreshed = new Dictionary<Renderer, List<int>>();
            foreach (var target in Renderer_Targets)
            {
                if (target.Key)
                    Refreshed.Add(target.Key, target.Value);
                else
                    Debug.LogWarning("Removing Renderer from VisorHelper!");
            }
            Renderer_Targets = Refreshed;
        }
        public void Update()
        {
            if (!Player)
                return;
            var material = GetCurrentVisorMaterial();
            propertyBlock.SetColor("_Color", material.GetColor("_Color"));
            propertyBlock.SetFloat("_VoiceEmis", material.GetFloat("_VoiceEmis"));
            propertyBlock.SetFloat("_Voice", material.GetFloat("_Voice"));
            propertyBlock.SetFloat("_M1", material.GetFloat("_M1"));
            propertyBlock.SetFloat("_M2", material.GetFloat("_M2"));
            propertyBlock.SetFloat("_TextureStrength", material.GetFloat("_TextureStrength"));
            propertyBlock.SetFloat("_TextureStrength2", material.GetFloat("_TextureStrength2"));
            bool valid;
            do
            {
                valid = true;
                foreach (var target in Renderer_Targets)
                {
                    if (!target.Key)
                    {
                        valid = false;
                        break;
                    }
                    foreach (var index in target.Value)
                    {
                        target.Key.SetPropertyBlock(propertyBlock, index);
                    }
                }
                if (!valid)
                    Verify();
            } while (!valid);
        }
        public void UpdateVisorPropertyBlock()
        {
            Player.refs.visor.visorRenderer.GetPropertyBlock(propertyBlock, Player.refs.visor.visorMaterialIndex);
        }
        public Color GetCurrentVisorColor()
        {
            return GetCurrentVisorMaterial().GetColor("_Color");
        }
        public Material GetCurrentVisorMaterial()
        {
            return Player.refs.visor.visorRenderer.sharedMaterials[Player.refs.visor.visorMaterialIndex];
        }
    }
}
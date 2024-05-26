using System;
using TMPro;
using UnityEngine;
namespace UCustomPrefabsAPI.ContentWarning.CustomActions
{
    public class CW_ShaderFix : CustomActionsBase
    {
        public override void RegisterActions()
        {
            AddOnStateChanged(DoFix);
        }
        public void DoFix(string last, string state)
        {
            foreach (var target in Handler.LoadedTemplates)
                FixShaders(target.Value.transform);
        }
        public void FixShaders(Transform target)
        {
            foreach (var renderer in target.GetComponentsInChildren<Renderer>())
            {
                var materials = renderer.sharedMaterials;
                for (int i = 0; i < materials.Length; i++)
                {
                    var newShader = Shader.Find(materials[i].shader.name);
                    if (newShader != null)
                        materials[i].shader = newShader;
                }
                renderer.sharedMaterials = materials;
            }
        }
    }
}
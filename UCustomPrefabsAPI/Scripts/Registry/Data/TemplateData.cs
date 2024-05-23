using System;
using System.Collections.Generic;
using UnityEngine;
namespace UCustomPrefabsAPI
{
    public static partial class TemplateRegistry
    {
        public class TemplateData
        {
            public string UID;
            public GameObject Container = null;
            public List<CustomActionsTemplate> CustomActionsTemplates = new List<CustomActionsTemplate>();
            public List<PrefabTemplate> Templates = new List<PrefabTemplate>();
            public TemplateData(string uid)
            {
                UID = uid;
            }
            /// <summary>
            /// Instantiates a empty Container for external modification.
            /// </summary>
            public void InstantiateContainer()
            {
                Reset();
                var container = new GameObject($"{UID}");
                container.transform.parent = TemplateContainer.transform;
                Container = container;
                Container.hideFlags = HideFlags.HideAndDontSave;
                Container.SetActive(false);
            }
            /// <summary>
            /// Instantiates Container from a prefab then searches for Templates and CustomActionTemplates.
            /// </summary>
            public void InstantiatePrefab(GameObject prefab)
            {
                Reset();
                var template_prefab = GameObject.Instantiate(prefab, TemplateContainer.transform);
                if (!template_prefab)
                    return;
                Container = template_prefab;
                Container.hideFlags = HideFlags.HideAndDontSave;
                Container.SetActive(false);
                foreach (var customActions in Container.GetComponents<CustomActionsTemplate>())
                {
                    AddCustomActionsTemplate(customActions);
                }
                foreach (var template in Container.GetComponentsInChildren<PrefabTemplate>())
                {
                    AddTemplate(template);
                }
            }
            /// <summary>
            /// Creates a new instance of a CustomActionsTemplate.
            /// </summary>
            /// <returns>The CustomActionsTemplate to be modified externally.</returns>
            public T InstantiateCustomActionsTemplate<T>() where T : CustomActionsTemplate
            {
                if (!Container)
                {
                    Debug.LogWarning("Could not add CustomActionsTemplate. : Container Missing...!");
                    return null;
                }
                return Container.AddComponent<T>();
            }
            /// <summary>
            /// Allows instantiating templates directly from prefabs.
            /// </summary>
            public void InstantiateTemplate(GameObject prefab)
            {
                if (!Container)
                {
                    Debug.LogWarning("Could not instatiate Template. : Container Missing...!");
                    return;
                }
                var templateprefab = GameObject.Instantiate(prefab, Container.transform);
                if (!templateprefab)
                {
                    Debug.LogWarning("Could not instatiate Template. : Invalid Prefab...!");
                    return;
                }
                var template = templateprefab.GetComponent<PrefabTemplate>();
                if (!template)
                {
                    Debug.LogWarning("Template does not have PrefabTemplate component.");
                }
                else
                    AddTemplate(template);
            }
            /// <summary>
            /// Adds a CustomActionsTemplate to CustomActionsTemplates list
            /// </summary>
            public void AddCustomActionsTemplate(CustomActionsTemplate customActions)
            {
                CustomActionsTemplates.Add(customActions);
            }
            /// <summary>
            /// Adds a Template to Templates list
            /// </summary>
            /// <param name="reParent">Set template's parent to current Container.</param>
            public void AddTemplate(PrefabTemplate prefab, bool reParent = false)
            {
                if (!prefab)
                    return;
                if (reParent)
                    prefab.transform.parent = Container.transform;
                Templates.Add(prefab);
            }
            /// <summary>
            /// Iterates through the CustomActionsTemplates and returns their types and associated data.
            /// </summary>
            public Dictionary<Type, object[]> GetCustomActions()
            {
                var result = new Dictionary<Type, object[]>();
                foreach (var customActionsTemplate in CustomActionsTemplates)
                {
                    var type = customActionsTemplate.RegisterCustomActionsBaseType();
                    if (type != null && !result.ContainsKey(type))
                    {
                        //Gross Data Compacting...
                        result.Add(type, new object[] { customActionsTemplate.Priority, customActionsTemplate.PrepareTemplateData() });
                    }
                }
                return result;
            }
            /// <summary>
            /// Resets this TemplateData to be reinstantiated. Possibly redundant.
            /// </summary>
            public void Reset()
            {
                GameObject.DestroyImmediate(Container);
                CustomActionsTemplates.Clear();
                Templates.Clear();
            }
        }
    }
}

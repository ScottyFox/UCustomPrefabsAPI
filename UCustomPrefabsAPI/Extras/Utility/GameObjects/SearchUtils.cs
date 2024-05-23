using System;
using System.Collections.Generic;
using UCustomPrefabsAPI.Extras.AssetBundles;
using UnityEngine;
namespace UCustomPrefabsAPI.Extras.Utility
{
    public static class SearchUtils
    {
        /// <summary>
        /// Recursively Finds A Child
        /// </summary>
        public static Transform RecursivelyFindChild(Transform parent, string name)
        {
            foreach (Transform child in parent)
            {
                if (child.name == name)
                    return child;
                Transform found = RecursivelyFindChild(child, name);
                if (found)
                    return found;
            }
            return null;
        }
        /// <summary>
        /// Iteratively Finds A Child
        /// </summary>
        public static Transform IterativelyFindChild(Transform parent, string name)
        {
            var searchQueue = new Queue<Transform>();
            searchQueue.Enqueue(parent);
            while (searchQueue.Count != 0)
            {
                foreach (Transform child in parent)
                {
                    if (child.name == name)
                        return child;
                    searchQueue.Enqueue(child);
                }
            }
            return null;
        }
        /// <summary>
        /// Recursively Collects All Child Names
        /// </summary>
        public static void RecursivelyCollectChildNames(Transform target, ref Dictionary<string, Transform> looseNameRefs)
        {
            if (looseNameRefs == null)
                looseNameRefs = new Dictionary<string, Transform>();
            foreach (Transform child in target)
            {
                if (!looseNameRefs.ContainsKey(child.name))
                    looseNameRefs.Add(child.name, child);
                if (child.childCount > 0)
                    RecursivelyCollectChildNames(child, ref looseNameRefs);
            }
        }
        /// <summary>
        /// Iteratively Collects All Child Names
        /// </summary>
        public static void IterativelyCollectChildNames(Transform target, ref Dictionary<string, Transform> looseNameRefs)
        {
            if (looseNameRefs == null)
                looseNameRefs = new Dictionary<string, Transform>();
            var searchQueue = new Queue<Transform>();
            searchQueue.Enqueue(target);
            while (searchQueue.Count != 0)
            {
                foreach (Transform child in searchQueue.Dequeue())
                {
                    if (!looseNameRefs.ContainsKey(child.name))
                        looseNameRefs.Add(child.name, child);
                    searchQueue.Enqueue(child);
                }
            }
        }
        /// <summary>
        /// Replaces Portion Of Path With Provided String
        /// </summary>
        public static void FixPaths(ref string[] paths, string replace, string with)
        {
            for (int i = 0; i < paths.Length; i++)
            {
                paths[i] = paths[i].Replace(replace, with);
            }
        }
        /// <summary>
        /// Creates A Path From A Root And It's Child Target.
        /// </summary>
        public static string FindPath(Transform root, Transform target)
        {
            var path = target ? target.name : string.Empty;
            while (target != null && target.parent != root)
            {
                target = target.parent;
                path = $"{target.name}/{path}";
            }
            return path;
        }
        /// <summary>
        /// Creates a "Relative" Path from origin and target.
        /// </summary>
        public static string RelativeFindPath(Transform origin, Transform target)
        {
            var targetParents = MapParentHierarchy(target);
            var path = string.Empty;
            if (targetParents.Contains(origin))
            {
                path = FindPath(origin, target);
            }
            else
            {
                var rootParents = MapParentHierarchy(origin);
                foreach (var parent in rootParents)
                {
                    path += "../";
                    if (targetParents.Contains(parent))
                    {
                        path += FindPath(parent, target);
                        break;
                    }
                }
            }
            return path;
        }
        //add "*" to the start so that it searches all children until it finds that name.
        //add "**" to the start so that it searches all parents until it finds that name.
        //add "|" searches all children until it finds either first or second token afterwards
        //add "||" searches all children until it finds either first or second token afterwards
        //add "@" checks if a assetbundle is registered with the asset.
        //^^ This may require a GameObject reference instead.
        public static Transform RelativeFind(Transform origin, string relativePath)
        {
            var tokens = new Queue<string>(relativePath.Split('/'));
            var target = origin;
            try
            {
                while (target != null && tokens.Count > 0)
                {
                    var token = tokens.Dequeue();
                    Transform temp;
                    string tempA;
                    string tempB;
                    switch (token)
                    {
                        case "..":
                            target = target.parent;
                            break;
                        case "*":
                            target = IterativelyFindChild(origin, tokens.Dequeue());
                            break;
                        case "**":
                            target = FindParent(target, tokens.Dequeue());
                            break;
                        case "|":
                            tempA = tokens.Dequeue();
                            tempB = tokens.Dequeue();
                            temp = IterativelyFindChild(target, tempA);
                            if (!temp)
                                temp = IterativelyFindChild(target, tempB);
                            target = temp;
                            break;
                        case "||":
                            tempA = tokens.Dequeue();
                            tempB = tokens.Dequeue();
                            temp = FindParent(target, tempA);
                            if (!temp)
                                temp = FindParent(target, tempB);
                            target = temp;
                            break;
                        case "@":
                            tempA = tokens.Dequeue();
                            var prefab = AssetBundleRegistry.LoadPrefab(tempA, string.Join("/", tokens));
                            if (prefab)
                                target = prefab.transform;
                            break;
                        default:
                            target = target.Find(token);
                            break;
                    }
                }
            }
            catch (Exception) { Debug.LogError("Invalid Relative Path."); target = null; }
            return target;
        }
        /// <summary>
        /// Finds first parent with a name
        /// </summary>
        public static Transform FindParent(Transform origin, string name)
        {
            bool found = false;
            var target = origin;
            while (!found && target != null)
            {
                target = target.parent;
                if (target.name == name)
                    found = true;
            }
            return (found) ? target : null;
        }
        /// <summary>
        /// Maps out Parent Hierarchy
        /// </summary>
        public static HashSet<Transform> MapParentHierarchy(Transform root)
        {
            var hierarchy = new HashSet<Transform>();
            var pointer = root;
            while (root.parent != null)
            {
                hierarchy.Add(root.parent);
                root = root.parent;
            }
            return hierarchy;
        }
    }
}
using System;
using System.Collections.Generic;
using UnityEngine;
using UCustomPrefabsAPI.Extras.Utility;
namespace UCustomPrefabsAPI.Extras.Animation
{
    [Serializable]
    public class BoneMapPair
    {
        public string origin = string.Empty;
        public string target = string.Empty;
    }
    [Serializable]
    public class BoneMap
    {
        public bool UsePaths = false;
        private Dictionary<string, BoneMapPair> _dict = new Dictionary<string, BoneMapPair>();
        public List<BoneMapPair> bonePairs = new List<BoneMapPair>();
        public void AddDefaultHumanBoneNames()
        {
            ClearPairs();
            foreach (var humanBone in RigUtilities.HumanBodyBonesAry)
            {
                AddPair(humanBone.ToString(), string.Empty);
            }
        }
        public void AddHumanBones(Animator animator)
        {
            if (!animator)
            {
                Debug.LogWarning("Animator not found.");
                return;
            }
            ClearPairs();
            foreach (var bone in RigUtilities.HumanBodyBonesAry)
            {
                try
                {
                    var transform = animator.GetBoneTransform(bone);
                    if (!transform)
                        continue;
                    if (UsePaths)
                        AddPair(Enum.GetName(typeof(HumanBodyBones), bone), SearchUtils.FindPath(animator.transform, transform));
                    else
                        AddPair(Enum.GetName(typeof(HumanBodyBones), bone), transform.name);
                }
                catch (Exception) { }
            }
        }
        public void AddRecursiveBones(Transform target)
        {
            ClearPairs();
            Dictionary<string, Transform> dict = new Dictionary<string, Transform>();
            SearchUtils.RecursivelyCollectChildNames(target, ref dict);
            foreach (var pair in dict)
            {
                if (UsePaths)
                    AddPair(pair.Key, SearchUtils.FindPath(target, pair.Value));
                else
                    AddPair(pair.Key, pair.Key);
            }
        }
        public List<string> MatchBoneMaps(BoneMap target)
        {
            ValidateDict();
            var bones = new List<string>();
            if (target == null)
                Debug.Log("Cannot Match BoneMaps!!!");
            else
                foreach (var pair in bonePairs)
                    if (target.HasPair(pair.origin))
                        bones.Add(pair.origin);
            return bones;
        }
        public bool HasPair(string origin)
        {
            ValidateDict();
            if (!_dict.TryGetValue(origin, out var pair))
                return false;
            return !string.IsNullOrWhiteSpace(pair.target);
        }
        public string FetchPair(string origin)
        {
            ValidateDict();
            if (!_dict.TryGetValue(origin, out var pair))
                return string.Empty;
            return pair.target;
        }
        public void AddPair(string origin, string target)
        {
            ValidateDict();
            if (_dict.ContainsKey(origin))
                return;
            var pair = new BoneMapPair { origin = origin, target = target };
            bonePairs.Add(pair);
            _dict.Add(origin, pair);
        }
        public void RemovePair(string origin)
        {
            ValidateDict();
            if (!_dict.TryGetValue(origin, out var pair))
                return;
            bonePairs.Remove(pair);
            _dict.Remove(origin);
        }
        public void ClearEmptyPairs()
        {
            ValidateDict();
            var removeList = new HashSet<string>(_dict.Keys);
            foreach (var pair in bonePairs)
            {
                if (!string.IsNullOrEmpty(pair.target))
                    removeList.Remove(pair.origin);
            }
            foreach (var name in removeList)
            {
                _dict.Remove(name);
            }
        }
        public void ClearPairs()
        {
            _dict.Clear();
            bonePairs.Clear();
        }
        public void ValidateDict()
        {
            var removeList = new HashSet<string>(_dict.Keys);
            foreach (var pair in bonePairs)
            {
                if (_dict.ContainsKey(pair.origin))
                    removeList.Remove(pair.origin);
                else
                    _dict.Add(pair.origin, pair);
            }
            foreach (var name in removeList)
            {
                _dict.Remove(name);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using UnityEngine;
using UCustomPrefabsAPI.Extras.Utility;
namespace UCustomPrefabsAPI.Extras.Animation
{
    public static class RigUtilities
    {
        /// <summary>
        /// Array of HumanBodyBones for Reference
        /// </summary>
        public static HumanBodyBones[] HumanBodyBonesAry = (HumanBodyBones[])Enum.GetValues(typeof(HumanBodyBones));

        /// <summary>
        /// Calculates height ratio between origin and target y-position.
        /// </summary>
        public static bool TryBuildRig(RigBuilderTemplate Template, Transform target, BoneMap targetBoneMap, out BoneRigInfo rig)
        {
            rig = null;
            Transform temp_target = null;
            Transform temp_rig = null;
            try
            {
                temp_target = GameObject.Instantiate(target, null).transform;
                temp_rig = GameObject.Instantiate(Template.RigPrefab, null).transform;
                if (Template.PoseTargetRig)
                    Template.ReferencePose?.SampleAnimation(temp_target.gameObject, 0f);
                if (Template.PoseTemplateRig)
                    Template.ReferencePose?.SampleAnimation(temp_rig.gameObject, 0f);
                var originBoneNames = new Dictionary<string, Transform>();
                var targetBoneNames = new Dictionary<string, Transform>();
                SearchUtils.RecursivelyCollectChildNames(temp_target, ref originBoneNames);
                SearchUtils.RecursivelyCollectChildNames(temp_rig, ref targetBoneNames);
                var sharedBones = targetBoneMap.MatchBoneMaps(Template.BoneMap);
                var matchedBones = new List<KeyValuePair<Transform, Transform>>();
                var matchedBonesNames = new List<string>();
                foreach (var boneName in sharedBones)
                {
                    Transform originBone = null;
                    Transform targetBone = null;
                    var originPath = targetBoneMap.FetchPair(boneName);
                    var targetPath = Template.BoneMap.FetchPair(boneName);
                    if (targetBoneMap.UsePaths)
                        originBone = temp_target.Find(originPath);
                    else
                        originBoneNames.TryGetValue(originPath, out originBone);
                    if (Template.BoneMap.UsePaths)
                        targetBone = temp_rig.Find(targetPath);
                    else
                        targetBoneNames.TryGetValue(targetPath, out targetBone);
                    if (!targetBone || !originBone)
                        continue;
                    matchedBones.Add(new KeyValuePair<Transform, Transform>(originBone, targetBone));
                    matchedBonesNames.Add(boneName);
                }
                var size = matchedBones.Count;
                rig = new BoneRigInfo();
                rig.originUsePaths = targetBoneMap.UsePaths;
                rig.targetUsePaths = Template.BoneMap.UsePaths;
                for (int i = 0; i < size; i++)
                {
                    var boneName = matchedBonesNames[i];
                    var targetBone = matchedBones[i].Value;
                    var originBone = matchedBones[i].Key;
                    rig.targetPaths.Add(Template.BoneMap.FetchPair(boneName));
                    rig.originPaths.Add(targetBoneMap.FetchPair(boneName));
                    rig.rotations.Add(CalculateRotationOffset(targetBone, originBone));
                    if (rig.rootIndex == -1 && boneName == Template.RootName)
                    {
                        rig.rootIndex = i;
                        rig.offset = CalculatePositionOffset(originBone, targetBone);
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                return false;
            }
            finally
            {
                GameObject.DestroyImmediate(temp_target?.gameObject);
                GameObject.DestroyImmediate(temp_rig?.gameObject);
            }
            return true;
        }
        //HeightRatio Requires a Pivot Point to properly function, TODO include functionality to do so.
        /// <summary>
        /// Calculates height ratio between origin and target y-position.
        /// </summary>
        public static float CalculateHeightRatio(Transform origin, Transform target)
        {
            return origin.position.y / target.position.y;
        }
        /// <summary>
        /// Calculates positional offset between origin and target.
        /// </summary>
        public static Vector3 CalculatePositionOffset(Transform origin, Transform target)
        {
            return Vector3.Project(target.position - origin.position, target.up);
        }
        /// <summary>
        /// Calculates rotational offset between origin and target.
        /// </summary>
        public static Quaternion CalculateRotationOffset(Transform origin, Transform target)
        {
            return Quaternion.Inverse(origin.rotation) * target.rotation;
        }
        /// <summary>
        /// Applys positional offset
        /// </summary>
        public static void ApplyPositionOffset(Transform origin, Transform target, Vector3 offset)
        {
            origin.position = target.position - Vector3.Project(offset, target.up);
        }
        /// <summary>
        /// Applys Rotational Offset
        /// </summary>
        public static void ApplyRotationOffset(Transform origin, Transform target, Quaternion offset)
        {
            origin.rotation = target.rotation * offset;
        }
    }
}

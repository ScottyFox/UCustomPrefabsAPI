using System;
using System.Collections.Generic;
using UnityEngine;
using UCustomPrefabsAPI.Extras.Utility;
//WIP//
namespace UCustomPrefabsAPI.Extras.Animation
{
    public class BoneRigTracker
    {
        //References//
        public Transform origin = null;
        public Transform target = null;
        //References//
        public List<Transform> originBones = new List<Transform>();
        public List<Transform> targetBones = new List<Transform>();
        //Offsets//
        public List<Quaternion> rotations = new List<Quaternion>();
        public bool useRotations;
        public List<Vector3> positions = new List<Vector3>();
        public bool usePositions;
        //Offsets//
        public int rootIndex = -1;
        //Pivot//
        public Transform pivot = null;
        public Vector3 offset = Vector3.zero;
        public BoneRigTracker(Transform origin, Transform target)
        {
            this.origin = origin;
            this.target = target;
        }
        public void SetUpRig(BoneRigInfo info, Transform pivot = null)
        {
            if (info == null)
                return;
            this.pivot = pivot;
            rotations = new List<Quaternion>(info.rotations);
            useRotations = info.useRotations;
            positions = new List<Vector3>(info.positions);
            usePositions = info.usePositions;
            rootIndex = info.rootIndex;
            offset = info.offset;
            RegisterBones(info);
            VerifyBones();
        }
        private void RegisterBones(BoneRigInfo info)
        {
            if (!origin || !target)
                return;
            var size = info.originPaths.Count;
            originBones = new List<Transform>(size);
            targetBones = new List<Transform>(size);
            Dictionary<string, Transform> originNameDict = null;
            Dictionary<string, Transform> targetNameDict = null;
            if (!info.originUsePaths)
                SearchUtils.IterativelyCollectChildNames(origin, ref originNameDict);
            if (!info.targetUsePaths)
                SearchUtils.IterativelyCollectChildNames(target, ref targetNameDict);
            for (int i = 0; i < size; i++)
            {
                Transform originBone;
                Transform targetBone;
                if (info.originUsePaths)
                    originBone = origin.Find(info.originPaths[i]);
                else
                    originNameDict.TryGetValue(info.originPaths[i], out originBone);
                if (info.targetUsePaths)
                    targetBone = target.Find(info.targetPaths[i]);
                else
                    targetNameDict.TryGetValue(info.targetPaths[i], out targetBone);
                originBones.Add(originBone);
                targetBones.Add(targetBone);
            }
            VerifyBones();
        }
        public void VerifyBones()
        {
            for (int i = 0; i < originBones.Count; i++)
            {
                if (i < 0)
                    continue;
                if (!originBones[i] || !targetBones[i])
                {
                    RemoveBone(i);
                    i--;
                }
            }
        }
        public void RemoveBone(int index)
        {
            Debug.LogWarning("Removing Bone!");
            try
            {
                originBones.RemoveAt(index);
                targetBones.RemoveAt(index);
                if (useRotations)
                    rotations.RemoveAt(index);
                if (usePositions)
                    positions.RemoveAt(index);
                if (index < rootIndex && rootIndex != -1)
                    rootIndex--;
                else if (rootIndex == index || rootIndex < 0 || rootIndex == originBones.Count)
                    rootIndex = -1;
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }
        public bool GetBonePair(int index, out Transform origin, out Transform target)
        {
            origin = originBones[index];
            target = targetBones[index];
            if (origin == null || target == null)
            {
                Debug.LogWarning("Bone seem to be invalid, Will Verify Bone Rig.");
                VerifyBones();
                Update();
                return false;
            }
            return true;
        }
        public void Update()
        {
            for (int i = 0; i < originBones.Count; i++)
            {
                if (!GetBonePair(i, out var origin, out var target))
                    return;
                if (useRotations)
                    UpdateBoneRotation(i, origin, target);
                if (usePositions)
                    UpdateBonePosition(i, origin, target);
            }
            UpdatePivotOffset();
        }
        private void UpdateBoneRotation(int index, Transform origin, Transform target)
        {
            RigUtilities.ApplyRotationOffset(origin, target, rotations[index]);
        }
        private void UpdateBonePosition(int index, Transform origin, Transform target)
        {
            RigUtilities.ApplyPositionOffset(origin, target, positions[index]);
        }
        public void UpdatePivotOffset()
        {
            if (rootIndex == -1)
                return;
            if (pivot != null)
                RigUtilities.ApplyPositionOffset(originBones[rootIndex], pivot, offset);
            else
                RigUtilities.ApplyPositionOffset(originBones[rootIndex], targetBones[rootIndex], offset);
        }
    }
}

using System;
using System.Collections.Generic;
using UnityEngine;
namespace UCustomPrefabsAPI.Extras.Animation
{
    [Serializable]
    public class BoneRigInfo
    {
        public List<Quaternion> rotations = new List<Quaternion>();
        public bool useRotations = true;
        public List<Vector3> positions = new List<Vector3>();
        public bool usePositions = false;
        public List<string> originPaths = new List<string>();
        public List<string> targetPaths = new List<string>();
        public int rootIndex = -1;
        public Vector3 offset = Vector3.zero;
        public bool originUsePaths = false;
        public bool targetUsePaths = false;
    }
}

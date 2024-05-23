using System;
using UnityEngine;
namespace UCustomPrefabsAPI.Extras.Animation
{
    [Serializable]
    public partial class RigBuilderTemplate : MonoBehaviour
    {
        public GameObject RigPrefab = null;
        public AnimationClip ReferencePose = null;
        public bool PoseTargetRig = false;
        public bool PoseTemplateRig = false;
        public string RootName = "Hips";
        public virtual bool BuildRig(Transform target, BoneMap targetBoneMap, out BoneRigInfo rig)
        {
            return RigUtilities.TryBuildRig(this, target, targetBoneMap, out rig);
        }
    }
    //Serialization
    public partial class RigBuilderTemplate : MonoBehaviour
    {

        private BoneMap _bonemap = null;
        public BoneMap BoneMap { get { if (_bonemap == null) Deserialize(); return _bonemap; } }

        public string Data = string.Empty;
        public void Deserialize()
        {
            if (string.IsNullOrEmpty(Data))
                return;
            if (_bonemap == null)
                _bonemap = new BoneMap();
            try
            {
                JsonUtility.FromJsonOverwrite(Data, _bonemap);
                _bonemap.ValidateDict();
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }
    }
}

using System;
using UnityEngine;
namespace UCustomPrefabsAPI.Extras.Animation
{
    [Serializable]
    public class RigBuilder : MonoBehaviour
    {
        private BoneRigInfo _rig = null;
        public BoneRigInfo Rig { get { if (_rig == null) Deserialize(); return _rig; } }
        public string Data = string.Empty;
        public void Deserialize()
        {
            if (string.IsNullOrWhiteSpace(Data))
                return;
            _rig = new BoneRigInfo();
            try
            {
                JsonUtility.FromJsonOverwrite(Data, _rig);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }
    }
}

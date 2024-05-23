using UnityEngine;
namespace UCustomPrefabsAPI.Extras.Utility.Serialization
{
    //TODO Create Possible Serialization Helper
    public abstract class MonobehaviourSerialized : MonoBehaviour, ISerializationCallbackReceiver
    {
        public string Data = string.Empty;
        public void OnAfterDeserialize()
        {
            if (string.IsNullOrWhiteSpace(Data))
                return;
            var tmpData = Data;
            JsonUtility.FromJsonOverwrite(Data, this);
            Data = tmpData;
        }
        public void OnBeforeSerialize()
        {
            Data = string.Empty;
            Data = JsonUtility.ToJson(this);
        }
    }
}
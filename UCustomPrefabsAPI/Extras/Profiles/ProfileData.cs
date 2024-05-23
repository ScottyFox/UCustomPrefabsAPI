using System;
using UnityEngine;
//WIP not ready for production use//
//WIP//
namespace UCustomPrefabsAPI.Extras.Profiles
{
    public abstract class ProfileData
    {
        public object Data { get; set; } = null;
        public Type Type { get; set; } = typeof(string);
        public int Size { get; set; } = -1;
        public virtual bool TryGet<T>(out T data)
        {
            data = default;
            bool isValid = Data != null && Type.IsInstanceOfType(typeof(T));
            if (isValid)
                data = (T)Data;
            else
                Debug.Log("Data is invalid.");
            return isValid;
        }
        public virtual string Serailize()
        {
            if (Data == null)
                return null;
            var data = Data.ToString();
            if (Size != -1)
                try
                {
                    data = data.Substring(0, Size);
                }
                catch (Exception) { Debug.Log("Data is too small."); };
            return data;
        }
        public virtual void Deserialize(string data)
        {
            if (string.IsNullOrEmpty(data))
                return;
            if (Size != -1)
                try
                {
                    data = data.Substring(0, Size);
                }
                catch (Exception) { Debug.Log("Data is too small."); };
            Data = data;
        }
    }
}

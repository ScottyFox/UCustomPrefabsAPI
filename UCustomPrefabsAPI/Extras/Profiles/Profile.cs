using System;
using System.Collections.Generic;
using UnityEngine;
//WIP not ready for production use//
//WIP//
namespace UCustomPrefabsAPI.Extras.Profiles
{
    public partial class Profile
    {
        private Dictionary<string, ProfileData> _data = new Dictionary<string, ProfileData>();
        private List<ProfileData> _list = new List<ProfileData>();
        private bool locked = false;
        public int Count { get { return _list.Count; } }
        //WeakReference so we don't hang onto the manager//
        private WeakReference<ProfileManager> _manager;
        public ProfileManager Manager { get { return (_manager == null || !_manager.TryGetTarget(out var target)) ? null : target; } }
        public void SetManager(ProfileManager manager)
        {
            if (manager != null)
                _manager = new WeakReference<ProfileManager>(manager);
        }
        public Profile(ProfileManager manager)
        {
            SetManager(manager);
        }
        public Profile Clone()
        {
            Profile newConfig = new(Manager);
            foreach (var pair in _data)
            {
                var data = pair.Value;
                newConfig.SetData(pair.Key, data.Data, data.GetType());
            }
            foreach (var pair in _data)
            {
                var data = pair.Value;
                int index = _list.IndexOf(data);
                newConfig.SetDataIndex(pair.Key, index);
            }
            return newConfig;
        }
        public void SetData(string key, object value, Type type)
        {
            if (Manager == null)
                /*Manager is required to be registered for type handling.*/
                return;
            if (!_data.TryGetValue(key, out var data) && !locked)
            {
                if (!Manager.TryInstantiateDataType(type, out data))
                    _data.Add(key, data);
                _list.Add(data);
                data.Data = value;
                return;
            }
            data.Data = value;
        }
        public void SetData<T>(string key, T value)
        {
            SetData(key, value, typeof(T));
        }
        public T GetData<T>(string key)
        {
            TryGetData<T>(key, out var value);
            return value;
        }
        public bool TryGetData<T>(string key, out T value)
        {
            var valid = _data.TryGetValue(key, out var data);
            if (valid)
            {
                value = (T)data.Data;
            }
            else
                value = default;
            return valid;
        }
        public bool HasData(string key)
        {
            return _data.ContainsKey(key);
        }
        public void RemoveData(string key)
        {
            if (locked)
                return;
            if (!_data.TryGetValue(key, out var data))
                return;
            _data.Remove(key);
            _list.Remove(data);
        }
        public void PurgeData()
        {
            if (locked)
                return;
            _data.Clear();
            _list.Clear();
        }
        public void SetDataIndex(string key, int index)
        {
            if (locked)
                return;
            if (!_data.TryGetValue(key, out var data))
                return;
            if (_list.IndexOf(data) == index)
                return;
            _list.Remove(data);
            _list.Insert(index, data);
        }
        public int GetDataIndex(string key)
        {
            if (!_data.TryGetValue(key, out var data))
                return -1;
            return _list.IndexOf(data);
        }
        public void LockEdits()
        {
            locked = true;
        }
    }
    public partial class Profile
    {
        private const char START_TAG = '[';
        private const char END_TAG = ']';
        //Possibly Consider using Base64 for non-string values, probably not needed.
        public string Serialize(bool useChecksum = true)
        {
            List<string> tokens = new List<string>();
            for (int i = 0; i < Count; i++)
                tokens.Add(string.Empty);
            foreach (var pair in _data)
            {
                var data = pair.Value;
                var value = data.Serailize();
                //Check if the size is dynamic.
                if (data.Size == -1)
                    value = $"{START_TAG}{value}{END_TAG}";
                tokens[GetDataIndex(pair.Key)] = value;
            }
            var output = string.Join(string.Empty, tokens);
            if (useChecksum)
                output = GenerateChecksum(output) + output;
            return output;
        }
        public void Deserialize(string input, bool hasChecksum = true, int offset = 0)
        {
            if (hasChecksum && !ValidateChecksum(input))
            {
                Debug.LogWarning("Profile data does not match checksum--");
                Debug.LogWarning("data may be incorrect... Ignoring");
                return;
            }
            int pointer = hasChecksum ? 1 : 0;
            int size;
            for (int i = offset; i < _list.Count; i++)
            {
                var data = _list[i];
                size = data.Size;
                bool isDynamic = size == -1;
                if (isDynamic)
                    size = FindDataSize(input, pointer);
                if (size <= 0)
                    continue;
                string token;
                if (isDynamic)
                    token = input.Substring(pointer + 1, size - 1);
                else
                    token = input.Substring(pointer, size);
                data.Deserialize(token);
                pointer += size;
            }
        }
        private bool ValidateChecksum(string input)
        {
            var data_token = input[0];
            var token = GenerateChecksum(input, 1);
            return data_token == token;
        }
        private char GenerateChecksum(string input, int offset = 0)
        {
            int pointer = offset;
            var token = '#';
            while (pointer < input.Length)
            {
                char data = input[pointer];
                token ^= data;
                bool flip = (token % 2 == 0);
                token = (char)((token % ('Z' - 'A' + 1)) + 'A');
                token = (flip) ? char.ToUpper(token) : char.ToLower(token);
                pointer++;
            }
            return token;
        }
        private int FindDataSize(string input, int pointer)
        {
            pointer = input.IndexOf(START_TAG, pointer);
            if (pointer < 0)
                return -1;
            int length = 0;
            int nestedCount = 0;
            while (pointer < input.Length)
            {
                switch (input[pointer])
                {
                    case START_TAG:
                        nestedCount++;
                        break;
                    case END_TAG:
                        nestedCount--;
                        break;
                }
                if (nestedCount == 0)
                    break;
                length++;
                pointer++;
            }
            return length;
        }
    }
}

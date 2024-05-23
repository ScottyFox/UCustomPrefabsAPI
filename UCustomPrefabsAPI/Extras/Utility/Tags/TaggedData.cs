using System;
using System.Collections.Generic;
using UnityEngine;
namespace UCustomPrefabsAPI.Extras.Utility
{
    public class TaggedData : TaggedBehaviour
    {
        [Tooltip("WIP Tag Data to help serialize things in UCustomPrefabs.")]
        public string[] Data;
        private Dictionary<string, string> _tagdata;
        public Dictionary<string, string> TagData
        {
            get
            {
                if (_tagdata == null)
                {
                    _tagdata = new Dictionary<string, string>();
                    InitTagData();
                }
                return _tagdata;
            }
        }
        /// <summary>
        /// Internal Function. Initializes the Tags by adding it to a HashSet.
        /// </summary>
        private void InitTagData()
        {
            for (int i = 0; i < Data.Length && i < Tags.Length; i++)
            {
                try
                {
                    _tagdata.Add(Tags[i], Data[i]);
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                }
            }
        }
        /// <summary>
        /// Adds a Tag with Data pair, 
        /// ex: <code>AddTag("tag","data");</code>
        /// WIP Needs data pair otherwise will not add tag.
        /// </summary>
        public override void AddTag(params string[] tags)
        {
            for (int i = 0; i + 1 < tags.Length; i += 2)
            {
                TagSet.Add(tags[i]);
                try
                {
                    TagData.Add(tags[i], tags[i + 1]);
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                }
            }
        }
        /// <summary>
        /// Removes a Tag.
        /// </summary>
        public override void RemoveTag(params string[] tags)
        {
            foreach (var tag in tags)
            {
                TagSet.Remove(tag);
                try
                {
                    TagData.Remove(tag);
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                }
            }
        }
        /// <summary>
        /// Tries to fetch data associated with a tag.
        /// </summary>
        public bool TryGetTagData(string tag, out string data)
        {
            return TagData.TryGetValue(tag, out data);
        }
    }
}

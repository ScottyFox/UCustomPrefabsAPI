using System.Collections.Generic;
using UnityEngine;

namespace UCustomPrefabsAPI.Extras.Utility
{
    public class TaggedBehaviour : MonoBehaviour
    {
        [Tooltip("WIP Tags to help organize things in UCustomPrefabs.")]
        public string[] Tags;
        private HashSet<string> _tagset;
        public HashSet<string> TagSet
        {
            get
            {
                if (_tagset == null)
                {
                    _tagset = new HashSet<string>();
                    InitTags();
                }
                return _tagset;
            }
        }
        /// <summary>
        /// Internal Function. Initializes the Tags by adding it to a HashSet.
        /// </summary>
        private void InitTags()
        {
            foreach (var tag in Tags)
            {
                _tagset.Add(tag);
            }
        }
        /// <summary>
        /// Adds a Tag.
        /// </summary>
        public virtual void AddTag(params string[] tags)
        {
            foreach (var tag in tags)
                TagSet.Add(tag);
        }
        /// <summary>
        /// Removes a Tag.
        /// </summary>
        public virtual void RemoveTag(params string[] tags)
        {
            foreach (var tag in tags)
                TagSet.Remove(tag);
        }
        /// <summary>
        /// Returns bool if has Tag
        /// </summary>
        public virtual bool HasTag(params string[] tags)
        {
            foreach (var tag in tags)
                if (!TagSet.Contains(tag))
                    return false;
            return tags.Length > 0;
        }
        /// <summary>
        /// Clears all Tags.
        /// </summary>
        public virtual void ClearTags()
        {
            TagSet.Clear();
        }
        /// <summary>
        /// Finds Tag(s) in first child. 
        /// ex: <code>FindTagInChildren("tag1","tag2","tag3")</code>
        /// </summary>
        public virtual TaggedBehaviour FindTagInChildren(params string[] tags)
        {
            foreach (var tagged in GetComponentsInChildren<TaggedBehaviour>(true))
            {
                if (tagged.HasTag(tags))
                    return tagged;
            }
            return null;
        }
        /// <summary>
        /// Finds Tag(s) in multiple children. 
        /// ex: <code>FindTagsInChildren("tag1","tag2","tag3")</code>
        /// </summary>
        public virtual List<TaggedBehaviour> FindTagsInChildren(params string[] tags)
        {
            var targets = new List<TaggedBehaviour>();
            foreach (var tagged in GetComponentsInChildren<TaggedBehaviour>(true))
            {
                if (tagged.HasTag(tags))
                    targets.Add(tagged);
            }
            return targets;
        }
    }
}

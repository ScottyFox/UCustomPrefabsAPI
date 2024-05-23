using System.Collections.Generic;
using UnityEngine;
namespace UCustomPrefabsAPI.Extras.AssetBundles
{
    public static class AssetBundleRegistry
    {
        private static Dictionary<string, AssetBundleData> AssetBundles = new Dictionary<string, AssetBundleData>();
        /// <summary>
        /// Registers a AssetBundle, Will attempt to load from path, if fails it will not register.
        /// path will load relative from the assembly path of <typeparamref name="T"/>
        /// </summary>
        public static bool Register<T>(string path, string name = null)
        {
            var assetbundle = new AssetBundleData(typeof(T), path, name, false);
            if (HasAssetBundle(assetbundle.name) || !assetbundle.LoadAssetBundle())
                return false;
            AssetBundles.Add(assetbundle.name, assetbundle);
            return true;
        }
        /// <summary>
        /// Registers a AssetBundle, Will attempt to load from path, if fails it will not register.
        /// path will load relative from the assembly path of <typeparamref name="T"/>
        /// </summary>
        public static bool Register<T>(string path, out string name)
        {
            name = null;
            var assetbundle = new AssetBundleData(typeof(T), path, null, false);
            if (HasAssetBundle(assetbundle.name) || !assetbundle.LoadAssetBundle())
                return false;
            AssetBundles.Add(assetbundle.name, assetbundle);
            name = assetbundle.name;
            return true;
        }
        /// <summary>
        /// Registers a AssetBundle, Will attempt to load from embedded path, if fails it will not register.
        /// path will load from the assembly <typeparamref name="T"/>
        /// </summary>
        public static void RegisterEmbedded<T>(string path, string name = null)
        {
            var assetbundle = new AssetBundleData(typeof(T), path, name, true);
            if (HasAssetBundle(assetbundle.name))
                return;
            if (assetbundle.LoadAssetBundle())
                AssetBundles.Add(assetbundle.name, assetbundle);
        }
        /// <summary>
        /// Removes AssetBundle.
        /// </summary>
        public static void Remove(string assetbundleName, bool unloadAllLoadedObjects)
        {
            if (!AssetBundles.TryGetValue(assetbundleName, out var assetbundle))
                return;
            assetbundle.Unload(unloadAllLoadedObjects);
            AssetBundles.Remove(assetbundleName);
        }
        /// <summary>
        /// Has AssetBundle.
        /// </summary>
        public static bool HasAssetBundle(string name)
        {
            return AssetBundles.ContainsKey(name);
        }
        /// <summary>
        /// Loads a GameObject from a AssetBundle.
        /// </summary>
        public static GameObject LoadPrefab(string assetbundleName, string name)
        {
            if (!AssetBundles.TryGetValue(assetbundleName, out var assetbundle))
                return null;
            return assetbundle.LoadPrefab(name);
        }
        /// <summary>
        /// Loads a Asset from a AssetBundle.
        /// </summary>
        public static T LoadAsset<T>(string assetbundleName, string name) where T : UnityEngine.Object
        {
            if (!AssetBundles.TryGetValue(assetbundleName, out var assetbundle))
                return null;
            return assetbundle.LoadAsset<T>(name);
        }
    }
}

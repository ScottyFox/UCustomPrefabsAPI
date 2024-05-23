using System.IO;
using System;
using System.Reflection;
using UnityEngine;
//TODO add support for assetbundle management.
//? Unsure what I meant at the time. Possibly the ability to unload at anytime?
namespace UCustomPrefabsAPI.Extras.AssetBundles
{
    public class AssetBundleData
    {
        public string name;
        public string path;
        public bool embedded;
        public Assembly assembly;
        public AssetBundle assetbundle;
        public AssetBundleData(Type origin, string path, string name = null, bool embedded = false)
        {
            assembly = origin.Assembly;
            this.path = path;
            this.embedded = embedded;
            if (string.IsNullOrWhiteSpace(name))
                name = Path.GetFileName(path);
            this.name = name;
        }
        /// <summary>
        /// WIP Handles loading AssetBundle
        /// </summary>
        public bool LoadAssetBundle()
        {
            if (string.IsNullOrWhiteSpace(path))
                return false;
            if (embedded)
                return LoadEmbedded();
            else
                return Load();
        }
        /// <summary>
        /// WIP Loads AssetBundle resource from provided information.
        /// </summary>
        private bool Load()
        {
            assetbundle = null;
            try
            {
                string filePath = Path.Combine(Path.GetDirectoryName(assembly.Location), path);
                assetbundle = AssetBundle.LoadFromFile(filePath);
            }
            catch (Exception e)
            {
                Debug.Log("Unable to load assetbundle. Make sure your pathing is correct.");
                Debug.Log("Make sure to use LoadEmbeddedAssetBundle if using an embedded resource");
                Debug.LogError(e);
            }
            if (!assetbundle)
                return false;
            embedded = false;
            return true;
        }
        /// <summary>
        /// WIP Loads Embedded AssetBundle resource from provided information.
        /// </summary>
        private bool LoadEmbedded()
        {
            assetbundle = null;
            try
            {
                var embeddedpath = path.Replace('\\', '.');
                embeddedpath = embeddedpath.Replace('/', '.');
                embeddedpath = assembly.FullName.Split(',')[0] + "." + embeddedpath;
                using (var stream = assembly.GetManifestResourceStream(embeddedpath))
                    assetbundle = AssetBundle.LoadFromStream(stream);
            }
            catch (Exception e)
            {
                Debug.Log("Unable to load assetbundle. Make sure your pathing is correct.");
                Debug.Log("Embedded Resources use \".\" instead of the usual \"\\\".");
                Debug.LogError(e);
            }
            if (!assetbundle)
                return false;
            return true;
        }
        /// <summary>
        /// Loads a GameObject prefrab from AssetBundle
        /// </summary>
        public GameObject LoadPrefab(string name)
        {
            return assetbundle?.LoadAsset<GameObject>(name);
        }
        /// <summary>
        /// Loads Asset of <typeparamref name="T"/> from AssetBundle
        /// </summary>
        public T LoadAsset<T>(string name) where T : UnityEngine.Object
        {
            return assetbundle?.LoadAsset<T>(name);
        }
        /// <summary>
        /// Unloads the AssetBundle.
        /// </summary>
        public void Unload(bool unloadAllLoadedObjects)
        {
            assetbundle?.Unload(unloadAllLoadedObjects);
        }
    }
}

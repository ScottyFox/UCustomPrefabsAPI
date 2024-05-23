using System;
using System.Collections.Generic;
using System.IO;
using UCustomPrefabsAPI.Extras.AssetBundles;
using UnityEngine;
//WIP Implement External Template Creation//
namespace UCustomPrefabsAPI
{
    public static class UCustomPrefabFileHelper
    {
        private static bool IsAssetBundle(string path)
        {
            bool valid = false;
            try
            {
                using (FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read))
                {
                    using (BinaryReader reader = new BinaryReader(fileStream))
                    {
                        byte[] headerBytes = reader.ReadBytes(8);
                        string header = System.Text.Encoding.ASCII.GetString(headerBytes);
                        if (header.StartsWith("UnityFS") ||
                            header.StartsWith("UnityRaw") ||
                            header.StartsWith("UnityWeb"))
                            valid = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error Reading File : {ex.Message}");
            }
            return valid;
        }
        public static void TryToLoadTemplateAssetBundlesFromPath<T>(string path)
        {
            var assemblyPath = Path.GetDirectoryName(typeof(T).Assembly.Location);
            var fullPath = Path.Combine(assemblyPath, path);
            var files = Directory.GetFiles(fullPath);
            var ValidAssetBundlePaths = new List<string>();
            foreach (var file in files)
            {
                if (IsAssetBundle(file))
                {
                    ValidAssetBundlePaths.Add(Path.Combine(path, Path.GetFileName(file)));
                }
            }
            foreach (var assetbundlePath in ValidAssetBundlePaths)
            {
                if (AssetBundleRegistry.Register<T>(assetbundlePath, out var name))
                    if (!TryToLoadTemplatesFromAssetBundle(name))
                    {
                        Debug.Log($"Unable to load Templates from Assetbundle : {name}");
                        AssetBundleRegistry.Remove(name, true);
                    }
                else
                    Debug.Log($"Unable to Register AssetBundle : {assetbundlePath}");
            }
        }
        public static bool TryToLoadTemplatesFromAssetBundle(string assetBundleName)
        {
            var templatesJson = AssetBundleRegistry.LoadAsset<TextAsset>(assetBundleName, "templates.ucp.txt");
            if (templatesJson == null || string.IsNullOrWhiteSpace(templatesJson.text))
            {
                Debug.Log($"Unable to Read Templates Json from {assetBundleName}");
                return false;
            }
            var data = new UCustomPrefab_AssetBundle_TemplatesJSON();
            JsonUtility.FromJsonOverwrite(templatesJson.text, data);
            if (!data.Verify())
            {
                Debug.Log("Templates Data is Invalid!");
                return false;
            }
            for (int i = 0; i < data.Template_Names.Count; i++)
            {
                var templatePrefab = AssetBundleRegistry.LoadPrefab(assetBundleName, data.Template_Prefabs[i]);
                if (templatePrefab == null)
                    continue;
                TemplateRegistry.Register(data.Template_Names[i], templatePrefab);
            }
            return true;
        }
    }
    [Serializable]
    public class UCustomPrefab_AssetBundle_TemplatesJSON
    {
        public string Name = null;
        public string Author = null;
        public string Description = null;
        public List<string> Template_Names = new List<string>();
        public List<string> Template_Prefabs = new List<string>();
        public bool Verify() =>
            !string.IsNullOrWhiteSpace(Name) &&
            !string.IsNullOrWhiteSpace(Author) &&
            !string.IsNullOrWhiteSpace(Description) &&
            Template_Names.Count != 0 &&
            Template_Prefabs.Count != 0;
    }
}

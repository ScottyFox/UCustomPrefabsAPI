using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using System;
using System.Reflection;
using UCustomPrefabsAPI.ContentWarning.CustomActions;
using UCustomPrefabsAPI.ContentWarning.Patches.Mirror;
using UCustomPrefabsAPI.Extras.AssetBundles;
using UCustomPrefabsAPI.Extras.CustomActions;
namespace UCustomPrefabsAPI.ContentWarning
{
    public static class PluginInfo
    {
        public const string GUID = "UCustomPrefabsAPI.ContentWarning";
        public const string NAME = "UCustomPrefabsAPI.ContentWarning";
        public const string VERSION = "0.0.4";
        public const string WEBSITE = "https://github.com/ScottyFox/UCustomPrefabsAPI";
    }
    [BepInPlugin(PluginInfo.GUID, PluginInfo.NAME, PluginInfo.VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        public static ConfigFile config;
        public static ConfigEntry<bool> UseMirrorSelector;
        public static ConfigEntry<string> TemplatesAssetBundlesPath;
        public static ConfigEntry<string> TemplatesFileFinderName;
        public static ConfigEntry<string> localPlayerConfig;
        internal static Harmony instance = new(PluginInfo.GUID);
        private void Awake()
        {
            try
            {
                SetUpConfig();
                RegisterAssetBundles();
                RegisterCustomActions();
                RegisterCustomTemplates();
                BulkLoadTemplates();
                instance.PatchAll(Assembly.GetExecutingAssembly());
                Logger.LogInfo($"Plugin {PluginInfo.GUID} is loaded!");
            }
            catch (Exception exception)
            {
                Logger.LogInfo($"Plugin {PluginInfo.GUID} failed to load...");
                Logger.LogError(exception);
            }
        }
        public void SetUpConfig()
        {
            config = base.Config;
            UseMirrorSelector = Plugin.config.Bind<bool>("ContentWarning Extras", "Use Mirror Selector", true);
            TemplatesAssetBundlesPath = Plugin.config.Bind<string>("UCustomPrefabs Paths", "Templates Folder", "Templates/");
            TemplatesFileFinderName = Plugin.config.Bind<string>("UCustomPrefabs Paths", "Templates Folder Finder File", "ucustomprefabs.templates.txt");
            localPlayerConfig = Plugin.config.Bind<string>("Player Config", "Current Template(s)", "");
        }
        public static void RegisterAssetBundles()
        {
            AssetBundleRegistry.RegisterEmbedded<Plugin>("Assets/mirrorselector", "CW_MirrorSelector_AssetBundle");
        }
        public static void RegisterCustomActions()
        {
            CustomActionsRegistry.Register<CW_RigHelper>("CW_RigHelper");
            CustomActionsRegistry.Register<CW_ShaderFix>("CW_ShaderFix");
            CustomActionsRegistry.Register<CW_VisorHelper>("CW_VisorHelper");
            CustomActionsRegistry.Register<CW_VisorTextHelper>("CW_VisorTextHelper");
            CustomActionsRegistry.Register<CW_MirrorSelector>("CW_MirrorDebugSelector");
        }
        public static void RegisterCustomTemplates()
        {
            TemplateRegistry.Register("CW_MirrorSelector", AssetBundleRegistry.LoadPrefab("CW_MirrorSelector_AssetBundle", "mirrortest"));
        }
        public static void BulkLoadTemplates()
        {
            var Directories = UCustomPrefabFileHelper.FindDirectoriesWithFileName(BepInEx.Paths.PluginPath, TemplatesFileFinderName.Value);
            foreach (var directory in Directories)
            {
                UCustomPrefabFileHelper.TryToLoadTemplateAssetBundlesFromPath<Plugin>(directory);
            }
        }
    }
}

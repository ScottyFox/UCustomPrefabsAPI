using System.Collections.Generic;
//WIP Not ready for production use//
namespace UCustomPrefabsAPI.Extras.Profiles
{
    internal static class ProfileRegistry
    {
        private static Dictionary<string, ProfileManager> ProfileManagers = new Dictionary<string, ProfileManager>();
        public static ProfileManager CreateProfileManager(string uid)
        {
            if (ProfileManagers.TryGetValue(uid,out var manager))
                return manager;
            manager = new ProfileManager();
            ProfileManagers.Add(uid, manager);
            return manager;
        }
        public static Profile CreateProfile(string uid, string name)
        {
            ProfileManagers.TryGetValue(uid, out var profiles);
            return profiles?.CreateProfile(name);
        }
        public static void RegisterProfileTemplate(string uid, Profile template)
        {
            ProfileManagers.TryGetValue(uid, out var profiles);
            profiles?.RegisterTemplate(template);
        }
        public static bool TryGetProfile(string uid, string name, out Profile profile)
        {
            profile = null;
            if (!ProfileManagers.TryGetValue(uid, out var profiles))
                return false;
            return profiles.TryGetProfile(name, out profile);
        }
    }
}
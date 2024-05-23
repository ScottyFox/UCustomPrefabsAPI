using System;
using System.Collections.Generic;
//WIP not ready for production use//
//WIP Will move towards a better method of serialization to express data types//
namespace UCustomPrefabsAPI.Extras.Profiles
{
    public partial class ProfileManager
    {
        private Dictionary<string, Profile> Profiles = new Dictionary<string, Profile>();
        private Profile Template = null;
        public Profile InstantiateProfile()
        {
            return new Profile(this);
        }
        public Profile CreateProfile(string name)
        {
            if (Template == null)
                return null;
            if (TryGetProfile(name, out var profile))
                return profile;
            profile = Template.Clone();
            profile.LockEdits();
            Profiles.Add(name, profile);
            return profile;
        }
        public bool TryGetProfile(string name, out Profile config)
        {
            return Profiles.TryGetValue(name, out config);
        }
        public void RegisterTemplate(Profile profile)
        {
            ClearTemplate();
            Template = profile; ;
        }
        public void ClearProfiles()
        {
            Profiles.Clear();
        }
        public void ClearTemplate()
        {
            Template = null;
        }
    }
    //Type Management//
    public partial class ProfileManager
    {
        private Dictionary<Type, Type> DataTypes = new Dictionary<Type, Type>
        {
            { typeof(string), typeof(ProfileData) },
            { typeof(float), typeof(ProfileData_Float) },
            { typeof(int), typeof(ProfileData_Integer) },
            { typeof(bool), typeof(ProfileData_Boolean) },
            { typeof(char), typeof(ProfileData_Char) }
        };
        public void RegisterDataType<T>(Type type) where T : ProfileData
        {
            try
            {
                DataTypes.Add(type, typeof(T));
            }
            catch (ArgumentException)
            {
                Console.WriteLine("Data Type Already Registered.");
            }
        }
        public bool TryInstantiateDataType(Type type, out ProfileData data)
        {
            data = null;
            if (DataTypes.TryGetValue(type, out var dataType))
            {
                data = (ProfileData)Activator.CreateInstance(dataType);
            }
            return data != null;
        }
    }
}

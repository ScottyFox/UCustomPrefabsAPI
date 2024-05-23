//WIP not ready for production use//
//WIP//
namespace UCustomPrefabsAPI.Extras.Profiles 
{
    public class ProfileData_Float : ProfileData
    {
        public ProfileData_Float()
        {
            Type = typeof(float);
        }
        public override void Deserialize(string data)
        {
            if (float.TryParse(data, out var result))
            {
                Data = result;
            }
        }
    }
    public class ProfileData_Integer : ProfileData
    {
        public ProfileData_Integer()
        {
            Type = typeof(int);
        }
        public override void Deserialize(string data)
        {
            if (int.TryParse(data, out var result))
            {
                Data = result;
            }
        }
    }
    public class ProfileData_Boolean : ProfileData
    {
        public ProfileData_Boolean()
        {
            Type = typeof(bool);
            Size = 1;
        }
        public override string Serailize()
        {
            return TryGet<bool>(out var data) ? data ? "1" : "0" : "0";
        }
        public override void Deserialize(string data)
        {
            if (string.IsNullOrEmpty(data))
                return;
            if (data.Length == 1)
            {
                var upper = data.ToUpper();
                switch (upper)
                {
                    case "T":
                        Data = true;
                        break;
                    case "F":
                        Data = false;
                        break;
                    case "1":
                        Data = true;
                        break;
                    case "0":
                        Data = false;
                        break;
                }
            }
            else
            if (bool.TryParse(data, out var result))
            {
                Data = result;
            }
        }
    }
    public class ProfileData_Char : ProfileData
    {
        public ProfileData_Char()
        {
            Type = typeof(char);
            Size = 1;
        }
        public override void Deserialize(string data)
        {
            if (!string.IsNullOrEmpty(data) && data.Length > 0)
                Data = data[0];
        }
    }
}

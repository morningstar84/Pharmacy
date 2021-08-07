using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Data.Models
{
    public partial class User : PersistedEntity
    {
        public int Id { get; set; }

        [JsonProperty("username")] public string? Username { get; set; }

        public byte[]? PasswordHash { get; set; }
        public byte[]? PasswordSalt { get; set; }

        [JsonProperty("enabled")] public bool Enabled { get; set; } = true;

        [JsonProperty("deleted")] public bool Deleted { get; set; }

        [JsonProperty("user_role")] public UserRole UserRole { get; set; } = UserRole.RegisteredUser;
    }

    public partial class User
    {
        public static User? FromJson(string json)
        {
            return JsonConvert.DeserializeObject<User>(json, Converter.Settings);
        }
    }

    public static class Serialize
    {
        public static string ToJson(this User self)
        {
            return JsonConvert.SerializeObject(self, Converter.Settings);
        }
    }

    internal static class Converter
    {
        public static readonly JsonSerializerSettings? Settings = new()
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters =
            {
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            }
        };
    }
}
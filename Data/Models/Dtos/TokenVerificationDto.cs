using System.ComponentModel.DataAnnotations;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Data.Models.Dtos
{
    public partial class TokenVerificationDto
    {
        [Required]
        public string Token { get; set; }

    }
    
     public partial class TokenVerificationDto
     {
         public static TokenVerificationDto? FromJson(string json)
         {
             return JsonConvert.DeserializeObject<TokenVerificationDto>(json, Converter.Settings);
         }
     }

     public static class Serialize
     {
         public static string ToJson(this TokenVerificationDto self)
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
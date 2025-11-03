using System.Text.Json.Serialization;

namespace Crime_Management_System.Models
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum UserRole 
    { 
        Admin,
        Investigator,
        Officer,
        Citizen
    }
}

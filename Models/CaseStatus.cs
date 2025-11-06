using System.Text.Json.Serialization;

namespace Crime_Management_System.Models
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum CaseStatus 
    { 
        Pending,
        Ongoing,
        Closed
    }
}

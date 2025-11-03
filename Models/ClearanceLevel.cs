using System.Text.Json.Serialization;

namespace Crime_Management_System.Models
{
    // to represent clearance levels in swagger
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ClearanceLevel 
    { 
        Low = 0,
        Medium = 1,
        High = 2,
        Critical = 3 
    }
}

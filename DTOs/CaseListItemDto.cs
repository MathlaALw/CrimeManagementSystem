using Crime_Management_System.Models;
using System;

namespace Crime_Management_System.DTOs
{
    public class CaseListItemDto
    {
       public int Id { get; set; }
        public string CaseNumber { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string? AreaCity { get; set; }
        public string? CaseType { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public ClearanceLevel AuthorizationLevel { get; set; }
    }
}

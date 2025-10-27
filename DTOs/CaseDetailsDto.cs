using Crime_Management_System.Models;

namespace Crime_Management_System.DTOs
{
    public class CaseDetailsDto
    {
         public int Id { get; set; }
         public string CaseNumber { get; set; }
          public  string Name { get; set; }

         public string? Description {get; set; }
         public string? AreaCity { get; set; }
         public string? CaseType { get; set; }
         public  CaseStatus Status { get; set; }
         public  ClearanceLevel AuthorizationLevel { get; set; }
         public  string CreatedBy { get; set; }
         public  DateTime CreatedAt { get; set; }
         public string? ReportedBy { get; set; } // rebresents the user who reported the crime (via linked CrimeReport)
        public int Assignees { get; set; }
         public int Evidences { get; set; }
        public  int Suspects { get; set; }
        public int Victims { get; set; }
        public  int Witnesses { get; set; }
    }
}

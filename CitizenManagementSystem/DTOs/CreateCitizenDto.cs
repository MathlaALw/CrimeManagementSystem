namespace CitizenManagementSystem.DTOs
{
    public class CreateCitizenDto
    {
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
    }
}

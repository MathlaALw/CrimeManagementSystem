namespace CitizenManagementSystem.Models
{
    public class Citizen
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        //city of residence
        public string City { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
      
       

    }
}

namespace Crime_Management_System.Servises
{
    public interface IPasswordService
    {
        string Hash(string password);
        bool Verify(string hash, string password);
    }
}

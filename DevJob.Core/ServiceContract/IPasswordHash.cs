
namespace DevJob.Application.ServiceContract
{
    public interface IPasswordHash
    {
       public string PasswordHashed(string password);
    }
}

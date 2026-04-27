using DevJob.Application.DTOs.User;
using DevJob.Domain.Entities;
using DevJob.Application.ServiceContract;
namespace DevJob.Application.Repository_Contract
{
    public interface IUserCvDataRepository:IRepository<UserCvData>
    {
        Task<UserCvData?> GetUserDataWithCv(int userId);
        Task<UserCvData?> GetActiveUserWithCvById(int userId);
        Task<List<GetUserDataDto>> GetUserData();
        Task<List<int>> GetUserIds(string appUser);
        
    }
}

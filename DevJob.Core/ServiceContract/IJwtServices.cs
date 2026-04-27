using DevJob.Application.DTOs.Auth;
using DevJob.Domain.Entities;
using System.Security.Claims;

namespace DevJob.Application.ServiceContract
{
    public interface IJwtServices
    {
       Task< string>  CreateTokenAsync(ApplicationUser user);
        string GenerateRefreshToken();
        ClaimsPrincipal? GenerateUserPrincipal(string? token);
        Task<AuthResponse> GenerateNewAccessToken(TokenModel tokenModel);

    }
}

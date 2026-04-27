using DevJob.Application.DTOs.Auth;

namespace DevJob.Application.ServiceContract
{
    public interface IAuthService 
    {
       Task<AuthResponse> UserRegister(RegisterDTO registerDTO);
       Task<AuthResponse> Login(LoginDTO loginDTO);
       // Task<AuthResponse> GoogleResponse(string googleToken);
        Task<PasswordDto> ForgetPassword(ForgetPasswordDTO forgetPasswordDTO);
        Task<PasswordDto> ResetPassword(ResetPasswordDTO resetPasswordDTO);
        Task<AuthResponse> ConfirmEmail(string email,string token);
        Task<CompanyAuthResponse> CompanyRegister(CompanyRegisterDTO companyRegisterDTO);
        Task<AuthResponse> ResendConfirmationEmail(string email);


        }
}

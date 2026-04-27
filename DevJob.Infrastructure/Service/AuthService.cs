using AutoMapper;
using DevJob.Application.DTOs.Auth;
using DevJob.Domain.Entities;
using DevJob.Application.ServiceContract;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Serilog;
using System.Text;
namespace DevJob.Infrastructure.Service
{
    public class AuthService : IAuthService 
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly IMapper mapper;
        private readonly IJwtServices jwtServices;
        private readonly IWebHostEnvironment webHostEnvironment;
        private readonly IConfiguration configuration;
        private readonly IMailServices mailServices;

        private readonly IUnitOfWork unitOfWork;
        public AuthService(UserManager<ApplicationUser> userManager,
            IMapper mapper,
            SignInManager<ApplicationUser> signInManager,
            IJwtServices jwtServices,
            IConfiguration configuration
            ,IUnitOfWork unitOfWork
            ,IMailServices mailServices
            ,IWebHostEnvironment webHostEnvironment)
        {
            this.mapper = mapper;
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.jwtServices = jwtServices;
            this.configuration = configuration;
            this.mailServices = mailServices;
            this.webHostEnvironment = webHostEnvironment;
            this.unitOfWork = unitOfWork;
        }
        async Task sendEmail(string fileName, string replaceWord, string url, string Email)
        {
            string path = Path.Combine(webHostEnvironment.WebRootPath, "Templete", fileName);
            string content = File.ReadAllText(path);
            string finalContent = content.Replace(replaceWord, url);
            await mailServices.SendEmailAsync(mailTo: Email, subject: "DevJob", null, finalContent);
        }
        public async Task< AuthResponse> UserRegister(RegisterDTO registerDTO)
        {
            await unitOfWork.BeginTransaction();
            try { 
                var user = mapper.Map<ApplicationUser>(registerDTO);
                user.CreatedAt = DateOnly.FromDateTime(DateTime.Now);
                var response = await userManager.CreateAsync(user, registerDTO.Password);
                if (response.Succeeded) {

                    await userManager.AddToRoleAsync(user, "DEVELOPER");
                    //get profile user 
                    var userProfile = mapper.Map<UserProfile>(registerDTO);
                    userProfile.userId = user.Id;
                    //save in database
                    await unitOfWork.UserProfile.AddAsync(userProfile);
                    await unitOfWork.SaveChangesAsync();

                    //create confirmation token
                    var confirmationToken = await userManager.GenerateEmailConfirmationTokenAsync(user);
                    var encodedToken = Uri.EscapeDataString(confirmationToken);
                    var baseUrl = configuration["AppUrl"];
                    //send confirmation token to email
                    var confirmationLink = $"{baseUrl}/api/Auth/confirm-email?email={user.Email}&token={encodedToken}";
                    await sendEmail("ConfirmEmail.html", "{ConfirmationLink}", confirmationLink, user.Email);
                    //create jwt token
                    string token = await jwtServices.CreateTokenAsync(user);
                    var expire = configuration["RefreshToken:Expire"];
                    string refreshToken = jwtServices.GenerateRefreshToken();
                    user.RefreshToken = refreshToken;
                    user.RefreshTokenExpirationDate = DateTime.Now.AddMinutes(Convert.ToInt32(expire));
                    await userManager.UpdateAsync(user);

                    //sign in
                    await signInManager.SignInAsync(user, false);
                    await unitOfWork.CommitAsync();
                    return new AuthResponse()
                    {
                        Success = true,
                        Token = token,
                        RefreshToken = refreshToken,
                        RefreshtTokenExpirationDate = user.RefreshTokenExpirationDate
                    };
                }
                else {
                    string errors = string.Join(", ", response.Errors.Select(x => x.Description));
                    throw new ArgumentException(errors);
                }
            }

            catch(Exception ex)
            {
               await unitOfWork.RollBackAsync();
                throw;
            }
            
        }

        public async Task<AuthResponse> Login(LoginDTO loginDTO)
        {
            //check if user is found
            var user = await userManager.FindByEmailAsync(loginDTO.Email);
           if (user==null || !await userManager.CheckPasswordAsync(user, loginDTO.Password))
            {
                //not found
                throw new UnauthorizedAccessException("Invalid Email or Password");
            }
           //check email confirmation
            if (!await userManager.IsEmailConfirmedAsync(user))
                throw new UnauthorizedAccessException("Please confirm your email first.");

            await signInManager.SignInAsync(user, false);
           
            //create token
            string token = await jwtServices.CreateTokenAsync(user);
            var expire = configuration["RefreshToken:Expire"];
            string refreshToken = jwtServices.GenerateRefreshToken();
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpirationDate = DateTime.Now.AddMinutes(Convert.ToInt32(expire));
           var res= await userManager.UpdateAsync(user);
            if (!res.Succeeded)
                throw new UnauthorizedAccessException("Error occurred while updating user tokens.");
            
            return new AuthResponse()
            {
                Success = true,
                Token = token,
                RefreshToken=user.RefreshToken,
                RefreshtTokenExpirationDate=user.RefreshTokenExpirationDate
            };
        }
        public async Task<AuthResponse> ConfirmEmail(string email,string token)
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user == null)
                return new AuthResponse() { Success = false, Message = "Invalid Email" };
            var encodedToken = Uri.UnescapeDataString(token);

            var res =await userManager.ConfirmEmailAsync(user, encodedToken);
            if (res.Succeeded)
                return new AuthResponse() { Success = true };
            var errors = string.Join(", ", res.Errors.Select(e => e.Description));
            throw new InvalidOperationException(errors);
        }
     //modification
        public async Task<PasswordDto> ForgetPassword(ForgetPasswordDTO forgetPasswordDTO)
        {
            var user = await userManager.FindByEmailAsync(forgetPasswordDTO.Email);
            if (user == null)
                return new PasswordDto() { Message = "User not Found", Success = false };
            var token = await userManager.GeneratePasswordResetTokenAsync(user);
            var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
            var param = new Dictionary<string, string>
            {
                {"token" ,encodedToken},
                {"email",forgetPasswordDTO.Email }
            };
            var callback = QueryHelpers.AddQueryString(forgetPasswordDTO.ClientUri, param);
            string path = Path.Combine(webHostEnvironment.WebRootPath, "Templete", "ResetPassword.html");
            string content = File.ReadAllText(path);
            string finalContent = content.Replace("resetPassword", callback);
            await mailServices.SendEmailAsync(mailTo: user.Email, subject: "DevJob", null, callback);

           
                return new PasswordDto()
                {
                    Success=true,
                };
        }
        public async Task<PasswordDto> ResetPassword(ResetPasswordDTO resetPasswordDTO)
        {
            //find user
            var user = await userManager.FindByEmailAsync(resetPasswordDTO.Email);
            if (user==null || !await userManager.IsEmailConfirmedAsync(user))
            {
                throw new UnauthorizedAccessException("Email not confirmed");
            }
            var decodedToken = WebEncoders.Base64UrlDecode(resetPasswordDTO.Token);
            var tokn = Encoding.UTF8.GetString(decodedToken);
            var result = await userManager.ResetPasswordAsync(user, tokn, resetPasswordDTO.Password);
            if (result.Succeeded)
                return new PasswordDto() { Success = true };
           throw new InvalidOperationException(string.Join(',', result.Errors.Select(x => x.Description)));
        }
        public async Task<CompanyAuthResponse> CompanyRegister(CompanyRegisterDTO companyRegisterDTO)
        {
            ApplicationUser companyProfile = mapper.Map<ApplicationUser>(companyRegisterDTO);
            await unitOfWork.BeginTransaction();
            try
            {
                var response = await userManager.CreateAsync(companyProfile, companyRegisterDTO.Password);
                if (response.Succeeded)
                {

                    await userManager.AddToRoleAsync(companyProfile, "COMPANY");
                    //get company profile
                    CompanyProfile company = mapper.Map<CompanyProfile>(companyRegisterDTO);
                    company.ApplicationUser = companyProfile.Id;
                   
                    await unitOfWork.CompanyProfile.AddAsync(company);
                    await unitOfWork.SaveChangesAsync();
                    //send email
                    var confirmationToken = await userManager.GenerateEmailConfirmationTokenAsync(companyProfile);
                    var encodedToken = Uri.EscapeDataString(confirmationToken);
                    var baseUrl = configuration["AppUrl"];
                    var confirmationLink = $"{baseUrl}/api/Auth/confirm-email?email={companyProfile.Email}&token={encodedToken}";
                    await sendEmail("ConfirmEmail.html", "{ConfirmationLink}", confirmationLink, companyProfile.Email);

                    var jwtToken = await jwtServices.CreateTokenAsync(companyProfile);
                   await unitOfWork.CommitAsync();
                    return new CompanyAuthResponse()
                    {
                        Email = companyProfile.Email,
                        Success = true,
                        Token = jwtToken
                    };

                }

                else
                {
                    string errors = string.Join(", ", response.Errors.Select(x => x.Description));
                    throw new ArgumentException(errors);
                }
            }
            catch(Exception ex)
            {
               await unitOfWork.RollBackAsync();
                throw;
            }
        }
        public async Task<AuthResponse> ResendConfirmationEmail(string email)
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user == null)
                throw new KeyNotFoundException("Email Not found");
            var confirmationToken = await userManager.GenerateEmailConfirmationTokenAsync(user);
            var encodedToken = Uri.EscapeDataString(confirmationToken);
            var baseUrl = configuration["AppUrl"];
            var confirmationLink = $"{baseUrl}/api/Auth/confirm-email?email={user.Email}&token={encodedToken}";
            await sendEmail("ConfirmEmail.html", "{ConfirmationLink}", confirmationLink, user.Email);
            return new AuthResponse() { Success = true, Message = "Email send Successfully" };
        }
    }
}

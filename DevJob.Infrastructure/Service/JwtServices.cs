using DevJob.Application.DTOs.Auth;
using DevJob.Domain.Entities;
using DevJob.Application.ServiceContract;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
namespace DevJob.Infrastructure.Service
{
    public class JwtServices  : IJwtServices
    {
        private readonly IConfiguration configuration;
        private readonly UserManager<ApplicationUser> userManager;
        public JwtServices(IConfiguration configuration,UserManager<ApplicationUser> userManager)
        {
            this.configuration = configuration;
            this.userManager = userManager;
        }
        public async Task<string> CreateTokenAsync(ApplicationUser user)
        {
            DateTime expire = DateTime.UtcNow.AddMinutes(Convert.ToDouble(configuration["Jwt:Expire"]));
            var role =await userManager.GetRolesAsync(user);
            var roleClaim = role.Select(x => new Claim(ClaimTypes.Role, x)).ToList();
            var claim = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim (JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
               new Claim(JwtRegisteredClaimNames.Iat,
    new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString(),
    ClaimValueTypes.Integer64),
                new Claim (ClaimTypes .Email,user.Email),
                new Claim (ClaimTypes.Name,user.Name)
            };
            claim.AddRange(roleClaim);
            Console.WriteLine(configuration["Jwt:Key"]);
            SymmetricSecurityKey symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]));
            SigningCredentials signingCredentials= new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);
            JwtSecurityToken tokenGenerator = new JwtSecurityToken(
     configuration["Jwt:Issuer"],
     configuration["Jwt:Audience"],
     claim,
     expires: expire,
     signingCredentials: signingCredentials
     );
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            string token = tokenHandler.WriteToken(tokenGenerator);
            return token;
        }
        public string GenerateRefreshToken()
        {
            Byte[] bytes = new Byte[64];
          var random=  RandomNumberGenerator.Create();
            random.GetBytes(bytes);
            return Convert.ToBase64String(bytes);
        }

        public ClaimsPrincipal? GenerateUserPrincipal(string? token)
        {
            var tokenValidator = new TokenValidationParameters()
            {
                ValidateIssuer = true,
                ValidIssuer = configuration["Jwt:Issuer"],

                ValidateAudience = true,
                ValidAudience = configuration["Jwt:Audience"],

                ValidateLifetime = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]))

            };
            JwtSecurityTokenHandler jwtSecurityTokenHandler = new JwtSecurityTokenHandler();

            ClaimsPrincipal principal = jwtSecurityTokenHandler.ValidateToken(token, tokenValidator, out SecurityToken securityToken);

            if (securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid token");
            }

            return principal;
        }
        public async Task< AuthResponse> GenerateNewAccessToken(TokenModel tokenModel)
        {
            if (tokenModel == null)
                return new AuthResponse() { Success = false, Message = "Invalid Request" };
            ClaimsPrincipal? claimsPrincipal = GenerateUserPrincipal(tokenModel.Token);
            if (claimsPrincipal == null)
                return new AuthResponse() { Message = "Invalid access token", Success = false };
            string? email = claimsPrincipal.FindFirstValue(ClaimTypes.Email);
            var user = await userManager.FindByEmailAsync(email);
            if (user == null || user.RefreshToken != tokenModel.RefreshToken || user.RefreshTokenExpirationDate <= DateTime.Now)
                return new AuthResponse() { Success = false, Message = "Invalid refresh token" };
            //create token
            string newJwtToken = await CreateTokenAsync(user);
            user.RefreshToken = GenerateRefreshToken();
            user.RefreshTokenExpirationDate = DateTime.Now.AddDays(7);
            await userManager.UpdateAsync(user);
            return new AuthResponse() { Success =true,RefreshToken=user.RefreshToken,Token= newJwtToken };
        }
    }
}

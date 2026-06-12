using AutoMapper;
using DevJob.Application.DTOs.Auth;
using DevJob.Application.ServiceContract;
using DevJob.Domain.Entities;
using DevJob.Infrastructure.Service;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Moq;
using System.Security.Cryptography.Xml;


namespace Devjob.Test.Services
{
    public class AuthServiceTest
    {
        private readonly Mock<UserManager<ApplicationUser>> userManagerMock;
        private readonly Mock<SignInManager<ApplicationUser>> signInManagerMock;
        private readonly Mock<IMapper> mapperMock;
        private readonly Mock<IJwtServices> jwtServicesMock;
        private readonly Mock<IUnitOfWork> unitOfWorkMock;
        private readonly Mock<IMailServices> mailServicesMock;
        private readonly Mock<IConfiguration> configurationMock;
        private readonly Mock<IWebHostEnvironment> webHostMock;
        private readonly AuthService authService;

        public AuthServiceTest()
        {
            var store = new Mock<IUserStore<ApplicationUser>>();
            userManagerMock = new Mock<UserManager<ApplicationUser>>(store.Object, 
    null,
    null,
    null,
    null,
    null,
    null,
    null,
    null);
            signInManagerMock = new Mock<SignInManager<ApplicationUser>>(userManagerMock.Object,
            Mock.Of<IHttpContextAccessor>(),
            Mock.Of<IUserClaimsPrincipalFactory<ApplicationUser>>(),
            null, null, null, null);
            mapperMock = new Mock<IMapper>();
            jwtServicesMock = new Mock<IJwtServices>();
            unitOfWorkMock = new Mock<IUnitOfWork>();
            mailServicesMock = new Mock<IMailServices>();
            configurationMock = new Mock<IConfiguration>();
            webHostMock = new Mock<IWebHostEnvironment>();

            authService = new AuthService(userManagerMock.Object, mapperMock.Object, signInManagerMock.Object, jwtServicesMock.Object,configurationMock.Object, unitOfWorkMock.Object, mailServicesMock.Object, webHostMock.Object
                );
        }
        [Fact]
        public async Task Login_UserNotFound_ThrowUnauthorized()
        {
            //Arrange
            var login_dto = new LoginDTO()
            {
                Email = "xyx@gmail.com",
                Password = "123##"
            };
            userManagerMock.Setup(x => x.FindByEmailAsync(login_dto.Email))
                .ReturnsAsync((ApplicationUser)null);
            //Act
            var act = async () => await authService.Login(login_dto);
            //Assert
            await act.Should().ThrowAsync<UnauthorizedAccessException>()
                .WithMessage("Invalid Email or Password");
        }
        [Fact]
        public async Task Login_Email_NotConfirmed()
        {
            //Arrange
            var login_dto = new LoginDTO()
            {
                Email = "xyx@gmail.com",
                Password = "123##"
            };
            ApplicationUser user = new ApplicationUser();
            userManagerMock.Setup(x => x.FindByEmailAsync(login_dto.Email))
                .ReturnsAsync(user);
            userManagerMock.Setup(x => x.CheckPasswordAsync(user, login_dto.Password))
                .ReturnsAsync(true);
            //Act
            var act = async () => await authService.Login(login_dto);
            //Assert
            await act.Should().ThrowAsync<UnauthorizedAccessException>()
                .WithMessage("Please confirm your email first.");
        }
        [Fact]
        public async Task Login_Success()
        {
            //Arrange 
            var login_dto = new LoginDTO()
            {
                Email = "xyx@gmail.com",
                Password = "123##"
            };
            ApplicationUser user = new ApplicationUser();
            userManagerMock.Setup(x => x.FindByEmailAsync(login_dto.Email))
                .ReturnsAsync(user);
            userManagerMock.Setup(x => x.CheckPasswordAsync(user, login_dto.Password))
                .ReturnsAsync(true);
            userManagerMock.Setup(x => x.IsEmailConfirmedAsync(user))
                .ReturnsAsync(true);
            jwtServicesMock.Setup(x => x.CreateTokenAsync(user))
                .ReturnsAsync("fake-jwt-token");
            configurationMock.Setup(x => x["RefreshToken:Expire"])
                .Returns("60");
            jwtServicesMock.Setup(x => x.GenerateRefreshToken())
                .Returns("fake-refresh-token");
            userManagerMock.Setup(x => x.UpdateAsync(user))
                .ReturnsAsync(IdentityResult.Success);
            // Act
            var result = await authService.Login(login_dto);

            // Assert
            result.Success.Should().BeTrue();
            result.Token.Should().Be("fake-jwt-token");
        }
        [Fact]
        public async Task ForgetPassword_UserNotFound_ReturnsFalse()
        {
            // Arrange
            var dto = new ForgetPasswordDTO { Email = "notfound@test.com" };
            userManagerMock.Setup(x => x.FindByEmailAsync(dto.Email))
                .ReturnsAsync((ApplicationUser)null);

            // Act
            var result = await authService.ForgetPassword(dto);

            // Assert
            result.Success.Should().BeFalse();
            result.Message.Should().Be("User not Found");
        }
        [Fact]
        public async Task ConfirmEmail_InvalidEmail_ReturnsFalse()
        {
            // Arrange
            userManagerMock.Setup(x => x.FindByEmailAsync("wrong@test.com"))
                .ReturnsAsync((ApplicationUser)null);

            // Act
            var result = await authService.ConfirmEmail("wrong@test.com", "token");

            // Assert
            result.Success.Should().BeFalse();
            result.Message.Should().Be("Invalid Email");
        }

    }
}

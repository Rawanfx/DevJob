using DevJob.Application.DTOs;
using DevJob.Application.DTOs.Company;
using DevJob.Application.DTOs.Cvs;
using DevJob.Application.ServiceContract;
using DevJob.Domain.Entities;
using DevJob.Infrastructure.Service;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Moq;
using System.Linq.Expressions;
using Xunit;

public class CompanyServicesTests
{
    private readonly Mock<IUnitOfWork> unitOfWorkMock;
    private readonly Mock<UserManager<ApplicationUser>> userManagerMock;
    private readonly Mock<IUploadToAzure> uploadToAzureMock;
    private readonly CompanyServices companyServices;

    public CompanyServicesTests()
    {
        unitOfWorkMock = new Mock<IUnitOfWork>();
        uploadToAzureMock = new Mock<IUploadToAzure>();

        var store = new Mock<IUserStore<ApplicationUser>>();
        userManagerMock = new Mock<UserManager<ApplicationUser>>(
            store.Object, null, null, null, null, null, null, null, null);

        companyServices = new CompanyServices(
            unitOfWorkMock.Object,
            userManagerMock.Object,
            uploadToAzureMock.Object
        );
    }

    // ✅ Test 1: GetCompany - User Not Found
    [Fact]
    public async Task GetCompany_UserNotFound_ReturnsEmptyDto()
    {
        // Arrange
        userManagerMock.Setup(x => x.FindByIdAsync("invalid-id"))
            .ReturnsAsync((ApplicationUser)null);

        // Act
        var result = await companyServices.GetCompany("invalid-id");

        // Assert
        result.Should().NotBeNull();
        result.Email.Should().BeNull();
    }

    // ✅ Test 2: GetCompany - Success
    [Fact]
    public async Task GetCompany_ValidId_ReturnsCompanyData()
    {
        // Arrange
        var user = new ApplicationUser
        {
            Id = "valid-id",
            Email = "company@test.com",
            Name = "Test Company"
        };

        var profile = new CompanyProfile
        {
            Location = "Cairo",
            Description = "Test",
            Website = "www.test.com"
        };

        userManagerMock.Setup(x => x.FindByIdAsync("valid-id"))
            .ReturnsAsync(user);

        unitOfWorkMock.Setup(x => x.CompanyProfile
            .FirstOrDefaultAsync(It.IsAny<Expression<Func<CompanyProfile, bool>>>()))
            .ReturnsAsync(profile);

        // Act
        var result = await companyServices.GetCompany("valid-id");

        // Assert
        result.Email.Should().Be("company@test.com");
        result.Name.Should().Be("Test Company");
        result.Location.Should().Be("Cairo");
    }

    // ✅ Test 3: CompanyId - Company Not Found
    [Fact]
    public async Task CompanyId_CompanyNotFound_ReturnsNull()
    {
        // Arrange
        unitOfWorkMock.Setup(x => x.CompanyProfile
            .FirstOrDefaultAsync(It.IsAny<Expression<Func<CompanyProfile, bool>>>()))
            .ReturnsAsync((CompanyProfile)null);

        // Act
        var result = await companyServices.CompanyId("invalid-user-id");

        // Assert
        result.Should().BeNull();
    }

    // ✅ Test 4: CompanyId - Success
    [Fact]
    public async Task CompanyId_ValidUser_ReturnsId()
    {
        // Arrange
        unitOfWorkMock.Setup(x => x.CompanyProfile
            .FirstOrDefaultAsync(It.IsAny<Expression<Func<CompanyProfile, bool>>>()))
            .ReturnsAsync(new CompanyProfile { Id = 5 });

        // Act
        var result = await companyServices.CompanyId("valid-user-id");

        // Assert
        result.Should().Be(5);
    }

    // ✅ Test 5: ApplicantSearch - Company Not Found
    [Fact]
    public async Task ApplicantSearch_CompanyNotFound_ReturnsFalse()
    {
        // Arrange
        unitOfWorkMock.Setup(x => x.CompanyProfile
            .FirstOrDefaultAsync(It.IsAny<Expression<Func<CompanyProfile, bool>>>()))
            .ReturnsAsync((CompanyProfile)null);

        // Act
        var result = await companyServices.ApplicantSearch("invalid-id", "search", 1);

        // Assert
        result.Success.Should().BeFalse();
        result.Message.Should().Be("Company Not Found");
    }

    // ✅ Test 6: ApplicantSearch - Job Not Found
    [Fact]
    public async Task ApplicantSearch_JobNotFound_ReturnsFalse()
    {
        // Arrange
        unitOfWorkMock.Setup(x => x.CompanyProfile
            .FirstOrDefaultAsync(It.IsAny<Expression<Func<CompanyProfile, bool>>>()))
            .ReturnsAsync(new CompanyProfile { Id = 1 });

        unitOfWorkMock.Setup(x => x.Jobs
            .FirstOrDefaultAsync(It.IsAny<Expression<Func<Job, bool>>>()))
            .ReturnsAsync((Job)null);

        // Act
        var result = await companyServices.ApplicantSearch("company-id", "search", 1);

        // Assert
        result.Success.Should().BeFalse();
        result.Message.Should().Be("Job Not Found");
    }

    // ✅ Test 7: Update - User Not Found
    [Fact]
    public async Task Update_UserNotFound_ReturnsFalse()
    {
        // Arrange
        var dto = new UpdateCompanyProfileDTO { Id = "invalid-id" };
        var logo = new UploadPictureDTO();

        userManagerMock.Setup(x => x.FindByIdAsync("invalid-id"))
            .ReturnsAsync((ApplicationUser)null);

        // Act
        var result = await companyServices.Update(dto, logo);

        // Assert
        result.Success.Should().BeFalse();
        result.Message.Should().Be("Invalid Id");
    }

    // ✅ Test 8: Update - Profile Not Found
    [Fact]
    public async Task Update_ProfileNotFound_ReturnsFalse()
    {
        // Arrange
        var dto = new UpdateCompanyProfileDTO { Id = "valid-id" };
        var logo = new UploadPictureDTO();

        userManagerMock.Setup(x => x.FindByIdAsync("valid-id"))
            .ReturnsAsync(new ApplicationUser { Id = "valid-id" });

        unitOfWorkMock.Setup(x => x.CompanyProfile
            .FirstOrDefaultAsync(It.IsAny<Expression<Func<CompanyProfile, bool>>>()))
            .ReturnsAsync((CompanyProfile)null);

        // Act
        var result = await companyServices.Update(dto, logo);

        // Assert
        result.Success.Should().BeFalse();
        result.Message.Should().Be("Invalid Id");
    }

    // ✅ Test 9: Update - Success
    [Fact]
    public async Task Update_ValidData_ReturnsSuccess()
    {
        // Arrange
        var dto = new UpdateCompanyProfileDTO
        {
            Id = "valid-id",
            Name = "New Name",
            Location = "New Location",
            Description = "New Description",
            Website = "www.new.com"
        };
        var logo = new UploadPictureDTO();

        userManagerMock.Setup(x => x.FindByIdAsync("valid-id"))
            .ReturnsAsync(new ApplicationUser { Id = "valid-id" });

        unitOfWorkMock.Setup(x => x.CompanyProfile
            .FirstOrDefaultAsync(It.IsAny<Expression<Func<CompanyProfile, bool>>>()))
            .ReturnsAsync(new CompanyProfile { Id = 1 });

        uploadToAzureMock.Setup(x => x.UploadFileToAzure(It.IsAny<IFormFile>(), It.IsAny<string>()))
            .ReturnsAsync(new UploadToAzureResult { Link = "www.logo.com" });

        userManagerMock.Setup(x => x.UpdateAsync(It.IsAny<ApplicationUser>()))
            .ReturnsAsync(IdentityResult.Success);

        unitOfWorkMock.Setup(x => x.SaveChangesAsync())
            .ReturnsAsync(1);

        // Act
        var result = await companyServices.Update(dto, logo);

        // Assert
        result.Success.Should().BeTrue();
        result.Message.Should().Be("Company profile Update Succesfully");
    }
}
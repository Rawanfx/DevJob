using Castle.Core.Configuration;
using DevJob.Application.ServiceContract;
using DevJob.Domain.Entities;
using DevJob.Infrastructure.Service;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Devjob.Test.Services
{
    public class CvServiceUnitTest
    {
        private readonly Mock<IHttpClientFactory> httpClientFactoryMock;
        private readonly Mock<IUnitOfWork> unitOfWorkMock;
        private readonly Mock<IUploadToAzure> uploadToAzureMock;
        private readonly Mock<IConfiguration> configurationMock;
        private readonly Mock<ILogger<CvServices>> loggerMock;
        private readonly Mock<UserManager<ApplicationUser>> userManagerMock;
        private readonly CvServices cvServices;

        public CvServiceUnitTest()
        {
            httpClientFactoryMock = new Mock<IHttpClientFactory>();
            unitOfWorkMock = new Mock<IUnitOfWork>();
            uploadToAzureMock = new Mock<IUploadToAzure>();
            configurationMock = new Mock<IConfiguration>();
            loggerMock = new Mock<ILogger<CvServices>>();
            var store = new Mock<IUserStore<ApplicationUser>>();
            userManagerMock = new Mock<UserManager<ApplicationUser>>();
        }
    }
}

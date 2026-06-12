
using DevJob.Application.DTOs.Chat;
using DevJob.Application.DTOs.Company;
using DevJob.Application.ServiceContract;
using DevJob.Domain.Entities;
using DevJob.Domain.Enums;
using DevJob.Infrastructure.Hubs;
using DevJob.Infrastructure.Service;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Moq;
using System.Linq.Expressions;

namespace Devjob.Test.Services
{
    public class ChatServiceTest
    {
        private Mock<INotificationService> notificationServiceMock;
        private Mock<IHubContext<MessageHub>> httpContextMock;
        private Mock<IUnitOfWork> unitOfWork;
        private ChatServices chatServices;
        public ChatServiceTest()
        {
            notificationServiceMock = new Mock<INotificationService>();
            httpContextMock = new ();
            unitOfWork = new Mock<IUnitOfWork>();
            var mockClients = new Mock<IHubClients>();
            var mockClientProxy = new Mock<IClientProxy>();
            httpContextMock.Setup(x => x.Clients).Returns(mockClients.Object);
            mockClients.Setup(x => x.User(It.IsAny<string>())).Returns(mockClientProxy.Object);

            chatServices = new ChatServices(httpContextMock.Object, notificationServiceMock.Object, unitOfWork.Object);
        }

        [Fact]
        public async Task BeginConversation_CompanyNotFound_ReturnsFalse()
        {
            // Arrange
            var dto = new BeginConversationDto
            {
                userId = 1,
                jobId = 1,
                companyId = 1
            };

            unitOfWork.Setup(x => x.CompanyProfile
                .FirstOrDefaultAsync(It.IsAny<Expression<Func<CompanyProfile, bool>>>()))
                .ReturnsAsync((CompanyProfile)null);

            // Act
            var result = await chatServices.BedinConversation(dto, "company-user-id");

            // Assert
            result.Succes.Should().BeFalse();
            result.Message.Should().Be("Company not found");
        }
        [Fact]
        public async Task BeginConversation_UserNotInInterview_ReturnsFalse()
        {
            // Arrange
            var dto = new BeginConversationDto
            {
                userId = 1,
                jobId = 1,
                companyId = 1
            };

            unitOfWork.Setup(x => x.CompanyProfile
                .FirstOrDefaultAsync(It.IsAny<Expression<Func<CompanyProfile, bool>>>()))
                .ReturnsAsync(new CompanyProfile { Id = 1 });

            unitOfWork.Setup(x => x.UserCvData
                .FirstOrDefaultAsync(It.IsAny<Expression<Func<UserCvData, bool>>>()))
                .ReturnsAsync(new UserCvData { Id = 1 });

            unitOfWork.Setup(x => x.Jobs
                .FirstOrDefaultAsync(It.IsAny<Expression<Func<Job, bool>>>()))
                .ReturnsAsync(new Job { Id = 1 });

            unitOfWork.Setup(x => x.UserJob
                .FirstOrDefaultAsync(It.IsAny<Expression<Func<UserJob, bool>>>()))
                .ReturnsAsync((UserJob)null);

            // Act
            var result = await chatServices.BedinConversation(dto, "company-user-id");

            // Assert
            result.Succes.Should().BeFalse();
            result.Message.Should().Be("user hasn't applied");
        }
        [Fact]
        public async Task BeginConversation_AlreadyExstis()
        {
            //Arrange 
            BeginConversationDto dto = new BeginConversationDto()
            {
                companyId = 1,
                jobId = 1,
                userId = 1
            };
            CompanyProfile companyProfile = new CompanyProfile();
            UserCvData user = new UserCvData();
            Job job = new Job();


            unitOfWork.Setup(x => x.CompanyProfile
            .FirstOrDefaultAsync(It.IsAny<Expression<Func<CompanyProfile, bool>>>()))
                .ReturnsAsync(companyProfile);

            unitOfWork.Setup(x => x.UserCvData.FirstOrDefaultAsync(It.IsAny<Expression<Func<UserCvData, bool>>>()))
                .ReturnsAsync(user);

            unitOfWork.Setup(x => x.Jobs.FirstOrDefaultAsync(It.IsAny<Expression<Func<Job, bool>>>()))
                .ReturnsAsync(job);

            unitOfWork.Setup(x => x.UserJob
           .FirstOrDefaultAsync(It.IsAny<Expression<Func<UserJob, bool>>>()))
           .ReturnsAsync(new UserJob { Status = Status.Interview });

            unitOfWork.Setup(x => x.Conversations
                .FirstOrDefaultAsync(It.IsAny<Expression<Func<Conversation, bool>>>()))
                .ReturnsAsync(new Conversation { Id = 5 });
            // Act
            var result = await chatServices.BedinConversation(dto, "company-user-id");

            // Assert
            result.Succes.Should().BeTrue();
            result.ConversationId.Should().Be(5);
            result.Message.Should().Be("conversation has started");
        }
        [Fact]
        public async Task SendMessage_conversationNotFound()
        {
            SendMessageDto dto = new SendMessageDto()
            {
                conversationId = 1,
                Message = "Hello world"
            };
            string user = "fake-user id";

            unitOfWork.Setup(x => x.Conversations.GetConversationWithCompanyProfileAndDeveloper(dto.conversationId))
                .ReturnsAsync(  (Conversation)(null));
            //act
          var result=await  chatServices.SendMessage(dto, user);

            result.Success.Should().BeFalse();
            result.Message.Should().Be("Conversation not found");
        }
        [Fact]
        public async Task SendMessage_UnauthorizedUser_ReturnsFalse()
        {
            // Arrange
            var dto = new SendMessageDto
            {
                Message = "Hello",
                conversationId = 1
            };

            var conversation = new Conversation
            {
                Id = 1,
                UserCvData1 = new UserCvData { UserId = "developer-id" },
                CompanyProfile1 = new CompanyProfile { ApplicationUser = "company-id" }
            };

            unitOfWork.Setup(x => x.Conversations
                .GetConversationWithCompanyProfileAndDeveloper(dto.conversationId))
                .ReturnsAsync(conversation);

            // Act
            var result = await chatServices.SendMessage(dto, "unauthorized-user-id");

            // Assert
            result.Success.Should().BeFalse();
            result.Message.Should().Be("You are not authorized to view this chat");
        }

        [Fact]
        public async Task UpdateMessage_TimeLimitExceeded_ReturnsFalse()
        {
            // Arrange
            var dto = new UpdateMessageDto
            {
                messageId = 1,
                conversationId = 1,
                newMessage = "Updated"
            };

            var conversation = new Conversation
            {
                Id = 1,
                UserCvData1 = new UserCvData { UserId = "user-id" },
                CompanyProfile1 = new CompanyProfile { ApplicationUser = "company-id" }
            };

            var oldMessage = new chats
            {
                Id = 1,
                SenderId = "user-id",
                date = DateTime.UtcNow.AddMinutes(-20), 
                IsDelete = false
            };

            unitOfWork.Setup(x => x.Conversations
                .GetConversationWithCompanyProfileAndDeveloper(dto.conversationId))
                .ReturnsAsync(conversation);

            unitOfWork.Setup(x => x.Chats
                .FirstOrDefaultAsync(It.IsAny<Expression<Func<chats, bool>>>()))
                .ReturnsAsync(oldMessage);

            // Act
            var result = await chatServices.UpdateMessage(dto, "user-id");

            // Assert
            result.Success.Should().BeFalse();
            result.Message.Should().Be("Time limit exceeded (15 mins)");
        }
        [Fact]
        public async Task DeleteMessage_MessageNotFound_ReturnsFalse()
        {
            // Arrange
            var dto = new DeleteMessageDto
            {
                messageId = 1,
                conversationId = 1
            };

            unitOfWork.Setup(x => x.Chats
                .FirstOrDefaultAsync(It.IsAny<Expression<Func<chats, bool>>>()))
                .ReturnsAsync((chats)null);

            // Act
            var result = await chatServices.DeleteMessage("user-id", dto);

            // Assert
            result.Success.Should().BeFalse();
            result.Message.Should().Be("Message Not Found");
        }
    }
}

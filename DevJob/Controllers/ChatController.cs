using DevJob.Application.DTOs.Chat;
using DevJob.Application.DTOs.Company;
using DevJob.Application.ServiceContract;
using DevJob.Infrastructure.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using System.Security.Claims;

namespace DevJob.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly IChatServices chatServices;
        public ChatController(IChatServices chatServices)
        {
            this.chatServices = chatServices;
        }
        [Authorize(Roles = "Company")]
        [HttpPost("start-conversation")]
        public async Task<IActionResult> StartConversation(BeginConversationDto beginConversationDto)
        {
            var company = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await chatServices.BedinConversation(beginConversationDto, company);
            if (result.Succes)
                return Ok(result);
            return BadRequest(result);
        }
        [Authorize]
        [HttpGet("all-chats")]
        public async Task<IActionResult> getAllChat()
        {
            var user = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await chatServices.displayAllConversationResult(user);
            if (result.Success)
                return Ok(result);
            return BadRequest(result);
        }
        [Authorize]
        [HttpGet("load-chat")]
        public async Task<IActionResult> LoadChat(int conversationId)
        {
            var user = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result =await  chatServices.LoadChat(user, conversationId);
            if (result.Success)
                return Ok(result);
            return BadRequest(result);
        }
        [Authorize]
        [HttpPost("send-message")]
        public async Task<IActionResult> SendMessage(SendMessageDto sendMessageDto)
        {
            var user = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await chatServices.SendMessage(sendMessageDto, user);
            if (result.Success)
                return Ok(result);
            return BadRequest(result);
        }
        [Authorize]
        [HttpDelete]
        public async Task<IActionResult> DeleteMessage (DeleteMessageDto deleteMessageDto)
        {
            var user = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await chatServices.DeleteMessage(user, deleteMessageDto);
            if (result.Success)
                return Ok(result);
            return BadRequest(result);
        }
        [Authorize]
        [HttpPut("update-message")]
        public async Task< IActionResult>UpdateMessage(UpdateMessageDto updateMessageDto)
        {
            var user = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await chatServices.UpdateMessage(updateMessageDto,user);
            if (result.Success)
                return Ok(result);
            return BadRequest(result);
        }
        [Authorize]
        [HttpGet("search")]
        public async Task<IActionResult> ConversationSearch(string item)
        {
            var user = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await chatServices.ConversationSearch(item,user);
            if (result.Success)
                return Ok(result);
            return BadRequest(result);
        }
    }
}

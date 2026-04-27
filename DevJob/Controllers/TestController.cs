using DevJob.Application.ServiceContract;
using Microsoft.AspNetCore.Mvc;
namespace DevJob.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly INotificationService notificationService;
        public TestController(INotificationService notificationService)
        {
            this.notificationService = notificationService;
        }
       
    }
}

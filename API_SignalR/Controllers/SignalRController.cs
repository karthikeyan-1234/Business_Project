using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

using Services;

namespace API_SignalR.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SignalRController : ControllerBase
    {
        private readonly IHubContext<InfoHub, IInfoHub> hubContext;

        public SignalRController(IHubContext<InfoHub, IInfoHub> hubContext)
        {
            this.hubContext = hubContext;
        }

        [HttpPost("NewCall")]
        public IActionResult NewCall()
        {
            hubContext.Clients.All.SendMessage("Test");

            return Ok();
        }

    }
}

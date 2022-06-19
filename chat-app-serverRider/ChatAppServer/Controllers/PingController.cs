using System.Linq;
using System.Threading.Tasks;
using ChatAppServer.DataAccess;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChatAppServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PingController : ControllerBase
    {
        private readonly ChatDbContext _chatDbContext;

        public PingController(ChatDbContext chatDbContext)
        {
            _chatDbContext = chatDbContext;
        }
        
        [HttpGet]
        public async Task<ActionResult> Get()
        {
            var firstUser = await _chatDbContext.Users.FirstAsync();
            var response = new { message = "GET pong!!!", firstUser = firstUser };
            return Ok(response);
        }

        [HttpPost]
        public Task<ActionResult> Post([FromBody] object data)
        {
            var response = new { message = "POST pong!!!" };
            return Task.FromResult<ActionResult>(Ok(response));
        }
    }
}
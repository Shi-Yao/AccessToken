using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace AcessToken.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        // GET: api/<TestController>
        [HttpGet("getVaule")]
        [CustomAuthorize]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

      
    }
}

using Microsoft.AspNetCore.Mvc;

namespace Notes.Website.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DirController : ControllerBase
    {
        [HttpGet("{*path}")]
        public IActionResult Get(string path)
        {
            return Ok(path);
        }

        [HttpPut("{*path}")]
        public IActionResult Put(string path)
        {
            return Ok(path);
        }
    }
}

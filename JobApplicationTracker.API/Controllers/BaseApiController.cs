using Microsoft.AspNetCore.Mvc;

namespace JobApplicationTracker.API.Controllers
{
    /// <summary>
    /// Base controller for API endpoints
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public abstract class BaseApiController : ControllerBase
    {
    }
}
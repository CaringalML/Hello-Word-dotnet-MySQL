using HelloWorldApi.Models;
using HelloWorldApi.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace HelloWorldApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HelloController : ControllerBase
    {
        private readonly HelloRepository _repository;
        private readonly ILogger<HelloController> _logger;

        public HelloController(HelloRepository repository, ILogger<HelloController> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        [HttpPost]
        public async Task<ActionResult<HelloMessage>> Post([FromBody] HelloMessage request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.Name))
                {
                    request.Name = "Anonymous";
                }

                // Customize the message based on the name
                request.Message = $"Hello, {request.Name}! Welcome to .NET 6 API with MySQL.";
                
                // Save to database
                await _repository.SaveMessageAsync(request);
                
                _logger.LogInformation($"Created hello message for {request.Name}");
                
                return CreatedAtAction(nameof(Get), new { id = request.Id }, request);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating hello message");
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<HelloMessage>> Get(int id)
        {
            try
            {
                var message = await _repository.GetMessageAsync(id);
                
                if (message == null)
                {
                    return NotFound();
                }
                
                return Ok(message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving hello message with ID {id}");
                return StatusCode(500, "An error occurred while processing your request");
            }
        }
    }
}
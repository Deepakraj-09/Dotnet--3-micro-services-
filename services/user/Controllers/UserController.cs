using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using UserService.Models;
using UserService.Services;

namespace UserService.Controllers
{
    [ApiController]
    [Route("")]
    public class UserController : ControllerBase
    {
        private readonly MongoDbService _mongoService;
        private readonly RedisService _redisService;
        private readonly ILogger<UserController> _logger;

        public UserController(MongoDbService mongoService, RedisService redisService, ILogger<UserController> logger)
        {
            _mongoService = mongoService;
            _redisService = redisService;
            _logger = logger;
        }

        [HttpGet("health")]
        public HealthStatus Health()
        {
            return new HealthStatus { App = "OK", Mongo = _mongoService.IsConnected };
        }

        [HttpGet("uniqueid")]
        public async Task<IActionResult> GetUniqueId()
        {
            try
            {
                var counter = await _redisService.IncrementCounterAsync("anonymous-counter");
                return Ok(new UniqueIdResponse { Uuid = $"anonymous-{counter}" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ERROR");
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("check/{id}")]
        public async Task<IActionResult> CheckUser(string id)
        {
            if (!_mongoService.IsConnected)
            {
                _logger.LogError("database not available");
                return StatusCode(500, "database not available");
            }

            try
            {
                var collection = _mongoService.GetUsersCollection();
                var user = await collection.Find(u => u.Name == id).FirstOrDefaultAsync();
                return user != null ? Ok("OK") : NotFound("user not found");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ERROR");
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("users")]
        public async Task<IActionResult> GetAllUsers()
        {
            if (!_mongoService.IsConnected)
            {
                _logger.LogError("database not available");
                return StatusCode(500, "database not available");
            }

            try
            {
                var collection = _mongoService.GetUsersCollection();
                var users = await collection.Find(_ => true).ToListAsync();
                return Ok(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ERROR");
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            _logger.LogInformation("login {Request}", request);

            if (string.IsNullOrEmpty(request.Name) || string.IsNullOrEmpty(request.Password))
            {
                _logger.LogWarning("credentials not complete");
                return BadRequest("name or password not supplied");
            }

            if (!_mongoService.IsConnected)
            {
                _logger.LogError("database not available");
                return StatusCode(500, "database not available");
            }

            try
            {
                var collection = _mongoService.GetUsersCollection();
                var user = await collection.Find(u => u.Name == request.Name).FirstOrDefaultAsync();
                _logger.LogInformation("user {User}", user);

                if (user == null)
                {
                    return NotFound("name not found");
                }

                if (user.Password != request.Password)
                {
                    return NotFound("incorrect password");
                }

                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ERROR");
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            _logger.LogInformation("register {Request}", request);

            if (string.IsNullOrEmpty(request.Name) || string.IsNullOrEmpty(request.Password) || string.IsNullOrEmpty(request.Email))
            {
                _logger.LogWarning("insufficient data");
                return BadRequest("insufficient data");
            }

            if (!_mongoService.IsConnected)
            {
                _logger.LogError("database not available");
                return StatusCode(500, "database not available");
            }

            try
            {
                var collection = _mongoService.GetUsersCollection();
                var existingUser = await collection.Find(u => u.Name == request.Name).FirstOrDefaultAsync();

                if (existingUser != null)
                {
                    _logger.LogWarning("user already exists");
                    return BadRequest("name already exists");
                }

                var newUser = new User
                {
                    Name = request.Name,
                    Password = request.Password,
                    Email = request.Email
                };

                await collection.InsertOneAsync(newUser);
                _logger.LogInformation("inserted {User}", newUser);
                return Ok("OK");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ERROR");
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("order/{id}")]
        public async Task<IActionResult> PlaceOrder(string id, [FromBody] object orderData)
        {
            _logger.LogInformation("order {OrderData}", orderData);

            if (!_mongoService.IsConnected)
            {
                _logger.LogError("database not available");
                return StatusCode(500, "database not available");
            }

            try
            {
                var usersCollection = _mongoService.GetUsersCollection();
                var user = await usersCollection.Find(u => u.Name == id).FirstOrDefaultAsync();

                if (user == null)
                {
                    return NotFound("user not found");
                }

                var ordersCollection = _mongoService.GetOrdersCollection();
                var existingOrder = await ordersCollection.Find(o => o.Name == id).FirstOrDefaultAsync();

                if (existingOrder != null)
                {
                    existingOrder.History.Add(orderData);
                    await ordersCollection.ReplaceOneAsync(o => o.Name == id, existingOrder);
                }
                else
                {
                    var newOrder = new Order
                    {
                        Name = id,
                        History = new List<object> { orderData }
                    };
                    await ordersCollection.InsertOneAsync(newOrder);
                }

                _logger.LogInformation("order placed for {Id}", id);
                return Ok("OK");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ERROR");
                return StatusCode(500, ex.Message);
            }
        }
    }
}

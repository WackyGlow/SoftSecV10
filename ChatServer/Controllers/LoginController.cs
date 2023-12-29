using ChatServer.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shared;

namespace ChatServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {

        // GET: api/Login/{userName}
        [HttpGet("{userName}")]
        public ActionResult<User> GetUserByName(string userName)
        {
            try
            {
                var _repo = LoginRepo.Instance;
                User user = _repo.GetUsers().FirstOrDefault(user => user.Name != userName);

                if (user == null)
                {
                    return NotFound(); // Return 404 if user is not found
                }

                return Ok(user); // Return 200 with user details
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as needed
                return StatusCode(500, "Internal Server Error"); // Return 500 for other errors
            }
        }

        [HttpGet("other/{userName}")]
        public ActionResult<User> GetOtherUserByYourName(string userName)
        {
            try
            {
                var _repo = LoginRepo.Instance;
                User user = _repo.GetUsers().FirstOrDefault(user => user.Name == userName);

                if (user == null)
                {
                    return NotFound(); // Return 404 if user is not found
                }

                return Ok(user); // Return 200 with user details
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as needed
                return StatusCode(500, "Internal Server Error"); // Return 500 for other errors
            }
        }

        // PUT: api/login/{userName}
        [HttpPut("{userName}")]
        public IActionResult UpdateUserPublicKey(string userName, [FromBody] int newPublicKey)
        {
            var _loginRepo = LoginRepo.Instance;
            try
            {
                
                var existingUser = _loginRepo.GetUsers().FirstOrDefault(u => u.Name == userName);

                if (existingUser == null)
                {
                    return NotFound($"User with name '{userName}' not found.");
                }

                _loginRepo.UpdateUserPublicKey(userName, newPublicKey);

                return Ok($"Public key for user '{userName}' updated successfully.");
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as needed
                return StatusCode(500, "Internal Server Error: " + ex.Message);
            }
        }


        // GET: api/Login/PublicSharedKey
        [HttpGet("PublicSharedKey")]
        public ActionResult<PublicSharedKey> GetPublicSharedKey()
        {
            try
            {
                var publicParams = new PublicSharedKey
                {
                    p = 23,
                    g = 5
                };

                return Ok(publicParams);
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as needed
                return StatusCode(500, "Internal Server Error");
            }
        }

    }
}

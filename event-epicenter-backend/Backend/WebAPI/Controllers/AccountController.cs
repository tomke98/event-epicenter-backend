using Firebase.Auth;
using FirebaseAdmin.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service;
using System.ComponentModel.DataAnnotations;
using WebAPI.Authentication;

namespace WebAPI.Controllers
{
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly FirebaseAuthClient client;

        private readonly UserService userService;

        public AccountController()
        {
            userService = new UserService();
            client = new FirebaseAuthClient(FirebaseConfig.AuthConfig);
        }

        [Authorize(Roles = "user")]
        [Route("update-role/{userId}/{role}")]
        [HttpPut]
        public async Task<IActionResult> UpdateUserRoleAsync(string userId, string role)
        {
            try
            {
                var claims = new Dictionary<string, object>(){ { "role", role } };
                await FirebaseAuth.DefaultInstance.SetCustomUserClaimsAsync(userId, claims);
            }
            catch (FirebaseAdmin.Auth.FirebaseAuthException ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok("Role updated!");
        }

        [Route("/login")]
        [HttpPost]
        public async Task<IActionResult> LoginUserAsync([FromBody] LoginModel model)
        {
            try
            {
                var userCredentials = await client.SignInWithEmailAndPasswordAsync(model.Email, model.Password);

                if (userCredentials != null)
                {
                    UserRecord userRecord = await FirebaseAuth.DefaultInstance.GetUserAsync(userCredentials.User.Uid);

                    Model.User user = await userService.GetUserByIdAsync(userCredentials.User.Uid);

                    var result = new UserInfo()
                    {
                        AccessToken = userCredentials.User.Credential.IdToken,
                        ExpiresIn = userCredentials.User.Credential.ExpiresIn.ToString(),
                        UserId = userCredentials.User.Uid,
                        Role = userRecord.CustomClaims["role"].ToString(),
                        City = user?.City,
                        Dob = (DateTime)(user?.Dob),
                        FirstName = user?.FirstName,
                        LastName = user?.LastName,
                        EventTypeIds = user?.EventTypeIds
                    };

                    return Ok(result);
                }
            }
            catch (Firebase.Auth.FirebaseAuthException ex)
            {
                return BadRequest(FirebaseHelper.HandleFirebaseError(ex));
            }

            return BadRequest("Failed to login.");
        }

        [Route("/register")]
        [HttpPost]
        public async Task<IActionResult> RegisterUserAsync([FromBody] RegisterModel model)
        {
            try
            {
                var userCredentials = await client.CreateUserWithEmailAndPasswordAsync(model.Email, model.Password);

                if (userCredentials != null)
                {
                    var claims = new Dictionary<string, object>()
                    {
                        { "role", "user" },
                    };
                    await FirebaseAuth.DefaultInstance.SetCustomUserClaimsAsync(userCredentials.User.Uid, claims);

                    Model.User user = new Model.User(userCredentials.User.Uid, model.City, model.Dob, model.FirstName, model.LastName, model.EventTypeIds);

                    await userService.CreateUserAsync(user);

                    return Ok("User successfully registered.");
                }
            }
            catch (Firebase.Auth.FirebaseAuthException ex)
            {
                return BadRequest(FirebaseHelper.HandleFirebaseError(ex));
            }

            return BadRequest("Failed to login.");
        }

        [Authorize(Roles = "user, admin")]
        [Route("/update-password/{password}")]
        [HttpGet]
        public async Task<IActionResult> UpdatePasswordAsync(string password)
        {
            try
            {
                if(client.User != null)
                {
                    await client.User.ChangePasswordAsync(password);
                }
            }
            catch (Firebase.Auth.FirebaseAuthException ex)
            {
                return BadRequest(FirebaseHelper.HandleFirebaseError(ex));
            }

            return Ok("Password updated.");
        }

        [Authorize(Roles = "user, admin")]
        [Route("/reset-password/{email}")]
        [HttpGet]
        public async Task<IActionResult> ResetPasswordAsync(string email)
        {
            try
            {
                await client.ResetEmailPasswordAsync(email);
            }
            catch (Firebase.Auth.FirebaseAuthException ex)
            {
                return BadRequest(FirebaseHelper.HandleFirebaseError(ex));
            }

            return Ok("Password reset request sent.");
        }

        [Authorize(Roles = "user, admin")]
        [Route("/logout")]
        [HttpGet]
        public IActionResult Logout()
        {
            try
            {
                if(client.User == null) 
                {
                    return BadRequest("User not logged in.");
                }
                client.SignOut();
            }
            catch (Firebase.Auth.FirebaseAuthException ex)
            {
                return BadRequest(FirebaseHelper.HandleFirebaseError(ex));
            }

            return Ok("Successfully logged out.");
        }
    }

    public class UserCredentials
    {
        public string AccessToken { get; set; }

        public string ExpiresIn { get; set; }

        public string UserId { get; set; }

        public string Role {  get; set; }
    }

    public class UserInfo : UserCredentials
    {
        public string City { get; set; }

        public DateTime Dob { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public List<string> EventTypeIds { get; set; }

    }

    public class LoginModel
    {
        [Required]
        public string Password { get; set; }

        [Required]
        public string Email { get; set; }
    }

    public class RegisterModel
    {
        [Required]
        public string Password { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string City { get; set; }

        [Required]
        public DateTime Dob { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        public List<string> EventTypeIds { get; set; }
    }
}
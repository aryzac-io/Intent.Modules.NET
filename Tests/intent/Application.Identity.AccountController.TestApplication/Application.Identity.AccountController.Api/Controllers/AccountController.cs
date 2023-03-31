using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Application.Identity.AccountController.Application.Account;
using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.Identity.AccountController.AccountController", Version = "1.0")]

namespace Application.Identity.AccountController.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IUserStore<IdentityUser> _userStore;
        private readonly ILogger<AccountController> _logger;
        private readonly IAccountEmailSender _accountEmailSender;
        private readonly IConfiguration _configuration;

        public AccountController(
            SignInManager<IdentityUser> signInManager,
            IUserStore<IdentityUser> userStore,
            UserManager<IdentityUser> userManager,
            ILogger<AccountController> logger,
            IAccountEmailSender accountEmailSender,
            IConfiguration configuration)
        {
            _signInManager = signInManager;
            _userStore = userStore;
            _userManager = userManager;
            _logger = logger;
            _accountEmailSender = accountEmailSender;
            _configuration = configuration;
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterDto input)
        {
            if (string.IsNullOrWhiteSpace(input.Email))
            {
                ModelState.AddModelError<RegisterDto>(x => x.Email, "Mandatory");
            }

            if (string.IsNullOrWhiteSpace(input.Password))
            {
                ModelState.AddModelError<RegisterDto>(x => x.Password, "Mandatory");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = new IdentityUser();

            await _userStore.SetUserNameAsync(user, input.Email, CancellationToken.None);
            await _userManager.SetEmailAsync(user, input.Email);
            var result = await _userManager.CreateAsync(user, input.Password!);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                return BadRequest(ModelState);
            }

            _logger.LogInformation("User created a new account with password.");

            var userId = await _userManager.GetUserIdAsync(user);
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

            if (_userManager.Options.SignIn.RequireConfirmedAccount)
            {
                await _accountEmailSender.SendEmailConfirmationRequest(
                    email: input.Email!,
                    userId: userId,
                    code: code);
            }

            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginDto input)
        {
            if (string.IsNullOrWhiteSpace(input.Email))
            {
                ModelState.AddModelError<LoginDto>(x => x.Email, "Mandatory");
            }

            if (string.IsNullOrWhiteSpace(input.Password))
            {
                ModelState.AddModelError<LoginDto>(x => x.Password, "Mandatory");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var email = input.Email!;
            var password = input.Password!;

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null ||
                !await _userManager.CheckPasswordAsync(user, password))
            {
                _logger.LogWarning("Invalid login attempt.");
                return Forbid();
            }

            if (await _userManager.IsLockedOutAsync(user))
            {
                _logger.LogWarning("User account locked out.");
                return Forbid();
            }

            var token = GetJwtToken(
                username: email,
                signingKey: Convert.FromBase64String(_configuration.GetSection("JwtToken:SigningKey").Get<string>()!),
                issuer: _configuration.GetSection("JwtToken:Issuer").Get<string>()!,
                audience: _configuration.GetSection("JwtToken:Audience").Get<string>()!,
                expiration: TimeSpan.FromMinutes(120));

            _logger.LogInformation("User logged in.");
            return Ok(token);
        }

        private static string GetJwtToken(
            string username,
            byte[] signingKey,
            string issuer,
            string audience,
            TimeSpan expiration)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub,username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                expires: DateTime.UtcNow.Add(expiration),
                claims: claims,
                signingCredentials: new(
                    key: new SymmetricSecurityKey(signingKey),
                    algorithm: SecurityAlgorithms.HmacSha256));

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmEmail(ConfirmEmailDto input)
        {
            if (string.IsNullOrWhiteSpace(input.UserId))
            {
                ModelState.AddModelError<ConfirmEmailDto>(x => x.UserId, "Mandatory");
            }

            if (string.IsNullOrWhiteSpace(input.Code))
            {
                ModelState.AddModelError<ConfirmEmailDto>(x => x.Code, "Mandatory");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = input.UserId!;
            var code = input.Code!;
            var user = await _userManager.FindByIdAsync(input.UserId!);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{userId}'.");
            }

            code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));

            var result = await _userManager.ConfirmEmailAsync(user, code);
            if (!result.Succeeded)
            {
                ModelState.AddModelError<ConfirmEmailDto>(x => x, "Error confirming your email.");
                return BadRequest(ModelState);
            }

            return Ok();
        }
    }

    public class RegisterDto
    {
        public string? Email { get; set; }
        public string? Password { get; set; }
    }

    public class LoginDto
    {
        public string? Email { get; set; }
        public string? Password { get; set; }
    }

    public class ConfirmEmailDto
    {
        public string? UserId { get; set; }
        public string? Code { get; set; }
    }
}

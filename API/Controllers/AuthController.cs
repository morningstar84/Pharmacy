using System;
using System.Security.Principal;
using System.Threading.Tasks;
using AuthenticationService.API.Data;
using AuthenticationService.API.Dtos;
using Data.Models;
using Data.Models.Dtos;
using Domain.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using NLog;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly IAuthRepository _repository;
        private readonly IConfiguration _configuration;
        private readonly ITokenService _tokenService;

        public AuthController(IAuthRepository repo, IConfiguration configuration, ITokenService tokenService)
        {
            _tokenService = tokenService;
            _repository = repo;
            _configuration = configuration;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserRegisterDto userToRegister)
        { // [FromBody] is not necessary beacuse we are using [ApiController] => auto infer from body
            // validated request
            userToRegister.Username = userToRegister.Username.ToLower();
            if (!HasValidUsernameAndPassword(userToRegister))
            {
                _logger.Error("Invalid username and/or password -> (" + userToRegister.Username + ", " + userToRegister.Password + ") ");
                return BadRequest("Invalid username and/or password");
            }
            if (await _repository.UserExists(userToRegister.Username))
            {
                _logger.Error("Username is already taken -> (" + userToRegister.Username + ")");
                return BadRequest("Username is already taken");
            }
            var userToCreate = new User
            {
                Username = userToRegister.Username
            };
            var createdUser = await _repository.Register(userToCreate, userToRegister.Password);
            // Should be nice to return User created at route and then specify URL for new user
            var result = _tokenService.GenerateNewTokenForIdentity(createdUser.Username, createdUser.Id.ToString(), createdUser.UserRole);
            return StatusCode(201, new { user_id = createdUser.Id, token = result.Item1, expires = result.Item2 });
        }

        private bool HasValidUsernameAndPassword(UserRegisterDto dto)
        {
            return !String.IsNullOrWhiteSpace(dto.Username) && !String.IsNullOrWhiteSpace(dto.Password);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginUserDto userForLoginDto)
        {
            var userFromRepo = await _repository.Login(userForLoginDto.Username, userForLoginDto.Password);

            if (userFromRepo == null)
            {
                _logger.Info("Can't find any corrispondence in database for user " + userForLoginDto.Username);
                return Unauthorized();
            }
            var result = _tokenService.GenerateNewTokenForIdentity(userFromRepo.Username, userFromRepo.Id.ToString(), userFromRepo.UserRole);
            return Ok(new
            {
                user_id = userFromRepo.Id,
                token = result.Item1,
                expires = result.Item2
            });
        }

        [HttpPost("verify")]
        public async Task<IActionResult> Verify(TokenVerificationDto tokenDto)
        {
            var verificationResult = VerifyToken(tokenDto.Token);
            return Ok(new
            {
                validToken = verificationResult.Item1
            });
        }

        private Tuple<bool, IPrincipal> VerifyToken(string token)
        {
            IPrincipal principal = _tokenService.GetIdentityFromToken(token);
            return principal == null ? Tuple.Create(false, principal)! : Tuple.Create(principal.Identity!.IsAuthenticated, principal);
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh(TokenVerificationDto tokenDto)
        {
            var validation = IsValidTokenAndIdentity(tokenDto.Token);
            if (!validation.Item1)
            {
                return Unauthorized();
            }
            var userFromRepo = await _repository.GetUserByUsername(validation.Item2.Identity!.Name);
            var result = _tokenService.GenerateNewTokenForIdentity(userFromRepo.Username, userFromRepo.Id.ToString(), userFromRepo.UserRole);
            return Ok(new
            {
                token = result.Item1,
                expires = result.Item2
            });
        }

        private (bool, IPrincipal) IsValidTokenAndIdentity(string token)
        {
            var isValidToken = VerifyToken(token).Item1;
            var identity = _tokenService.GetIdentityFromToken(token);
            return (isValidToken && identity != null, identity)!;
        }
    }
}


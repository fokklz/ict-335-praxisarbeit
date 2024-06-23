using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SaveUpBackend.Common.Attributes;
using SaveUpBackend.Common.Generics;
using SaveUpBackend.Interfaces;
using SaveUpModels.DTOs.Requests;
using SaveUpModels.DTOs.Responses;
using SaveUpModels.Models;

namespace SaveUpBackend.Controllers
{
    public class UsersController : GenericController<User, UserResponse, UpdateUserRequest, CreateUserRequest>
    {
        private readonly IUserService _service;

        public UsersController(IUserService userService) : base(userService)
        {
            _service = userService;
        }

        /// <summary>
        /// Allow a user to get their own information based on the submitted token
        /// </summary>
        /// <returns>UserResponse</returns>
        [HttpGet("me")]
        [CustomAuthorize]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserResponse))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Me()
        {
            var userInfo = await _service.GetMe();
            return userInfo.IsSuccess ? Ok(userInfo.Result) : NotFound(userInfo.Exception);
        }

        /// <summary>
        /// Revoke the refresh token of the current user
        /// </summary>
        /// <returns>Empty Success Response</returns>
        [HttpPost("revoke")]
        [CustomAuthorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Revoke()
        {
            var result = await _service.RevokeRefreshToken();
            return result.IsSuccess ? Ok() : BadRequest(result.Exception);
        }

        /// <summary>
        /// Refresh the login by providing a valid refresh token and a valid access token
        /// </summary>
        /// <param name="model">RefreshRequest</param>
        /// <returns>LoginResponse</returns>
        [HttpPost("refresh")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(LoginResponse))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Refresh([FromBody] RefreshRequest model)
        {
            var result = await _service.Refresh(model);
            return result.IsSuccess ? Ok(result.Result) : Unauthorized(result.Exception);
        }


        /// <summary>
        /// Login a user and return a token along with their information
        /// </summary>
        /// <param name="model">LoginRequest</param>
        /// <returns>LoginResponse</returns>
        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(LoginResponse))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login([FromBody] LoginRequest model)
        {
            var result = await _service.LoginAsync(model);
            return result.IsSuccess ? Ok(result.Result) : Unauthorized(result.Exception);
        }

        /// <summary>
        /// Register a new user and return a token along with their information
        /// </summary>
        /// <param name="model">The RegisterRequest</param>
        /// <returns>The LoginResponse</returns>
        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(LoginResponse))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Register([FromBody] RegisterRequest model)
        {
            var result = await _service.RegisterAsync(model);
            return result.IsSuccess ? Ok(result.Result) : Unauthorized(result.Exception);
        }
    }
}

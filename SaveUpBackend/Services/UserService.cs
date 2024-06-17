using AutoMapper;
using MongoDB.Bson;
using MongoDB.Driver;
using SaveUpBackend.Common;
using SaveUpBackend.Common.Enums;
using SaveUpBackend.Common.Generics;
using SaveUpBackend.Interfaces;
using SaveUpModels.DTOs.Requests;
using SaveUpModels.DTOs.Responses;
using SaveUpModels.Models;
using System.Security.Claims;

namespace SaveUpBackend.Services
{
    public class UserService : GenericService<User, UserResponse, UpdateUserRequest, CreateUserRequest>, IUserService
    {
        private readonly IMongoDBContext _context;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAuthService _authService;
        private readonly ITokenService _tokenService;

        public UserService(IMongoDBContext context, IMapper mapper, IHttpContextAccessor httpContextAccessor, IAuthService authService, ITokenService tokenService) : base(context, mapper, httpContextAccessor)
        {
            _context = context;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _authService = authService;
            _tokenService = tokenService;
        }


        /// <summary>
        /// Allow users to request their own information based on the submitted token
        /// </summary>
        /// <returns>the user information</returns>
        public async Task<TaskResult<object>> GetMe()
        {
            var user = _httpContextAccessor.HttpContext?.User;
            if (user == null) return CreateTaskResult.Error<object>(ErrorKey.ENTRY_NOT_FOUND);

            var claim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (claim == null) return CreateTaskResult.Error<object>(ErrorKey.ENTRY_NOT_FOUND);
            
            return await GetAsync(claim);
        }

        /// <summary>
        /// Login a User with the given Credentials
        /// </summary>
        /// <param name="model">The Credentials of the user</param>
        /// <returns>a login Response with all information of the loggedin user as well as a token</returns>
        public async Task<TaskResult<LoginResponse>> LoginAsync(LoginRequest model)
        {
            var result = await _authService.VerifyPasswordAsync(model.Username, model.Password);
            if (!result.IsSuccess) return CreateTaskResult.Error<LoginResponse>(result);

            var user = result.Result;
            if (!IsAdmin() && user.IsDeleted) return CreateTaskResult.Error<LoginResponse>(ErrorKey.ENTRY_NOT_FOUND);

            var token = await _tokenService.CreateToken(user, model.RememberMe);

            return CreateTaskResult.Success(new LoginResponse()
            {
                Id = user.Id.ToString(),
                Username = user.Username,
                Locked = user.Locked,
                Role = user.Role.ToString(),
                Auth = token,
                IsDeleted = user.IsDeleted
            });
        }

        /// <summary>
        /// Refreshes the Token of a User
        /// </summary>
        /// <param name="model">The RefreshRequest</param>
        /// <returns>a login Response with all information of the loggedin user as well as a token</returns>
        public async Task<TaskResult<LoginResponse>> Refresh(RefreshRequest model)
        {
            var result = await _tokenService.RefreshToken(model.Token, model.RefreshToken);
            if (result.Result == null)
            {
                return CreateTaskResult.Error<LoginResponse>(result);
            }

            var user = result.Result.User;

            return CreateTaskResult.Success(new LoginResponse()
            {
                Id = user.Id.ToString(),
                Username = user.Username,
                Locked = user.Locked,
                Role = user.Role.ToString(),
                Auth = result.Result.TokenData
            });
        }

        /// <summary>
        /// Will revoke the refresh token of the current user
        /// </summary>
        /// <returns>The user information</returns>
        public async Task<TaskResult<object>> RevokeRefreshToken()
        {
            var result = await GetMe();
            if (result.Result == null) return CreateTaskResult.Error<object>(ErrorKey.ENTRY_NOT_FOUND);
            if (result.Result is not User user) return CreateTaskResult.Error<object>(ErrorKey.ENTRY_NOT_FOUND);

            user.RefreshToken = null;
            await _context.Users.Collection.UpdateOneAsync(GetFilter(user.Id), Builders<User>.Update.Set(x => x.RefreshToken, null));

            return Resolve(user);
        }

        /// <summary>
        /// Unlocks a User
        /// </summary>
        /// <param name="id">Id of the Entry</param>
        /// <returns>User</returns>
        public async Task<TaskResult<object>> UnlockAsync(string id)
        {
            var userInfo = await GetMe();
            if (!userInfo.IsSuccess) return CreateTaskResult.Error<object>(ErrorKey.ENTRY_NOT_FOUND);
            if ((userInfo.Result as UserResponse)?.Id == id.ToString()) return CreateTaskResult.Error<object>(ErrorKey.CANNOT_UNLOCK_SELF);

            var user = await _context.Users.FindByIdAsync(id);
            if (user == null) return CreateTaskResult.Error<object>(ErrorKey.ENTRY_NOT_FOUND);
            if (!user.Locked) return CreateTaskResult.Error<object>(ErrorKey.USER_NOT_LOCKED);


            user.Locked = false;
            user.LoginAttempts = 0;

            var update = Builders<User>.Update.Set(x => x.Locked, false).Set(x => x.LoginAttempts, 0);
            await _context.Users.Collection.UpdateOneAsync(GetFilter(id), update);

            return Resolve(user);
        }

        /// <summary>
        /// Overrides the UpdateAsync Method from GenericService
        /// </summary>
        /// <param name="id">Id for the Entry</param>
        /// <param name="entity">Entity Data</param>
        /// <returns>UserResponse as TaskResult</returns>
        public override async Task<TaskResult<object>> UpdateAsync(string id, UpdateUserRequest entity)
        {
            var current = await _context.Users.FindByIdAsync(id);
            if (current == null) return CreateTaskResult.Error<object>(ErrorKey.ENTRY_NOT_FOUND);

            _mapper.Map(entity, current, opts =>
            {
                opts.Items["IsAdmin"] = IsAdmin();
                opts.Items["IsOwner"] = IsOwnerOrAdmin(current);
            });

            if (entity.Password != null)
            {
                _authService.CreatePasswordHash(entity.Password, out byte[] passwordHash, out byte[] passwordSalt);

                current.PasswordHash = passwordHash;
                current.PasswordSalt = passwordSalt;
            }

            await _context.Users.Collection.ReplaceOneAsync(GetFilter(id), current);

            return Resolve(current);
        }

        /// <summary>
        /// Overrides the CreateAsync Method from GenericService
        /// </summary>
        /// <param name="id">Id for the Entry</param>
        /// <param name="entity">Entity Data</param>
        /// <returns>UserResponse as TaskResult</returns>
        public override async Task<TaskResult<object>> CreateAsync(CreateUserRequest entity)
        {
            _authService.CreatePasswordHash(entity.Password, out byte[] passwordHash, out byte[] passwordSalt);

            var user = _mapper.Map<User>(entity, opts =>
            {
                opts.Items["IsAdmin"] = IsAdmin();
                opts.Items["IsOwner"] = true;
            });

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            await _context.Users.Collection.InsertOneAsync(user);

            return Resolve(user);
        }

    }
}

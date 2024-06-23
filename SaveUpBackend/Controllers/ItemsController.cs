using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using SaveUpBackend.Common.Generics;
using SaveUpBackend.Interfaces;
using SaveUpModels.DTOs.Requests;
using SaveUpModels.DTOs.Responses;
using SaveUpModels.Enums;
using SaveUpModels.Models;
using SaveUpBackend.Common.Attributes;
using System.Diagnostics;
using MongoDB.Bson;
using System.Globalization;
using Microsoft.AspNetCore.Http;

namespace SaveUpBackend.Controllers
{
    public class ItemsController : GenericController<Item, ItemResponse, UpdateItemRequest, CreateItemRequest>
    {
        private readonly IItemService _service;
        
        public ItemsController(IItemService service) : base(service)
        {
            _service = service;
        }

        [HttpPost]
        [CustomAuthorize(Roles = nameof(RoleNames.User))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public override async Task<IActionResult> Create([FromBody] CreateItemRequest entity)
        {
            Console.WriteLine(entity);
            return await base.Create(entity);
        }

        /// <summary>
        /// Get all by timespan for user
        /// </summary>
        /// <param name="userId">The target User</param>
        /// <returns>all orders for that user</returns>
        [HttpGet("user/{userId}/{timeSpan}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<ItemResponse>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetByTimeSpanForUserAsync(string userId, string timeSpan)
        {
            var result = await _service.GetByTimeSpanForUserAsync(timeSpan, userId);
            return result.IsSuccess ? Ok(result.Result) : NotFound(result.Exception);
        }
    }
}

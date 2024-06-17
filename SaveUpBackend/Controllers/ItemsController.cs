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
            var result = await _service.CreateAsync(entity);
            return result.IsSuccess ? Ok(result.Result) : BadRequest(result.Error);
        }
    }
}

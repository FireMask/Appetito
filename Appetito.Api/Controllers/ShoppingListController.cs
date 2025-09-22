using Appetito.Application.Interfaces.Repositories;
using Appetito.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Appetito.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class ShoppingListController(IShoppingListService _shoppingListService) : ControllerBase
{
    // POST /api/v1/shopping-lists/generate
    [HttpPost("generate")]
    public async Task<IActionResult> GenerateShoppingList([FromBody] ShoppingListGenerateRequestDto dto, CancellationToken ct)
    {
        var result = await _shoppingListService.GenerateShoppingList(dto, ct);
        return Ok(result);
    }
}
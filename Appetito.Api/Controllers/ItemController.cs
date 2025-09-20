using Appetito.Application.Abstractions;
using Appetito.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Pantry.Api.Controllers;

[ApiController]
[Route("api/items")]
[Authorize]
public sealed class ItemsController(IItemRepository items) : ControllerBase
{
    private Guid HouseholdId =>
        Guid.TryParse(User.FindFirst("householdId")?.Value, out var id) ? id
        : throw new UnauthorizedAccessException("Missing householdId claim.");

    // GET /api/items?search=...
    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] string? search, CancellationToken ct)
    {
        var list = await items.ListAsync(HouseholdId, search, ct);
        var result = list.Select(i => new ItemSummaryDto(
            i.Id, i.Name, i.ReorderPoint, i.TargetQty, i.ShelfLifeDays, i.IsActive));
        return Ok(result);
    }
    // GET /api/items/{id}
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var item = await items.GetAsync(id, HouseholdId, ct);
        if (item is null) return NotFound();
        return Ok(new ItemDetailDto(
            item.Id, item.HouseholdId, item.Name, item.CategoryId, item.DefaultUnitId,
            item.ReorderPoint, item.TargetQty, item.ShelfLifeDays, item.IsActive, item.Notes));
    }

    // POST /api/items
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ItemCreateDto dto, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
            return BadRequest("Name is required.");
        if (dto.ReorderPoint < 0 || dto.TargetQty < 0)
            return BadRequest("Quantities must be non-negative.");

        var entity = new Item
        {
            Id = Guid.NewGuid(),
            HouseholdId = HouseholdId,
            Name = dto.Name.Trim(),
            CategoryId = dto.CategoryId,
            DefaultUnitId = dto.DefaultUnitId,
            ReorderPoint = dto.ReorderPoint,
            TargetQty = dto.TargetQty,
            ShelfLifeDays = dto.ShelfLifeDays,
            Notes = dto.Notes,
            IsActive = true
        };

        await items.AddAsync(entity, ct);
        await items.SaveChangesAsync(ct);

        return CreatedAtAction(nameof(GetById), new { id = entity.Id }, new { entity.Id });
    }

    // PUT /api/items/{id}
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] ItemUpdateDto dto, CancellationToken ct)
    {
        var entity = await items.GetAsync(id, HouseholdId, ct);
        if (entity is null) return NotFound();

        if (string.IsNullOrWhiteSpace(dto.Name))
            return BadRequest("Name is required.");
        if (dto.ReorderPoint < 0 || dto.TargetQty < 0)
            return BadRequest("Quantities must be non-negative.");

        entity.Name = dto.Name.Trim();
        entity.CategoryId = dto.CategoryId;
        entity.DefaultUnitId = dto.DefaultUnitId;
        entity.ReorderPoint = dto.ReorderPoint;
        entity.TargetQty = dto.TargetQty;
        entity.ShelfLifeDays = dto.ShelfLifeDays;
        entity.Notes = dto.Notes;

        await items.SaveChangesAsync(ct);
        return NoContent();
    }

    // DELETE /api/items/{id}  (soft-delete)
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> SoftDelete(Guid id, CancellationToken ct)
    {
        var entity = await items.GetAsync(id, HouseholdId, ct);
        if (entity is null) return NotFound();

        if (!entity.IsActive) return NoContent(); // already deleted
        entity.IsActive = false;

        await items.SaveChangesAsync(ct);
        return NoContent();
    }
}

// ==== DTOs ====
public sealed record ItemCreateDto(
    string Name,
    Guid CategoryId,
    Guid DefaultUnitId,
    decimal ReorderPoint,
    decimal TargetQty,
    int? ShelfLifeDays,
    string? Notes
);

public sealed record ItemUpdateDto(
    string Name,
    Guid CategoryId,
    Guid DefaultUnitId,
    decimal ReorderPoint,
    decimal TargetQty,
    int? ShelfLifeDays,
    string? Notes
);

public sealed record ItemSummaryDto(
    Guid Id,
    string Name,
    decimal ReorderPoint,
    decimal TargetQty,
    int? ShelfLifeDays,
    bool IsActive
);

public sealed record ItemDetailDto(
    Guid Id,
    Guid HouseholdId,
    string Name,
    Guid CategoryId,
    Guid DefaultUnitId,
    decimal ReorderPoint,
    decimal TargetQty,
    int? ShelfLifeDays,
    bool IsActive,
    string? Notes
);

using Appetito.Application.Interfaces.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Appetito.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UnitController(IUnitRepository _unit) : ControllerBase
{
    // GET /api/unit
    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken ct)
    {
        var list = await _unit.GetAll(ct);
        var result = list.Select(c => new UnitSummaryDto(c.Id, c.Name, c.Abbrev));
        return Ok(result);
    }

    // GET /api/unit/{id}
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var unit = await _unit.GetById(id, ct);
        if (unit is null)
            return NotFound();
        var result = new UnitDetailDto(unit.Id, unit.Name, unit.Abbrev, unit.IsCountable, unit.BaseUnitId, unit.FactorToBase);
        return Ok(result);
    }

    // POST /api/unit
    [HttpPost]
    public async Task<IActionResult> Create(UnitCreateDto dto, CancellationToken ct)
    {
        var unit = new Domain.Entities.Unit
        {
            Name = dto.Name,
            Abbrev = dto.Abbrev,
            IsCountable = dto.IsCountable,
            BaseUnitId = dto.BaseUnitId,
            FactorToBase = dto.FactorToBase
        };
        await _unit.Create(unit);
        await _unit.SaveChanges(ct);
        var result = new UnitDetailDto(unit.Id, unit.Name, unit.Abbrev, unit.IsCountable, unit.BaseUnitId, unit.FactorToBase);
        
        return CreatedAtAction(nameof(GetById), new { id = unit.Id }, result);
    }

    // PUT /api/unit/{id}
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UnitUpdateDto dto, CancellationToken ct)
    {
        var unit = await _unit.GetById(id, ct);
        if (unit is null)
            return NotFound();

        unit.Name = dto.Name;

        await _unit.Update(unit);
        await _unit.SaveChanges(ct);

        return Ok();
    }

    // DELETE /api/category/{id}
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var unit = await _unit.GetById(id, ct);
        if (unit is null)
            return NotFound();

        await _unit.Delete(unit);
        await _unit.SaveChanges(ct);

        return NoContent();
    }

    // ==== DTOs ====
    public sealed record UnitCreateDto(string Name, string Abbrev, bool IsCountable, Guid? BaseUnitId, decimal FactorToBase);
    public sealed record UnitUpdateDto(string Name, string Abbrev, bool IsCountable, Guid? BaseUnitId, decimal FactorToBase);
    public sealed record UnitSummaryDto(Guid Id, string Name, string Abbrev);
    public sealed record UnitDetailDto(Guid Id, string Name, string Abbrev, bool IsCountable, Guid? BaseUnitId, decimal FactorToBase);
}
using Appetito.Application.Interfaces.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Appetito.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class HouseholdController(IHouseholdRepository _household) : ControllerBase
{
    // GET /api/household
    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken ct)
    {
        var list = await _household.GetAll(ct);
        var result = list.Select(c => new HouseholdSummaryDto(c.Id, c.Name));
        return Ok(result);
    }

    // GET /api/household/{id}
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var household = await _household.GetById(id, ct);
        if (household is null)
            return NotFound();
        var result = new HouseholdDetailDto(household.Id, household.Name);
        return Ok(result);
    }

    // POST /api/household
    [HttpPost]
    public async Task<IActionResult> Create(HouseholdCreateDto dto, CancellationToken ct)
    {
        var household = new Domain.Entities.Household
        {
            Name = dto.Name
        };
        await _household.Create(household);
        await _household.SaveChanges(ct);
        var result = new HouseholdDetailDto(household.Id, household.Name);
        
        return CreatedAtAction(nameof(GetById), new { id = household.Id }, result);
    }

    // PUT /api/household/{id}
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, HouseholdUpdateDto dto, CancellationToken ct)
    {
        var cat = await _household.GetById(id, ct);
        if (cat is null)
            return NotFound();

        cat.Name = dto.Name;

        await _household.Update(cat);
        await _household.SaveChanges(ct);

        return Ok();
    }

    // DELETE /api/household/{id}
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var cat = await _household.GetById(id, ct);
        if (cat is null)
            return NotFound();

        await _household.Delete(cat);
        await _household.SaveChanges(ct);

        return NoContent();
    }

    // ==== DTOs ====
    public sealed record HouseholdCreateDto(string Name);
    public sealed record HouseholdUpdateDto(string Name);
    public sealed record HouseholdSummaryDto(Guid Id, string Name);
    public sealed record HouseholdDetailDto(Guid Id, string Name);
}
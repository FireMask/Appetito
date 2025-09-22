using Appetito.Application.Interfaces.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Appetito.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CategoryController(ICategoryRepository category) : ControllerBase
{
    // GET /api/category
    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken ct)
    {
        var list = await category.GetAll(ct);
        var result = list.Select(c => new CategorySummaryDto(c.Id, c.Name));
        return Ok(result);
    }

    // GET /api/category/{id}
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var cat = await category.GetById(id, ct);
        if (cat is null)
            return NotFound();
        var result = new CategoryDetailDto(cat.Id, cat.Name);
        return Ok(result);
    }

    // POST /api/category
    [HttpPost]
    public async Task<IActionResult> Create(CategoryCreateDto dto, CancellationToken ct)
    {
        var cat = new Domain.Entities.Category
        {
            Name = dto.Name
        };
        await category.Create(cat);
        await category.SaveChanges(ct);
        var result = new CategoryDetailDto(cat.Id, cat.Name);
        
        return CreatedAtAction(nameof(GetById), new { id = cat.Id }, result);
    }

    // PUT /api/category/{id}
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, CategoryUpdateDto dto, CancellationToken ct)
    {
        var cat = await category.GetById(id, ct);
        if (cat is null)
            return NotFound();

        cat.Name = dto.Name;

        await category.Update(cat);
        await category.SaveChanges(ct);

        return Ok();
    }

    // DELETE /api/category/{id}
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var cat = await category.GetById(id, ct);
        if (cat is null)
            return NotFound();

        await category.Delete(cat);
        await category.SaveChanges(ct);

        return NoContent();
    }

    // ==== DTOs ====
    public sealed record CategoryCreateDto(string Name);
    public sealed record CategoryUpdateDto(string Name);
    public sealed record CategorySummaryDto(Guid Id, string Name);
    public sealed record CategoryDetailDto(Guid Id, string Name);
}
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VismaEnterprise.HomeTask.Application.DTOs;
using VismaEnterprise.HomeTask.Application.Services.Interfaces;

namespace VismaEnterprise.HomeTask.Web.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class EntriesController(IBookCatalogueService bookCatalogueService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var books = await bookCatalogueService.GetEntriesAsync();
        return Ok(books);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var entry = await bookCatalogueService.GetEntryAsync(id);

        if (entry == null)
        {
            return NotFound($"Entry with id {id} does not exist.");
        }

        return Ok(entry);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Put(Guid id, [FromBody] CatalogueEntryDto entryDto)
    {
        var entry = await bookCatalogueService.EditEntryAsync(id, entryDto);

        if (entry == null)
        {
            return NotFound($"Entry with id {id} does not exist.");
        }

        return Ok(entry);
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] CatalogueEntryDto entryDto)
    {
        var entry = await bookCatalogueService.CreateEntryAsync(entryDto);
        return Ok(entry);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var entry = await bookCatalogueService.DeleteEntryAsync(id);

        if (entry == null)
        {
            return NotFound($"Entry with id {id} does not exist.");
        }

        return Ok(entry);
    }
}
using Microsoft.AspNetCore.Mvc;
using TodoApi.Models;
using TodoApi.Services;

namespace TodoApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TodoItemsController : ControllerBase
{
    private readonly TodoItemsService _todoItemsService;

    public TodoItemsController(TodoItemsService todoItemsService) =>
        _todoItemsService = todoItemsService;

    [HttpGet]
    public async Task<ActionResult<List<TodoItemDTO>>> GetAll()
    {
        var items = await _todoItemsService.GetAsync();
        return items.Select(item => ItemToDTO(item)).ToList();
    }

    [HttpGet("{id:length(24)}")]
    public async Task<ActionResult<TodoItemDTO>> GetById(string id)
    {
        var item = await _todoItemsService.GetAsync(id);
        if (item is null) return NotFound();
        return ItemToDTO(item);
    }

    [HttpPost]
    public async Task<IActionResult> Create(TodoItemDTO itemDTO)
    {
        var item = new TodoItem
        {
            Name = itemDTO.Name,
            IsComplete = itemDTO.IsComplete
        };

        await _todoItemsService.CreateAsync(item);
        return CreatedAtAction(nameof(GetById), new { id = item.Id }, ItemToDTO(item));
    }

    [HttpPut("{id:length(24)}")]
    public async Task<IActionResult> Update(string id, TodoItemDTO itemDTO)
    {
        var item = await _todoItemsService.GetAsync(id);
        if (item is null) return NotFound();

        item.Name = itemDTO.Name;
        item.IsComplete = itemDTO.IsComplete;

        await _todoItemsService.UpdateAsync(id, item);
        return NoContent();
    }

    [HttpDelete("{id:length(24)}")]
    public async Task<IActionResult> Delete(string id)
    {
        var item = await _todoItemsService.GetAsync(id);
        if (item is null) return NotFound();

        await _todoItemsService.RemoveAsync(id);
        return NoContent();
    }

    private static TodoItemDTO ItemToDTO(TodoItem item) =>
        new TodoItemDTO
        {
            Id = item.Id,
            Name = item.Name,
            IsComplete = item.IsComplete
        };
}
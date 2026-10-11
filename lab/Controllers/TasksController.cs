using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagementApi.Models;

namespace TaskManagementApi.Controllers;

[ApiController]
[Route("api/tasks")]
public class TasksController : ControllerBase
{
    private static readonly List<TaskItem> tasks = new()
    {
        new TaskItem { Id = 1, Title = "Настроить JWT", Description = "Issuer, Audience, ключ подписи", IsCompleted = true },
        new TaskItem { Id = 2, Title = "Защитить POST и DELETE", Description = "Атрибут [Authorize]" },
        new TaskItem { Id = 3, Title = "Проверить безопасность", Description = "Шесть проверок из методички" }
    };

    // GET /api/tasks — без авторизации
    [HttpGet]
    public IEnumerable<TaskItem> GetAll() => tasks;

    // GET /api/tasks/{id} — без авторизации
    [HttpGet("{id:int}")]
    public IActionResult GetById(int id)
    {
        TaskItem? task = tasks.FirstOrDefault(t => t.Id == id);
        return task is null ? NotFound() : Ok(task);
    }

    // POST /api/tasks — только с JWT
    [Authorize]
    [HttpPost]
    public IActionResult Create([FromBody] TaskItem task)
    {
        // п. 7 методички: имя пользователя из Claims токена
        string? userName = HttpContext.User.Claims
            .FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;

        task.Id = tasks.Count == 0 ? 1 : tasks.Max(t => t.Id) + 1;
        tasks.Add(task);

        return CreatedAtAction(nameof(GetById), new { id = task.Id }, new { task, createdBy = userName });
    }

    // DELETE /api/tasks/{id} — только с JWT
    [Authorize]
    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
    {
        TaskItem? task = tasks.FirstOrDefault(t => t.Id == id);
        if (task is null) return NotFound();

        tasks.Remove(task);
        return NoContent();
    }
}

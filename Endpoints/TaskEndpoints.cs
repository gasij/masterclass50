using TaskBoard.Models;
using TaskBoard.Services;

namespace TaskBoard.Endpoints;

/// <summary>Адреса API: какой URL что делает.</summary>
public static class TaskEndpoints
{
    public static void MapTaskEndpoints(this WebApplication app)
    {
        var api = app.MapGroup("/api/tasks");

        // GET /api/tasks?search=git — список задач (с поиском)
        api.MapGet("", async (ITaskService service, string? search) =>
            Results.Ok(await service.GetAllAsync(search)));

        // GET /api/tasks/stats — счётчики для шапки
        api.MapGet("/stats", async (ITaskService service) =>
            Results.Ok(await service.GetStatsAsync()));

        // POST /api/tasks — создать задачу
        api.MapPost("", async (CreateTaskRequest request, ITaskService service) =>
        {
            var title = request.Title?.Trim();

            if (string.IsNullOrEmpty(title))
                return Results.BadRequest(new { error = "Напишите название дела." });

            if (title.Length > 120)
                return Results.BadRequest(new { error = "Название длиннее 120 символов. Сократите его." });

            var created = await service.AddAsync(title, request.Priority, request.DueDate);
            return Results.Created($"/api/tasks/{created.Id}", created);
        });

        // PATCH /api/tasks/5/state — перенести задачу в другую колонку
        api.MapPatch("/{id:int}/state", async (int id, MoveTaskRequest request, ITaskService service) =>
        {
            var moved = await service.MoveAsync(id, request.State);
            return moved is null ? Results.NotFound(new { error = "Такого дела нет." }) : Results.Ok(moved);
        });

        // DELETE /api/tasks/5 — удалить задачу
        api.MapDelete("/{id:int}", async (int id, ITaskService service) =>
            await service.DeleteAsync(id)
                ? Results.NoContent()
                : Results.NotFound(new { error = "Такого дела нет." }));
    }
}

using Microsoft.EntityFrameworkCore;
using TaskBoard.Data;
using TaskBoard.Models;

namespace TaskBoard.Services;

/// <summary>Вся логика работы с задачами. Endpoints только принимают запросы и вызывают этот класс.</summary>
public class TaskService(AppDbContext db) : ITaskService
{
    public async Task<List<TaskItem>> GetAllAsync(string? search = null)
    {
        var tasks = await db.Tasks.ToListAsync();

        // Фильтруем в памяти: так поиск не зависит от регистра и для русских букв тоже.
        if (!string.IsNullOrWhiteSpace(search))
        {
            var text = search.Trim();
            tasks = tasks
                .Where(t => t.Title.Contains(text, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        // Сначала самые срочные, внутри одной срочности — старые выше новых.
        return tasks
            .OrderByDescending(t => t.Priority)
            .ThenBy(t => t.CreatedAt)
            .ToList();
    }

    public async Task<TaskItem> AddAsync(string title, TaskPriority priority, DateOnly? dueDate)
    {
        var task = new TaskItem
        {
            Title = title,
            Priority = priority,
            DueDate = dueDate,
            State = TaskState.Todo,
            CreatedAt = DateTime.UtcNow
        };

        db.Tasks.Add(task);
        await db.SaveChangesAsync();
        return task;
    }

    public async Task<TaskItem?> MoveAsync(int id, TaskState state)
    {
        var task = await db.Tasks.FindAsync(id);
        if (task is null) return null;

        task.State = state;
        await db.SaveChangesAsync();
        return task;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var task = await db.Tasks.FindAsync(id);
        if (task is null) return false;

        db.Tasks.Remove(task);
        await db.SaveChangesAsync();
        return true;
    }

    public async Task<BoardStats> GetStatsAsync()
    {
        var tasks = await db.Tasks.ToListAsync();
        var today = DateOnly.FromDateTime(DateTime.Today);

        return new BoardStats(
            Total: tasks.Count,
            Todo: tasks.Count(t => t.State == TaskState.Todo),
            InProgress: tasks.Count(t => t.State == TaskState.InProgress),
            Done: tasks.Count(t => t.State == TaskState.Done),
            Overdue: tasks.Count(t => t.State != TaskState.Done && t.DueDate < today)
        );
    }
}

using TaskBoard.Models;

namespace TaskBoard.Services;

public interface ITaskService
{
    Task<List<TaskItem>> GetAllAsync(string? search = null);
    Task<TaskItem> AddAsync(string title, TaskPriority priority, DateOnly? dueDate);
    Task<TaskItem?> MoveAsync(int id, TaskState state);
    Task<bool> DeleteAsync(int id);
    Task<BoardStats> GetStatsAsync();
}

namespace TaskBoard.Models;

/// <summary>Одна задача. Каждая строка таблицы Tasks в базе — это один такой объект.</summary>
public class TaskItem
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public TaskPriority Priority { get; set; } = TaskPriority.Medium;
    public TaskState State { get; set; } = TaskState.Todo;
    public DateOnly? DueDate { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

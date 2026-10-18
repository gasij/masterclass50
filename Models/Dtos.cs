namespace TaskBoard.Models;

// DTO — объекты, которые ходят между браузером и сервером.
// record — короткий способ описать класс, который только хранит данные.

public record CreateTaskRequest(string Title, TaskPriority Priority, DateOnly? DueDate);

public record MoveTaskRequest(TaskState State);

public record BoardStats(int Total, int Todo, int InProgress, int Done, int Overdue);

using TaskBoard.Models;

namespace TaskBoard.Data;

/// <summary>Заполняет пустую базу примерами, чтобы доска не была пустой при первом запуске.</summary>
public static class DbSeeder
{
    public static void Seed(AppDbContext db)
    {
        if (db.Tasks.Any()) return;

        var today = DateOnly.FromDateTime(DateTime.Today);

        db.Tasks.AddRange(
            new TaskItem { Title = "Придумать название проекта", Priority = TaskPriority.Low, State = TaskState.Done },
            new TaskItem { Title = "Подключить базу данных SQLite", Priority = TaskPriority.Medium, State = TaskState.Done },
            new TaskItem { Title = "Написать метод добавления задачи", Priority = TaskPriority.High, State = TaskState.InProgress },
            new TaskItem { Title = "Сдать домашку по C#", Priority = TaskPriority.High, State = TaskState.Todo, DueDate = today.AddDays(-1) },
            new TaskItem { Title = "Выложить проект на GitHub", Priority = TaskPriority.High, State = TaskState.Todo, DueDate = today.AddDays(3) },
            new TaskItem { Title = "Научиться фильтровать задачи через LINQ", Priority = TaskPriority.Medium, State = TaskState.Todo },
            new TaskItem { Title = "Сделать тёмную тему", Priority = TaskPriority.Low, State = TaskState.Todo }
        );

        db.SaveChanges();
    }
}

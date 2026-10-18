namespace TaskBoard.Models;

/// <summary>В какой колонке доски лежит задача.</summary>
public enum TaskState
{
    Todo,        // Надо сделать
    InProgress,  // В работе
    Done         // Готово
}

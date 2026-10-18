namespace TaskBoard.Models;

/// <summary>Насколько задача срочная. Числа важны: по ним сортируем.</summary>
public enum TaskPriority
{
    Low = 0,     // Не горит
    Medium = 1,  // Обычно
    High = 2     // Срочно
}

# Дела — доска задач на C# и JavaScript

Учебный проект: канбан-доска «Надо сделать / В работе / Готово».
Сервер написан на C# (ASP.NET Core Minimal API), данные лежат в SQLite,
интерфейс — обычные HTML, CSS и JavaScript без фреймворков.

## Запуск

Нужен [.NET SDK 10](https://dotnet.microsoft.com/download) (работает на Windows, macOS и Linux).

```bash
dotnet run
```

Откройте http://localhost:5080. База `taskboard.db` создастся сама при первом запуске
и заполнится примерами. Чтобы начать с чистого листа, остановите сервер и удалите файл `taskboard.db`.

Если у вас стоит другая версия .NET, поменяйте `net10.0` в `TaskBoard.csproj`
(например, на `net8.0` и версию пакета EF Core на `8.0.*`).

## Что умеет

- добавлять дела со срочностью и сроком
- переносить карточки между колонками: перетаскиванием или кнопками ← →
- удалять дела
- искать по названию
- показывать, сколько дел осталось и сколько просрочено

## Как устроен проект

```
Program.cs                 запуск сервера, подключение базы
Models/                    описание данных: задача, статусы, DTO
Data/AppDbContext.cs       связь C#-классов с таблицами SQLite
Data/DbSeeder.cs           примеры задач для первого запуска
Services/TaskService.cs    вся логика: добавить, найти, перенести, удалить
Endpoints/TaskEndpoints.cs адреса API
wwwroot/                   страница: index.html, css/style.css, js/app.js, js/api.js
TaskBoard.http             готовые запросы к API (VS Code с REST Client, Rider, Visual Studio)
```

Путь запроса: кнопка в браузере → `js/api.js` → `Endpoints/TaskEndpoints.cs` →
`Services/TaskService.cs` → `Data/AppDbContext.cs` → SQLite.

## API

| Метод  | Адрес                    | Что делает                  |
|--------|--------------------------|-----------------------------|
| GET    | `/api/tasks?search=текст`| список задач, поиск         |
| GET    | `/api/tasks/stats`       | счётчики                    |
| POST   | `/api/tasks`             | создать задачу              |
| PATCH  | `/api/tasks/{id}/state`  | перенести в другую колонку  |
| DELETE | `/api/tasks/{id}`        | удалить задачу              |

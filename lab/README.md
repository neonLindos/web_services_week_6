# Лабораторная работа — Модуль 06

**Тема:** Защита доступа к API
**Проект:** `TaskManagementApi` (TaskItem)
**Используемые технологии:** ASP.NET Core Web API, JWT Bearer, Swagger

## Как запустить

В этой папке выполните `dotnet run` (нужен .NET 10 SDK; `make` делает то же самое).
Swagger: http://localhost:5161/swagger. Кнопка **Authorize** принимает токен без слова `Bearer`.
Готовые запросы для проверок — в [lab.http](lab.http). Данные хранятся в памяти.
Учебный пользователь: `student` / `12345`.

## Что реализовано

| Метод | Endpoint | Доступ | Успех | Ошибки |
|---|---|---|---|---|
| POST | `/api/auth/login` | открыт | 200, `accessToken` | 401 — неверный логин или пароль |
| GET | `/api/auth/me` | JWT | 200, список Claims | 401 |
| GET | `/api/tasks` | открыт | 200 | — |
| GET | `/api/tasks/{id}` | открыт | 200 | 404 |
| POST | `/api/tasks` | **JWT** | 201, задача и `createdBy` | 400 (пустой `Title`), 401 |
| DELETE | `/api/tasks/{id}` | **JWT** | 204 | 401, 404 |

`/api/auth/me` не входит в методичку: он нужен, чтобы показать содержимое Claims для отчёта.

## JWT

| Параметр | Значение |
|---|---|
| Issuer | `task-management-api` |
| Audience | `task-management-clients` |
| Алгоритм | HS256, ключ `Jwt:Key` из [appsettings.json](appsettings.json) (учебный) |
| Срок действия | 30 минут (`Jwt:ExpireMinutes`), `ClockSkew = 0` |
| Claims | `Name = student`, `Role = User` |

Сервер проверяет подпись, Issuer, Audience и срок действия (`TokenValidationParameters` в [Program.cs](Program.cs)).
Порядок middleware: `UseAuthentication()` → `UseAuthorization()`.

Пример Claims (ответ `GET /api/auth/me`):

```json
{
  "name": "student",
  "claims": [
    { "type": "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name", "value": "student" },
    { "type": "http://schemas.microsoft.com/ws/2008/06/identity/claims/role", "value": "User" },
    { "type": "exp", "value": "1791741607" },
    { "type": "iss", "value": "task-management-api" },
    { "type": "aud", "value": "task-management-clients" }
  ]
}
```

Имя пользователя в `POST /api/tasks` берётся из `HttpContext.User.Claims` (Claim `Name`) и возвращается в поле `createdBy`.

## Проверка безопасности (п. 9)

| № | Проверка | Ожидаемый результат | Получено |
|---|---|---|---|
| 1 | GET без токена | 200 OK | 200 |
| 2 | POST без токена | 401 Unauthorized | 401 |
| 3 | POST с корректным JWT | успешное выполнение | 201 |
| 4 | DELETE с корректным JWT | успешное выполнение | 204 |
| 5 | Запрос с изменённым JWT | 401 Unauthorized | 401 |
| 6 | Запрос с истёкшим JWT | 401 Unauthorized | 401 |

Как воспроизвести вручную: для № 5 поменяйте любой символ в середине токена. Для № 6 задайте
`Jwt:ExpireMinutes` = `1`, получите токен и подождите больше минуты (запас `ClockSkew` равен нулю).

## Что проверено

Все шесть проверок, а также неверный пароль, чужой ключ подписи, 404 и 400 выполнены автоматически на настоящем коде приложения в памяти
(тестовый хост, без запуска сервера и без браузера): 23 из 23 утверждений прошли. Скриншоты не снимались —
их нужно сделать после своего запуска (Swagger или [lab.http](lab.http)): получение JWT, POST без токена (401),
POST с токеном (201), ответ `/api/auth/me`.

## Краткий вывод

JWT позволяет не передавать пароль с каждым запросом: клиент один раз получает подписанный токен
и затем отправляет его в заголовке `Authorization: Bearer`. Сервер проверяет подпись, издателя, получателя и срок,
поэтому подделанный или просроченный токен получает 401. Атрибут `[Authorize]` закрывает только изменяющие методы,
а чтение остаётся открытым. Claims позволяют узнать пользователя внутри метода без обращения к базе.

## Итоговая таблица

| Метод | Endpoint | Назначение |
|---|---|---|
| POST | `/api/auth/login` | получить JWT |
| GET | `/api/tasks` | список задач |
| GET | `/api/tasks/{id}` | задача по Id |
| POST | `/api/tasks` | добавить задачу (JWT) |
| DELETE | `/api/tasks/{id}` | удалить задачу (JWT) |

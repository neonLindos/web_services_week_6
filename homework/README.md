# Домашняя работа — Модуль 06

**Тема:** Защита доступа к API
**Проект:** `homework` (Product, Category, User)
**Используемые технологии:** ASP.NET Core Web API, JWT Bearer, BCrypt

## Как запустить

В этой папке выполните `dotnet run` (нужен .NET 10 SDK). API слушает http://localhost:5122.
Swagger UI в проекте нет, только описание OpenAPI: http://localhost:5122/openapi/v1.json.
Для проверки используйте [homework.http](homework.http). Данные хранятся в памяти.

## Что реализовано

| Метод | Endpoint | Доступ | Успех | Ошибки |
|---|---|---|---|---|
| POST | `/api/auth/register` | открыт | 200 | 400 — пароли не совпали, 409 — email занят |
| POST | `/api/auth/login` | открыт | 200, `accessToken` | 401 |
| GET | `/api/products` | открыт | 200 | — |
| GET | `/api/products/{id}` | открыт | 200 | 404 |
| POST | `/api/products` | **JWT** | 200, товар и `createdBy` | 401 |
| DELETE | `/api/products/{id}` | **JWT** | 204 | 401, 404 |

Вход выполняется по `email` и паролю (в методичке — «логин и пароль»). Перед первым входом нужно зарегистрироваться:
пользователи хранятся в памяти и пропадают при перезапуске. Пароль хранится как хеш BCrypt.

## JWT

Параметры берутся из секции `Jwt` в [appsettings.json](appsettings.json): `Issuer`, `Audience`, `Key` (учебный ключ),
`ExpireMinutes` = 60. Проверяются подпись, Issuer, Audience и срок действия, `ClockSkew = 0`.
Токен содержит Claims `NameIdentifier`, `Name` и `Email`. Дополнительное задание: в `POST /api/products`
имя пользователя читается из `HttpContext.User.Claims` и возвращается в поле `createdBy`.

## Проверка

| Ситуация | Результат |
|---|---|
| `GET /api/products` без токена | 200 |
| `POST /api/products` без токена | 401 |
| `POST /api/products` с корректным JWT | 200, `createdBy` = email пользователя |
| `DELETE /api/products/{id}` без токена / с токеном | 401 / 204 |
| Изменённый JWT | 401 |
| Истёкший JWT | 401 |

Эти сценарии выполнены автоматически на настоящем коде приложения в памяти (тестовый хост, без запуска сервера
и браузера): 11 из 11 утверждений прошли. Скриншоты по списку из методички нужно снять после своего запуска.
Для истёкшего токена задайте `Jwt:ExpireMinutes` = `1` и подождите больше минуты.

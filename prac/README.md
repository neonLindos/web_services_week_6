# Практическая работа — Модуль 06

**Тема:** Защита доступа к API (API Key и JWT)
**Проект:** `SecureProductsApi` (Product)
**Используемые технологии:** ASP.NET Core Web API, JWT Bearer, Swagger

## Как запустить

В этой папке выполните `dotnet run` (нужен .NET 10 SDK; `make` делает то же самое).
Swagger: http://localhost:5162/swagger (кнопка **Authorize** принимает `X-Api-Key` и JWT).
Готовые запросы — в [prac.http](prac.http). Данные хранятся в памяти.
API-ключ — `SecretKey123` (`ApiKey` в [appsettings.json](appsettings.json)), логин — `student` / `12345`.

## Что реализовано

| Задание | Что сделано |
|---|---|
| 1 | `ProductController`: `GET /api/product` возвращает три товара, `Product` содержит `Id`, `Name`, `Price` |
| 2 | Атрибут [`[ApiKey]`](Filters/ApiKeyAttribute.cs) проверяет заголовок `X-Api-Key`; нет ключа или он неверный — 401 |
| 3 | JWT-аутентификация в [Program.cs](Program.cs): Issuer, Audience, `IssuerSigningKey`, проверка срока, `ClockSkew = 0`; подключены `UseAuthentication()` и `UseAuthorization()` |
| 4 | `POST /api/auth/login` принимает `username` и `password`, возвращает JWT на 30 минут |
| 5 | `[Authorize]` стоит на `GET /api/product/secure` |
| 6 | Токен передаётся в `Authorization: Bearer <token>` |
| 7 | Четыре ситуации проверены, коды в таблице ниже |

| Метод | Endpoint | Защита | Успех | Ошибка |
|---|---|---|---|---|
| GET | `/api/product` | `X-Api-Key` | 200 | 401 |
| GET | `/api/product/secure` | JWT | 200 | 401 |
| POST | `/api/auth/login` | открыт | 200 | 401 |

`/api/product/secure` добавлен, чтобы показать `[Authorize]` на отдельном методе: методичка просит защитить «один из методов»,
а исходный `GET /api/product` по заданию 2 закрыт ключом. Ключ и JWT здесь независимы: для `secure` ключ не нужен, для списка не нужен токен.

## Задание 7 — коды ответов

| № | Ситуация | Запрос | HTTP-код |
|---|---|---|---|
| 1 | Без токена | `GET /api/product/secure` | 401 |
| 2 | Корректный JWT | `GET /api/product/secure` + Bearer | 200 |
| 3 | Изменённый JWT | изменён символ в теле токена | 401 |
| 4 | Истёкший JWT | срок действия в прошлом | 401 |

Дополнительно по заданию 2: без ключа — 401, неверный ключ — 401, верный ключ — 200.

## Что проверено

Все сценарии выше выполнены автоматически на настоящем коде приложения в памяти (тестовый хост, без запуска сервера и браузера):
11 из 11 утверждений прошли. Скриншоты не снимались — их нужно сделать после своего запуска.
Для истёкшего токена вручную задайте `Jwt:ExpireMinutes` = `1`, получите токен и подождите больше минуты.

## Итоговая таблица

| Метод | Endpoint | Назначение |
|---|---|---|
| GET | `/api/product` | список товаров (X-Api-Key) |
| POST | `/api/auth/login` | получить JWT |
| GET | `/api/product/secure` | список товаров (JWT) |

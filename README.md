# Account Service

Данный проект это сервис для управления счетами пользователей и их транзакциями в банке.
Проект в целом делал для пониманияя микросервисной архитектуры.

<!-- TABLE OF CONTENTS -->
<details>
  <summary>Table of Contents</summary>
  <ol>
    <li>
      <a href="#stack">Stack</a>
    </li>
    <li>
      <a href="#to-run-by-docker">To Run by Docker</a>
    </li>
    <li><a href="#to-run-tests">To Run Tests</a></li>
    <li>
      <a href="#to-log-in">To Log In</a>
    </li>
    <li><a href="#api-overview">API Overview</a></li>
  </ol>
</details>

## Stack:

- ASP.NET Core,
- Entity Framework Core with Postgres,
- Hangfire,
- Keycloak + MariaDb,
- Swagger,
- JWT Auth,
- MediatR,
- xUnit + Moq + Testcontainers,
- FluentValidation
- AutoMapper
- RabbitMQ + MassTransit

Architecture style: `Vertical Slice`

## To Run by Docker:

```
docker-compose up --build
```

- localhost:80 - Web API
- localhost:80/hangfire-dashboard - Hangfire Dashboard
- localhost:8080 - Keycloak
- localhost:15672 - Rabbit MQ Manager

## To Run Tests:

```
sudo dotnet test
```

## To Log in:

1) Откройте Swagger
2) Нажмите на кнопку "Authorize" сверху на правом углу
3) Выберите "Select All" и введите "microservice-bank-client" в client ID
4) После, Swagger должен вас сделать переадресацию на страницу Keycloak

## API Overview

**⚠ Все Endpoints требуют авторизации**

Так же если хотите поподробнее прочитать про API, в полная документация есть в файле ApiManual.md.

### WalletController

1) GET /api/wallets/{id}
2) GET /api/wallets/all
3) POST /api/wallets/create
4) PUT /api/wallets/update
5) PATCH /api/wallets/update-interest-rate
6) DELETE /api/wallets/delete/{id}

### TransactionController

1) GET /transactions/{id}
2) GET /transactions/all/{accountId}/{fromAtUtc}
3) POST /transactions/create
4) POST /transactions/transfer-money


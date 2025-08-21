# API Documentation:

**⚠ Все Endpoints требуют авторизации**

**⚠ Все Endpoints возвращают `MbResult<T>` объект. Пример:**

#### For a success result:

```
{
  "IsSuccess": true,
  "Result": "Result",
  "Error": null,
  "CreatedAtUtc": "2025-08-13T14:32:45Z"
}
```

#### For a failure result :

```
{
  "IsSuccess": false,
  "Result": null,
  "Error": {
    "ErrorMessage": "Unable to connect to the database",
    "Source": "AccountService.Database",
    "HelpLink": "https://docs.example.com/errors/db-connection",
    "StackTrace": "at AccountService.Db.Connect() in /src/Db.cs:line 45",
    "InnerException": {
      "ErrorMessage": "Timeout expired while connecting",
      "Source": "System.Data.SqlClient",
      "HelpLink": null,
      "StackTrace": "at System.Data.SqlClient.SqlConnection.Open()",
      "InnerException": null
    }
  },
  "CreatedAtUtc": "2025-08-13T14:35:10Z"
}
```

### Wallet Controller

Manage user wallets (create, update, delete, fetch).

##### GET /api/wallets/{id}

Fetch a wallet by its ID.

Responses:

1) 200 OK – Wallet found
2) 401 Unauthorized
3) 403 Forbidden – Not the owner
4) 404 Not Found

##### GET /api/wallets/all

Fetch all wallets owned by the authenticated user.

Responses:

1) 200 OK – List of wallets
2) 401 Unauthorized

##### POST /api/wallets/create

Create a new wallet.

Body (form-data):

1) Wallet type (e.g. Checking, Credit)
2) IsoCurrency: ISO 4217 currency code (e.g. USD)
3) Balance: Initial balance
4) InterestRate (optional): For interest-based wallets
5) ClosedAtUtc (optional)

Responses:

1) 201 Created
2) 400 Bad Request
3) 401 Unauthorized
4) 404 Not Found – Owner not found

##### PUT /api/wallets/update

Fully update an existing wallet. Only allowed if it has not been used.

Body (form-data):

1) Id
2) NewType
3) NewIsoCurrencyCode
4) NewBalance
5) NewInterestRate
6) ClosedAtUtc

Responses:

1) 200 OK
2) 400 Bad Request – Already used or invalid data
3) 401 Unauthorized
4) 403 Forbidden
5) 404 Not Found

##### PATCH /api/wallets/update-interest-rate

Partially update the interest rate and/or closed date.

Body (form-data):

1) Id
2) NewInterestRate
3) ClosedAtUtc

Responses:

1) 200 OK
2) 400 Bad Request
3) 401 Unauthorized
4) 403 Forbidden
5) 404 Not Found

##### DELETE /api/wallets/delete/{id} - Delete a wallet by ID.

Responses:

1) 200 OK
2) 400 Bad Request – Already deleted
3) 401 Unauthorized
4) 403 Forbidden
5) 404 Not Found

### Transaction Controller

Manage money transfers and view transaction history.

##### GET /transactions/{id} - Get a transaction by ID.

Responses:

1) 200 OK
2) 401 Unauthorized
3) 403 Forbidden
4) 404 Not Found

##### GET /transactions/all/{accountId}/{fromAtUtc} - Get all transactions for a wallet from a specific UTC date.

Path Params:

1) accountId (GUID)
2) fromAtUtc (ISO datetime string)

Responses:

1) 200 OK
2) 401 Unauthorized
3) 403 Forbidden
4) 404 Not Found

##### POST /transactions/create - Create a new transaction (debit/credit between accounts).

Body (form-data):

1) AccountId and CounterpartyAccountId (not required)
2) Sum (decimal)
3) TransactionType (Debit or Credit)
4) IsoCurrencyCode (string)
5) Description (3–5000 chars)

Responses:

1) 201 Created
2) 400 Bad Request
3) 401 Unauthorized
4) 402 Payment Required – Insufficient balance
5) 403 Forbidden
6) 404 Not Found

##### POST /transactions/transfer-money - Transfer money from one wallet to another (Debit).

Body (form-data):

1) AccountId
2) TransferToCounterpartyAccountId
3) Sum
4) IsoCurrencyCode
5) Description

Responses: Same as /transactions/create

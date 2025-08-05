## To Run:

```
docker-compose up --build
```

###### localhost:80 - Web API, localhost:8080 - Keycloak

### To Use Keycloak, You need:

1) Open localhost:8080 and login as admin (password "admin")
2) Go to Realms Settings
3) Point to the Action menu in the top right corner of the realm settings screen, and select Import.
4) Choose the keycloak-main-realm.json file from this folder

#### If the json file was invalid:

```
1) Open keycloak in your browser
2) Log in as admin
3) Create new realm
4) Navigate to Realm Settings, Make sure your realm is current, Go to "Login" tab, enable "User registration" and save changes
4) Create new client with OpenID Connect Type
5) Enable Standard flow, Implicit Flow and Direct acess grants
6) Set:
Root URL: http://localhost
   Valid Redirect URIs: http://localhost/*
   Web Origins: +
   ⚠️ If your API uses port 80, miss the port number in these fields.
7) Click Save to finish setup
8) Open Swagger, Enter your client id and Press "select all" in Authorize
```

## API Overview

##### (All Endpoints need that you were authenticated)

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

## Documentation:

### Authorization

1) Open Swagger
2) Click "Authorize" button in the upper right corner
3) Select All and Enter client ID ("microservice-bank-client")
4) After this, Swagger redirects you to Keycloak Login Page

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
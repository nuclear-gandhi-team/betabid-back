# betabid-back

This is backend part of Betabid online auction web application.

Fill free to check our deployed [Web API](https://betabid.azurewebsites.net/swagger/index.html)

API base path is https://betabid.azurewebsites.net/api/

# Technologies used:
- ASP.NET Core
- EF Core
- MS SQL Server
- Identity framework

N-Layered architecture was used, so the Solution is divided into 4 projects
- Betabid.Application
- Betabid.Domain
- Betabid.Persistence
- Betabid.WebAPI

Deployed to Azure.

# Contracts
Requests and responces to for each endpoint
## Lots

### /api/lots/create POST
Request (from form(!))
```js
{
  "name": "Sample Lot",
  "startPrice": 100.00,
  "description": "This is a sample description of the lot.",
  "dateStarted": "2024-02-10T00:00:00Z",
  "deadline": "2024-03-10T00:00:00Z",
  "betStep": 10.00,
  "ownerId": "owner123",
  "tagIds": [1, 2, 3]
}
```

Responce
```js
  None
```

### /api/lots/delete/{id} DELETE
Request
```js
  {id}
```

Responce
```js
  None
```

### /api/lots/get/{id} GET
Request
```js
  {id}
```

Responce
```js
{
  "id": 1,
  "title": "Lot Title",
  "images": [
    "image1.jpg",
    "image2.jpg"
  ],
  "tags": [
    "Tag1",
    "Tag2"
  ],
  "status": "Active",
  "description": "Description of the lot.",
  "dateStarted": "2024-02-10T00:00:00Z",
  "deadline": "2024-03-10T00:00:00Z",
  "startPrice": 100.00,
  "currentPrice": 150.00,
  "minNextPrice": 160.00,
  "activeBetsCount": 5,
  "activeUsersCount": 4,
  "minBetStep": 10.00,
  "ownerName": "Owner's Name",
  "bidHistory": [
    {
      "id": 1,
      "userId": "user123",
      "userName": "User Name",
      "amount": 110.00,
      "time": "2024-02-11T00:00:00Z"
    },
    {
      "id": 2,
      "userId": "user124",
      "userName": "Another User",
      "amount": 120.00,
      "time": "2024-02-12T00:00:00Z"
    }
  ],
  "isSaved": false
}

```

### /api/lots/get-all GET
Request (from query(!))
```js
{
  "nameStartsWith": "Sample",
  "priceOrder": "Ascending",
  "tags": ["Tag1", "Tag2"],
  "status": "Active",
  "page": 1,
  "pageCount": 10
}
```

Responce
```js
{
  {
    "id": 1,
    "title": "Lot Title",
    "deadline": "2024-03-10T00:00:00Z",
    "currentPrice": 150.00,
    "description": "Brief description of the lot.",
    "tags": [
      "Tag1",
      "Tag2"
    ],
    "status": "Active",
    "isSaved": false,
    "image": "lotImage.jpg"
  },
  ...
}
```

### /api/lots/delete/tags GET
Request
```js
---
```

Responce
```js
[
  "Tag1",
  "Tag2",
  ...
]
```

### /api/lots/delete/statuses GET
Request
```js
---
```

Responce
```js
[
  "Status1",
  "Status2",
  ...
]
```

## Users

### /api/users/login POST
Request
```js
{
  "login": "string",
  "password": "string"
}
```

Responce
```js
  JWT
```

### /api/users/register POST
Request
```js
{
  "login": "string",
  "email": "user@example.com",
  "password": "string",
  "confirmPassword": "string"
}
```

Responce
```js
  None
```

### /api/users/{userId} GET
Request
```js
{userId}
```

Responce
```js
{
  "id": "user123",
  "login": "userLogin",
  "email": "user@example.com",
  "balance": 1000.00,
  "savedLots": [
    {
      "id": 1,
      "title": "Lot 1",
      "startPrice": 100.00,
      "currentPrice": 150.00,
      "description": "Description of Lot 1",
      "dateStarted": "2024-02-10T00:00:00Z",
      "deadline": "2024-03-10T00:00:00Z",
      "betStep": 10.00,
      "ownerId": "owner123",
      "tagIds": [1, 2, 3],
      "status": "Active",
      "image": "image1.jpg"
    },
    ...
  ],
  "bets": [
    {
      "id": 1,
      "lotId": 1,
      "userId": "user123",
      "amount": 110.00,
      "time": "2024-02-11T00:00:00Z"
    },
    ...
  ]
}

```

### /api/users/update-user-data PUT
Request
```js
{
  "id": "string",
  "newName": "string",
  "newEmail": "user@example.com"
}
```

Responce
```js
  None
```

### /api/users/update-user-password PUT
Request
```js
{
  "id": "string",
  "oldPassword": "string",
  "newPassword": "string",
  "confirmNewPassword": "string"
}
```

Responce
```js
  None
```

### /api/users/delete/{id} DELETE
Request
```js
  {id}
```

Responce
```js
  None
```

### /api/users/delete/save POST
Request
```js
{
  "userId": "string",
  "lotId": 0
}
```

Responce
```js
  None
```

### /api/users/delete/user-lots GET
Request  (from query(!))
```js
{
  "nameStartsWith": "Sample",
  "priceOrder": "Ascending",
  "tags": ["Tag1", "Tag2"],
  "status": "Active",
  "page": 1,
  "pageCount": 10
}
```

Responce
```js
{
  {
    "id": 1,
    "title": "Lot Title",
    "deadline": "2024-03-10T00:00:00Z",
    "currentPrice": 150.00,
    "description": "Brief description of the lot.",
    "tags": [
      "Tag1",
      "Tag2"
    ],
    "status": "Active",
    "isSaved": false,
    "image": "lotImage.jpg"
  },
  ...
}
```

### /api/users/delete/user-lots POST
Request
```js
{
  "amount": 0,
  "lotId": 0
}
```

Responce
```js
  None
```

## Comments

### /api/comments POST
Request (From query(!))
```js
{ lotId }
```

Responce
```js
[
  {
    "id": "1",
    "userId": "2a22882e-2b64-4a06-8ecd-c5ae06d42dd5",
    "userName": "string",
    "body": "this is some kind of comment",
    "childComments": [
      {
        "id": "2",
        "userId": "2a22882e-2b64-4a06-8ecd-c5ae06d42dd5",
        "userName": "string",
        "body": "this is some kind of reply comment",
        "childComments": []
      }
    ]
  }
]
```

### /api/comments/{lotId} GET
Request (From query(!))
```js
{
  "lotId": 0,
  "parentCommentId": 0,
  "body": "string"
}
```

Responce
```js
  None
```

## Database structure
![image](https://github.com/nuclear-gandhi-team/betabid-back/assets/58270142/7c36a30c-3c07-4ee1-bab6-37ccb338b5c1)
https://drawsql.app/teams/just-team-5/diagrams/betabid
<br>
Tables Users and Jwt is for demonstration only. Identity framework will populate DB with this.

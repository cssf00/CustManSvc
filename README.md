# Prerequisite
install .NET Core 3.1

# CustManSvc.API
ASP.NET Core 3.1 WebAPI service to manage/CRUD customer details

- Development tool used is VSCode
- To run the solution, on powershell, cd to CustManSvc.API directory and execute "dotnet run"
- To access swagger UI/Open API, on your local browser navigates to https://localhost:5001/swagger
- To call the endpoints you can use PostMan
- For details on the endpoints and how to call please refer to swagger UI as a guide: https://localhost:5001/swagger

## Create a customer, note the dateOfBirth must be in RFC3339 format in UTC timezone, e.g. "2000-02-02T00:00:00Z"
POST http://localhost:5000/api/customers

Request body:
```json
{
	"id": 0,
	"firstName": "James",
	"lastName": "Tilley",
	"dateOfBirth": "2000-02-02T00:00:00Z"
}
```

## Get the created customer
GET http://localhost:5000/api/customers/1

Response body:
```json
{
    "id": 1,
    "firstName": "James",
    "lastName": "Tilley",
    "dateOfBirth": "2000-02-02T00:00:00Z"
}
```

# CustManSvc.API.Tests
Contains integration tests to test all endpoints of CustManSvc.API
- using TestServer and InMemory database

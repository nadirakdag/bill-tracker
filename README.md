
# Bill Tracker

This is a test project to use while learning docker, Kubernetes, docker-compose.

It is pretty simple, it has two end-point one is info that supports only the GET function and returns API information. 
Other Bill end-point it has GET, POST, PUT and Delete end-points.

The project contains, Serilog file logging with CompactJsonFormatter. 
Datadog Tracker for trace requests in DataDog.
Swagger for API documentation, and testing purposes.

It is developed with [Clean Architecture](https://github.com/jasontaylordev/CleanArchitecture)
 approach and .NET 5

It uses MediatR, AutoMapper, EntityFrameworkCore with Postgresql.

## Lessons Learned

[How to Create Small and Secure Docker Container for .Net Applications](CREATE-SMALL-AND-SECURE-CONTAINERS.md)
[How to Create Docker Compose File](DOCKER-COMPOSE.md)

## Run Locally

Clone the project

```bash
  git clone https://github.com/nadirakdag/bill-tracker
```

Go to the project directory

```bash
  cd bill-tracker
```

Install dependencies

```bash
  dotnet restore
```

Install Postgresql

```bash
  docker run --name some-postgres -e POSTGRES_PASSWORD=mysecretpassword -p 5432:5432 -d postgres
```

Start the server

```bash
  dotnet run --project src/API/API.csproj
```
  
## Running Tests

To run tests, run the following command

```bash
  dotnet test BillTracker.sln
```

  
## API Reference

#### Get API Info

```http
  GET /api/info
```

#### Get Bills 

```http
  GET /api/bill
```

#### Get Bill by id

```http
  GET /api/bill/{id}
```

| Parameter | Type     | Description                       |
| :-------- | :------- | :-------------------------------- |
| `id`      | `string` | **Required**. Id of item to fetch |


#### Create Bill

```http
  POST /api/bill
```

| Body | Type     | Description                       | Location |
| :-------- | :------- | :-------------------------------- | :------ |
| `Bill Object` | `Json` | **Required**. Bill information to create | Body |


#### Update Bill

```http
  PUT /api/bill/{id}
```
| Parameter | Type     | Description                       | Location |
| :-------- | :------- | :-------------------------------- | :------ |
| `id`      | `string` | **Required**. Id of item to update | QueryString |
| `Bill Object` | `Json` | **Required**. Bill information to create | Body |

#### Delete Bill

```http
  DELETE /api/bill/{id}
```

| Parameter | Type     | Description                       | Location |
| :-------- | :------- | :-------------------------------- | :------  |
| `id`      | `string` | **Required**. Id of item to delete | QueryString |

 

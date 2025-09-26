# CRUD Code Test

This is a .NET application for managing customer data using DDD, TDD, BDD, Clean Architecture, and CQRS with Event Sourcing.

## Prerequisites
- .NET 7+ SDK
- Docker (for PostgreSQL database)
- Entity Framework Core CLI tools: Install with `dotnet tool install --global dotnet-ef`

## How to Run the Project
Follow these steps to set up and run the application:

1. **Start the Database with Docker**:
   Run the following command from the project root to start PostgreSQL:
`docker compose up -d`

2. **Apply Database Migrations**:
`dotnet ef migrations add InitialCreate`
`dotnet ef database update`

4. **Build and Run the Application**:
`dotnet build`
`dotnet run --project src/Presentation`

5. **Access the Application**:
The API will be available at:
- http://localhost:port

5. **Test with Swagger**:
Open your browser and navigate to:
- http://localhost:port/swagger/index.html

## Available Endpoints
- POST /api/customers - Create a new customer
- PUT /api/customers/{id} - Update an existing customer
- DELETE /api/customers/{id} - Delete a customer
- GET /api/customers/{id} - Get customer details

## Running Tests
- Run unit tests: `dotnet test`
- BDD tests are included for all customer operations

## Project Structure
- src/Domain - Entities, Events, Exceptions
- src/Application - CQRS Command & Query Handlers, Validation
- src/Infrastructure - EF Core, Repositories, EventStore
- src/Presentation - API Endpoints
- tests/UnitTests - TDD tests
- tests/AcceptanceTests - BDD tests
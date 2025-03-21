# Job Application Tracker

![.NET](https://img.shields.io/badge/.NET-7.0-512BD4) ![EF Core](https://img.shields.io/badge/EF_Core-7.0-purple) ![React](https://img.shields.io/badge/React-18.2-61DAFB)

A full-stack application built with ASP.NET Core and React to help you track and manage your job applications.

![Project Screenshot](screenshot.png)

## Project Architecture

This application follows a clean architecture pattern with clear separation of concerns:

- **Domain Layer**: Contains business entities and business logic
- **Application Layer**: Contains application use cases and services
- **Infrastructure Layer**: Contains implementations for persistence and external services
- **API Layer**: Contains controllers and API endpoints
- **React Frontend**: Contains the UI components and services

## Technologies Used

### Backend
- ASP.NET Core 7.0 Web API
- Entity Framework Core with SQLite (configurable to use in-memory for testing)
- AutoMapper for object mapping
- FluentValidation for input validation
- Serilog for comprehensive logging
- Swagger for API documentation

### Frontend
- React 18 with React Hook Form
- Axios for API communication
- React Router for client-side routing
- CSS3 for styling

### DevOps
- Docker + Docker Compose for containerization
- xUnit for backend testing
- FluentAssertions for more readable assertions

## How to Run

### Run Backend

```bash
cd JobApplicationTracker.API
dotnet run
```

OR with Docker:

```bash
docker build -t jobapptracker-api .
docker run -p 5000:80 jobapptracker-api
```

### Run Frontend

```bash
cd frontend
npm install
npm start
```

OR with Docker:

```bash
docker build -t jobapptracker-frontend .
docker run -p 3000:3000 jobapptracker-frontend
```

### Using Docker Compose (recommended)

```bash
docker-compose up --build
```

## API Endpoints

| Method | Endpoint                       | Description                                   |
|--------|--------------------------------|-----------------------------------------------|
| GET    | /api/applications              | List all applications (with pagination)       |
| GET    | /api/applications/{id}         | Get application details                       |
| POST   | /api/applications              | Add new application                           |
| PUT    | /api/applications/{id}         | Update application                            |
| PATCH  | /api/applications/{id}/status  | Update only the status of an application      |
| DELETE | /api/applications/{id}         | Delete an application                         |
| GET    | /api/applications/statistics   | Get application statistics                    |

## Assumptions

- SQLite database is used by default in production; easily switchable to In-memory database for testing.
- No authentication/authorization is implemented for this demo.
- Frontend is configured to connect to the backend at `https://localhost:7000/api` by default.

## Improvements for Production

- Add JWT authentication/authorization
- Move from SQLite to SQL Server/PostgreSQL for better performance and reliability
- Add CI/CD pipeline with GitHub Actions
- Implement extensive logging and monitoring
- Add unit and integration tests for frontend
- Add caching layer for better performance

## Testing

Run backend tests:

```bash
dotnet test
```

# Step-by-Step Guide to Run the Job Application Tracker Locally

## Prerequisites
- [.NET 7.0 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/7.0) installed
- [Node.js](https://nodejs.org/) (v16 or higher) and npm installed
- Git installed (optional, for cloning the repository)

## Troubleshooting

1. **HTTPS Certificate Issues**:
   - If you encounter HTTPS certificate issues with the backend, you may need to trust the development certificate:
     ```bash
     dotnet dev-certs https --trust
     ```

2. **CORS Issues**:
   - If the frontend cannot connect to the backend, check the CORS configuration in `Program.cs` and ensure your frontend URL is allowed.

3. **Database Issues**:
   - By default, the application uses SQLite. Make sure the application has write permissions to create the database file.
   - You can switch to an in-memory database by setting `UseInMemoryDatabase` to `true` in `appsettings.json`.

4. **Frontend Build Issues**:
   - If you encounter problems with the React build, try:
     ```bash
     npm cache clean --force
     rm -rf node_modules
     npm install
     ```

## Using Docker (Alternative)

If you have Docker installed, you can use Docker Compose to run both the frontend and backend:

1. **Build and run with Docker Compose**:
   ```bash
   docker-compose up --build
   ```

2. **Access the application**:
   - The frontend will be available at: http://localhost:3000
   - The backend API will be available at: http://localhost:5000
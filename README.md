# Hospital Appointment Booking System

A full-stack hospital appointment booking system built with **ASP.NET Core** (Clean Architecture) and **React** (Vite + TypeScript + Tailwind).

## Features

- Role-based access: Admin, Receptionist, Doctor, Patient
- Department and doctor management with weekly schedules
- Patient registration and appointment booking with slot validation
- Consultation workflow: check-in, diagnosis, prescriptions, completion
- JWT authentication, structured logging (Serilog), paginated list APIs
- Integration tests covering business rules and end-to-end flows

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Node.js 20+](https://nodejs.org/)
- SQL Server or **LocalDB** (default for development)

## Quick Start

### 1. Database & API

```bash
# From the solution root
dotnet restore
dotnet ef database update --project HospitalSystem.Infrastructure --startup-project HospitalSystem.API
dotnet run --project HospitalSystem.API --launch-profile http
```

The API runs at **http://localhost:5000**. Swagger UI: **http://localhost:5000/swagger**

On first run, migrations apply automatically and the database is seeded with demo users.

### 2. React Frontend

```bash
cd HospitalSystem.React
npm install
npm run dev
```

Open **http://localhost:5173**

Create `HospitalSystem.React/.env.local` (already included):

```
VITE_API_URL=http://localhost:5000/api
```

### 3. Run Tests

```bash
dotnet test HospitalSystem.IntegrationTests/HospitalSystem.IntegrationTests.csproj
```

## Seeded Credentials

| Role         | Email                  | Password       |
|--------------|------------------------|----------------|
| Admin        | admin@hospital.com     | Admin@123      |
| Receptionist | reception@hospital.com | Reception@123  |
| Doctor       | dr.smith@hospital.com  | Doctor@123     |
| Patient      | patient@hospital.com   | Patient@123    |

## Configuration

### Development

`appsettings.Development.json` — LocalDB connection, verbose Serilog (console + `logs/hospital-.log`).

### Production

Set environment variables:

| Variable | Description |
|----------|-------------|
| `DB_CONNECTION_STRING` | SQL Server connection string |
| `JWT_SECRET` | JWT signing key (32+ characters) |
| `ASPNETCORE_ENVIRONMENT` | `Production` |

`appsettings.Production.json` provides Serilog file logging defaults; secrets come from environment variables.

### React

| Variable | Description |
|----------|-------------|
| `VITE_API_URL` | Backend API base URL (default: `http://localhost:5000/api`) |

## Project Structure

```
HospitalSystem.Domain/          # Entities, enums, repository interfaces
HospitalSystem.Application/     # DTOs, services, validators
HospitalSystem.Infrastructure/  # EF Core, JWT, repositories, seeding
HospitalSystem.API/               # REST controllers, middleware
HospitalSystem.IntegrationTests/  # xUnit + WebApplicationFactory
HospitalSystem.React/             # Vite React SPA
```

## API Pagination

List endpoints accept `?page=1&pageSize=10` and return:

```json
{
  "success": true,
  "data": {
    "data": [],
    "totalCount": 0,
    "page": 1,
    "pageSize": 10,
    "totalPages": 0
  }
}
```

Applies to: `/api/departments`, `/api/doctors`, `/api/patients`, `/api/receptionists`.

## Logging

- **Serilog** writes to console and rolling files under `logs/hospital-YYYYMMDD.log`
- Every HTTP request is logged with method, path, status code, and duration
- Appointment book/reschedule/cancel/check-in actions are logged with actor and entity IDs

## License

MIT (portfolio / educational project)

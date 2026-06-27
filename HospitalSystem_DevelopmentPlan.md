# 🏥 Hospital Appointment Booking System
## Complete Development Plan — Cursor / Windsurf Edition
> **Stack:** ASP.NET Core Web API · Entity Framework Core · SQL Server · React
> **Architecture:** Clean Architecture (Monolithic)
> **Goal:** Production-quality portfolio project

---

## 📐 Architecture Overview

```
Solution/
├── HospitalSystem.Domain          # Entities, Enums, Interfaces, Domain Events
├── HospitalSystem.Application     # Use Cases, DTOs, Interfaces, Validators
├── HospitalSystem.Infrastructure  # EF Core, Repositories, Auth, External Services
├── HospitalSystem.API             # Controllers, Middleware, DI Registration
└── HospitalSystem.React           # React Frontend (Vite + TypeScript)
```

---

## 🗄️ Database Schema (Full)

### Tables

```sql
-- Identity
Users (Id, Email, PasswordHash, Role [Admin|Receptionist|Doctor|Patient], IsActive, CreatedAt)

-- Core
Departments (Id, Name, Description, IsActive)

Doctors (Id, UserId→Users, DepartmentId→Departments, FullName, 
         Specialization, Phone, IsActive)

DoctorSchedules (Id, DoctorId→Doctors, DayOfWeek [0-6], 
                 StartTime, EndTime, AppointmentDurationMinutes, IsActive)

Patients (Id, UserId→Users [nullable], FullName, DateOfBirth, Gender,
          Phone, Email, Address, NationalId, BloodType, CreatedAt)

Appointments (Id, PatientId→Patients, DoctorId→Doctors, 
              AppointmentDate, StartTime, EndTime,
              Status [Confirmed|CheckedIn|InProgress|Completed|Cancelled|NoShow],
              Notes, CreatedByReceptionistId→Users, CreatedAt, UpdatedAt)

Consultations (Id, AppointmentId→Appointments, DoctorId→Doctors, 
               PatientId→Patients, Diagnosis, Notes, StartedAt, CompletedAt)

Prescriptions (Id, ConsultationId→Consultations, MedicationName, 
               Dosage, Frequency, Duration, Notes)

Receptionists (Id, UserId→Users, FullName, Phone, IsActive)
```

---

## 👤 Roles & Permissions Matrix

| Action | Admin | Receptionist | Doctor | Patient |
|--------|-------|-------------|--------|---------|
| Manage Departments | ✅ | ❌ | ❌ | ❌ |
| Manage Doctors | ✅ | ❌ | ❌ | ❌ |
| Manage Receptionists | ✅ | ❌ | ❌ | ❌ |
| Configure Doctor Schedules | ✅ | ❌ | ❌ | ❌ |
| Register Patients | ❌ | ✅ | ❌ | ❌ |
| Book Appointments | ❌ | ✅ | ❌ | ❌ |
| Cancel/Reschedule | ❌ | ✅ | ❌ | ❌ |
| Check-in Patients | ❌ | ✅ | ❌ | ❌ |
| View Own Schedule | ❌ | ❌ | ✅ | ❌ |
| Start Consultation | ❌ | ❌ | ✅ | ❌ |
| Add Diagnosis/Prescription | ❌ | ❌ | ✅ | ❌ |
| View Own History | ❌ | ❌ | ❌ | ✅ |

---

## 📋 PHASE 1 — Solution Setup & Domain Layer
**Prompt to Cursor:** *"I'm building a Hospital Appointment Booking System using Clean Architecture with ASP.NET Core. Set up the solution structure."*

### Tasks:

**1.1 — Create Solution Structure**
```
Prompt: "Create a .NET 8 solution named HospitalSystem with 4 projects:
- HospitalSystem.Domain (Class Library)
- HospitalSystem.Application (Class Library, references Domain)
- HospitalSystem.Infrastructure (Class Library, references Application)
- HospitalSystem.API (ASP.NET Core Web API, references Infrastructure)
Add all project references correctly."
```

**1.2 — Domain Entities**
```
Prompt: "In HospitalSystem.Domain/Entities, create the following C# entity classes 
with proper encapsulation (private setters, factory methods):
- User.cs (Id, Email, PasswordHash, Role enum, IsActive, CreatedAt)
- Department.cs (Id, Name, Description, IsActive)
- Doctor.cs (Id, UserId, DepartmentId, FullName, Specialization, Phone, IsActive)
- DoctorSchedule.cs (Id, DoctorId, DayOfWeek, StartTime, EndTime, AppointmentDurationMinutes, IsActive)
- Patient.cs (Id, UserId nullable, FullName, DateOfBirth, Gender, Phone, Email, Address, NationalId, BloodType, CreatedAt)
- Appointment.cs (Id, PatientId, DoctorId, AppointmentDate, StartTime, EndTime, Status, Notes, CreatedByReceptionistId, CreatedAt, UpdatedAt)
- Consultation.cs (Id, AppointmentId, DoctorId, PatientId, Diagnosis, Notes, StartedAt, CompletedAt)
- Prescription.cs (Id, ConsultationId, MedicationName, Dosage, Frequency, Duration, Notes)
- Receptionist.cs (Id, UserId, FullName, Phone, IsActive)
Add navigation properties and include an AppointmentStatus enum with values: Confirmed, CheckedIn, InProgress, Completed, Cancelled, NoShow"
```

**1.3 — Domain Interfaces**
```
Prompt: "In HospitalSystem.Domain/Interfaces, create repository interfaces:
- IRepository<T> with methods: GetByIdAsync, GetAllAsync, AddAsync, UpdateAsync, DeleteAsync
- IAppointmentRepository extends IRepository<Appointment> with:
  - GetByDoctorAndDateAsync(doctorId, date)
  - IsSlotAvailableAsync(doctorId, date, startTime, endTime)
  - GetByPatientIdAsync(patientId)
- IDoctorRepository extends IRepository<Doctor> with GetByDepartmentAsync
- IUnitOfWork with properties for each repository + SaveChangesAsync"
```

---

## 📋 PHASE 2 — Application Layer (Use Cases + DTOs)
**Prompt to Cursor:** *"Now implement the Application layer with CQRS-style Use Cases."*

### Tasks:

**2.1 — DTOs**
```
Prompt: "In HospitalSystem.Application/DTOs, create request/response DTOs for:
Auth: LoginRequest, LoginResponse (with JWT token), RegisterRequest
Department: CreateDepartmentRequest, UpdateDepartmentRequest, DepartmentResponse
Doctor: CreateDoctorRequest, UpdateDoctorRequest, DoctorResponse, DoctorScheduleRequest
Patient: CreatePatientRequest, UpdatePatientRequest, PatientResponse
Appointment: CreateAppointmentRequest (patientId, doctorId, date, startTime, notes), 
             UpdateAppointmentRequest, AppointmentResponse (include doctor name, patient name, status),
             RescheduleRequest (newDate, newStartTime)
Consultation: StartConsultationRequest, AddDiagnosisRequest, CreatePrescriptionRequest, ConsultationResponse
Receptionist: CreateReceptionistRequest, ReceptionistResponse"
```

**2.2 — Validators (FluentValidation)**
```
Prompt: "Install FluentValidation in Application layer. Create validators:
- CreateAppointmentValidator: date cannot be in the past, doctorId required, patientId required
- CreateDoctorValidator: name required, specialization required, departmentId required
- CreatePatientValidator: name required, phone required, DateOfBirth valid
- DoctorScheduleValidator: StartTime < EndTime, AppointmentDurationMinutes between 10 and 120
- RescheduleValidator: newDate cannot be in the past"
```

**2.3 — Services Interfaces**
```
Prompt: "In HospitalSystem.Application/Interfaces, create service interfaces:
- IAuthService: LoginAsync(request), RegisterAsync(request)
- IAppointmentService:
    BookAppointmentAsync(request, receptionistId)
    RescheduleAsync(appointmentId, request, receptionistId)
    CancelAsync(appointmentId, receptionistId)
    CheckInAsync(appointmentId)
    GetAvailableSlotsAsync(doctorId, date)
    GetDoctorScheduleAsync(doctorId, date)
- IDoctorService: CRUD + AssignToDepartmentAsync + SetScheduleAsync
- IPatientService: CRUD + SearchAsync(query)
- IDepartmentService: CRUD
- IConsultationService: StartAsync, AddDiagnosisAsync, AddPrescriptionAsync, CompleteAsync"
```

**2.4 — Service Implementations (Application Layer)**
```
Prompt: "Implement AppointmentService in HospitalSystem.Application/Services:
- BookAppointmentAsync must:
  1. Validate appointment is not in the past
  2. Check doctor schedule exists for that day of week
  3. Verify start time is within doctor's working hours
  4. Check no double booking using IsSlotAvailableAsync
  5. Calculate EndTime = StartTime + doctor's AppointmentDurationMinutes
  6. Create appointment with Status = Confirmed
  7. Save using IUnitOfWork
- CancelAsync: set status to Cancelled (slot becomes available)
- RescheduleAsync: cancel old, create new with same validation
- GetAvailableSlotsAsync: generate all slots for the day from schedule, filter out booked ones"
```

---

## 📋 PHASE 3 — Infrastructure Layer
**Prompt to Cursor:** *"Implement the Infrastructure layer: EF Core, Repositories, Auth."*

### Tasks:

**3.1 — EF Core DbContext**
```
Prompt: "In HospitalSystem.Infrastructure, install:
- Microsoft.EntityFrameworkCore.SqlServer
- Microsoft.EntityFrameworkCore.Tools

Create HospitalDbContext with DbSets for all entities.
Configure using Fluent API in separate configuration classes (IEntityTypeConfiguration<T>):
- AppointmentConfiguration: index on (DoctorId, AppointmentDate, StartTime), Status as string
- DoctorScheduleConfiguration: unique index on (DoctorId, DayOfWeek)
- UserConfiguration: unique index on Email
Set all string lengths, required fields, and cascade delete behaviors."
```

**3.2 — Repositories**
```
Prompt: "Implement all repositories in HospitalSystem.Infrastructure/Repositories:
- GenericRepository<T> implementing IRepository<T> using HospitalDbContext
- AppointmentRepository: implement IsSlotAvailableAsync using a query that checks 
  for overlapping appointments where Status is not Cancelled
- DoctorRepository: GetByDepartmentAsync with Include(d => d.Department)
Implement UnitOfWork wrapping all repositories."
```

**3.3 — JWT Authentication**
```
Prompt: "Implement JWT authentication in Infrastructure:
- Install Microsoft.AspNetCore.Authentication.JwtBearer
- Create JwtService implementing IAuthService
- LoginAsync: validate credentials, generate JWT with claims: userId, email, role
- JWT config in appsettings.json: Secret, Issuer, Audience, ExpiryDays
- Create AuthService that uses UserRepository + JwtService
- Add extension method AddInfrastructure(this IServiceCollection) that registers all services and repositories"
```

**3.4 — Database Seed**
```
Prompt: "Create a DatabaseSeeder class that seeds:
- 1 Admin user (admin@hospital.com / Admin@123)
- 2 Departments (Cardiology, Orthopedics)
- 2 Doctors with schedules (Mon-Fri, 09:00-17:00, 30-min slots)
- 1 Receptionist user
- 3 sample patients
Run seeder in Program.cs using app.Services scope on startup (only if DB is empty)."
```

---

## 📋 PHASE 4 — API Layer (Controllers + Middleware)
**Prompt to Cursor:** *"Build the API layer with controllers, middleware, and authorization."*

### Tasks:

**4.1 — Middleware**
```
Prompt: "Create global exception handling middleware in HospitalSystem.API/Middleware:
- Catch all unhandled exceptions
- Return standardized JSON: { success: false, message: string, errors: [] }
- Log errors using ILogger
- Return 400 for ValidationException, 401 for UnauthorizedException, 
  403 for ForbiddenException, 404 for NotFoundException, 500 for others
Create custom exception classes in Application layer: NotFoundException, 
ValidationException, UnauthorizedException, ForbiddenException."
```

**4.2 — Auth Controller**
```
Prompt: "Create AuthController with:
POST /api/auth/login → returns JWT token
POST /api/auth/register (Admin only) → register new user
GET /api/auth/me → return current user info from JWT claims
All responses use ApiResponse<T> wrapper: { success, data, message }"
```

**4.3 — Admin Controllers**
```
Prompt: "Create these controllers (all require [Authorize(Roles = 'Admin')]):
DepartmentsController: GET /api/departments, GET /api/departments/{id}, 
  POST /api/departments, PUT /api/departments/{id}, DELETE /api/departments/{id}
DoctorsController: CRUD + POST /api/doctors/{id}/schedule
ReceptionistsController: CRUD
All return ApiResponse<T>."
```

**4.4 — Receptionist Controllers**
```
Prompt: "Create these controllers (require [Authorize(Roles = 'Receptionist')]):
PatientsController:
  GET /api/patients (with search query param)
  GET /api/patients/{id}
  POST /api/patients
  PUT /api/patients/{id}
AppointmentsController:
  GET /api/appointments?doctorId=&date= (view schedule)
  GET /api/appointments/available-slots?doctorId=&date=
  POST /api/appointments (book)
  PUT /api/appointments/{id}/reschedule
  PUT /api/appointments/{id}/cancel
  PUT /api/appointments/{id}/checkin
  GET /api/appointments/{id}"
```

**4.5 — Doctor Controller**
```
Prompt: "Create DoctorController (require [Authorize(Roles = 'Doctor')]):
GET /api/doctor/schedule?date= → view own appointments for date
GET /api/doctor/appointments/{id} → view appointment + patient info
POST /api/doctor/appointments/{id}/start → set status InProgress, create Consultation
PUT /api/doctor/appointments/{id}/diagnosis → add diagnosis to consultation
POST /api/doctor/appointments/{id}/prescriptions → add prescription
PUT /api/doctor/appointments/{id}/complete → set status Completed"
```

**4.6 — Patient Controller**
```
Prompt: "Create PatientController (require [Authorize(Roles = 'Patient')]):
GET /api/patient/profile → own profile
GET /api/patient/appointments → appointment history
GET /api/patient/appointments/{id} → appointment detail with diagnosis and prescriptions"
```

**4.7 — Program.cs Setup**
```
Prompt: "Configure Program.cs with:
- AddInfrastructure() extension
- JWT Authentication with validation parameters
- Role-based Authorization
- FluentValidation from Application assembly
- Swagger with JWT Bearer support (add Authorize button)
- CORS policy allowing React dev server (http://localhost:5173)
- Global exception middleware
- Database seeder call
- app.UseAuthentication() before app.UseAuthorization()"
```

---

## 📋 PHASE 5 — React Frontend
**Prompt to Cursor:** *"Build the React frontend using Vite + TypeScript + Tailwind."*

### Tasks:

**5.1 — Project Setup**
```
Prompt: "Create React app in /HospitalSystem.React using:
- Vite + TypeScript
- Tailwind CSS v3
- React Router DOM v6
- Axios for API calls
- React Query (TanStack Query) for server state
- React Hook Form + Zod for forms
- Lucide React for icons
Configure Axios base URL to http://localhost:5000/api.
Create axios interceptor that attaches JWT token from localStorage to all requests,
and redirects to /login on 401 response."
```

**5.2 — Auth & Layout**
```
Prompt: "Implement authentication flow:
- AuthContext: stores user info + token, login/logout functions
- PrivateRoute component: checks auth + role, redirects if unauthorized
- Login page: email/password form, calls POST /api/auth/login, stores token
- Layout components per role: AdminLayout, ReceptionistLayout, DoctorLayout
  Each with sidebar navigation showing only relevant links
- Protected routes setup in App.tsx:
  /admin/* → AdminLayout (role: Admin)
  /receptionist/* → ReceptionistLayout (role: Receptionist)
  /doctor/* → DoctorLayout (role: Doctor)
  /patient/* → PatientLayout (role: Patient)"
```

**5.3 — Admin Pages**
```
Prompt: "Create Admin section pages:
1. /admin/departments — list with Add/Edit/Delete (modal forms)
2. /admin/doctors — list with Add/Edit/Delete + Assign Department dropdown
3. /admin/doctors/{id}/schedule — weekly schedule configurator (checkboxes per day, time pickers)
4. /admin/receptionists — list with Add/Edit/Delete
Use React Query for data fetching. Use React Hook Form + Zod for all forms.
Show loading skeletons and error states."
```

**5.4 — Receptionist Pages**
```
Prompt: "Create Receptionist section pages:
1. /receptionist/patients — searchable list, Register Patient button
2. /receptionist/patients/{id} — patient profile with appointment history
3. /receptionist/patients/new — Register Patient form
4. /receptionist/appointments — book appointment:
   - Step 1: Select Department → Doctor → Date
   - Step 2: Show available time slots as clickable cards
   - Step 3: Confirm booking
5. /receptionist/schedule — today's appointment list for all doctors
   with Check-In button per appointment + status badges
6. /receptionist/appointments/{id} — detail with Cancel / Reschedule actions"
```

**5.5 — Doctor Pages**
```
Prompt: "Create Doctor section pages:
1. /doctor/schedule — calendar showing today's appointments, click to view detail
2. /doctor/appointments/{id} — consultation view:
   - Patient info panel (name, DOB, blood type)
   - Start Consultation button → unlocks Diagnosis form
   - Add Diagnosis textarea
   - Add Prescriptions (medication, dosage, frequency, duration) — multiple rows
   - Complete Appointment button
   - Show appointment status badge"
```

**5.6 — Patient Pages**
```
Prompt: "Create Patient section pages:
1. /patient/profile — view profile info
2. /patient/appointments — history list with status badges
3. /patient/appointments/{id} — detail showing:
   - Appointment info (doctor, date, time)
   - Diagnosis (if completed)
   - Prescriptions list (if any)"
```

**5.7 — Shared Components**
```
Prompt: "Create reusable components:
- StatusBadge: colored badge per AppointmentStatus 
  (Confirmed=blue, CheckedIn=yellow, InProgress=orange, Completed=green, Cancelled=red, NoShow=gray)
- DataTable: sortable table with loading/empty states
- Modal: accessible modal with close on backdrop click + Escape key
- ConfirmDialog: 'Are you sure?' modal for delete/cancel actions
- ApiResponse handler hook: useApiMutation wrapping React Query useMutation 
  with toast notifications on success/error"
```

---

## 📋 PHASE 6 — Integration & Business Rules Enforcement

### Tasks:

**6.1 — End-to-End Test Scenarios**
```
Prompt: "Help me manually test these flows using Swagger UI:
1. Login as Admin → Create Department → Create Doctor → Set Schedule
2. Login as Receptionist → Register Patient → Book Appointment (check validation: past date should fail, double booking should fail)
3. Login as Receptionist → Check-In patient
4. Login as Doctor → Start Consultation → Add Diagnosis → Add Prescription → Complete
5. Login as Patient → View appointment history with diagnosis
Document any errors found."
```

**6.2 — Business Rules Verification**
```
Prompt: "Add integration tests or verify these business rules work:
- Booking past appointments returns 400 with clear message
- Double booking returns 400 'Slot not available'
- Doctor booking endpoint returns 403 Forbidden
- Patient booking endpoint returns 403 Forbidden
- Cancelled appointment slot appears in available slots again
- Available slots respect doctor's working hours exactly"
```

---

## 📋 PHASE 7 — Polish & Production Readiness

### Tasks:

**7.1 — Pagination & Filtering**
```
Prompt: "Add pagination to all list endpoints:
- Accept ?page=1&pageSize=10 query params
- Return PagedResponse<T>: { data: [], totalCount, page, pageSize, totalPages }
- Update React tables to show pagination controls"
```

**7.2 — Audit & Logging**
```
Prompt: "Add Serilog for structured logging:
- Log to Console + File (logs/hospital-.log)
- Log every API request: method, path, status code, duration
- Log all appointment actions with who did it and when
- Add CreatedAt, UpdatedAt, CreatedBy to all entities via BaseEntity"
```

**7.3 — API Documentation**
```
Prompt: "Enhance Swagger documentation:
- Add XML comments to all controllers and DTOs
- Group endpoints by tag (Admin, Receptionist, Doctor, Patient, Auth)
- Document all response codes (200, 400, 401, 403, 404, 500) per endpoint
- Add example request/response bodies"
```

**7.4 — Environment Configuration**
```
Prompt: "Set up configuration for different environments:
- appsettings.Development.json: local SQL Server, verbose logging
- appsettings.Production.json: production connection string from env vars
- Use environment variables for: DB_CONNECTION_STRING, JWT_SECRET
- Add .env.local for React: VITE_API_URL
- Create README.md with setup instructions (DB migrations, seeding, frontend start)"
```

---

## 🗺️ Recommended Execution Order

```
Phase 1 → Phase 2 → Phase 3 → Phase 4 → Test API with Swagger → Phase 5 → Phase 6 → Phase 7
```

| Phase | Estimated Sessions | Milestone |
|-------|-------------------|-----------|
| 1 — Domain | 1 session | Solution compiles, entities defined |
| 2 — Application | 2 sessions | Business logic & validation ready |
| 3 — Infrastructure | 2 sessions | DB migrations run, seed data works |
| 4 — API | 2 sessions | All endpoints working in Swagger |
| 5 — React | 3 sessions | Full UI functional |
| 6 — Integration | 1 session | All business rules enforced |
| 7 — Polish | 1 session | Production-ready |

---

## ⚠️ Important Notes for Cursor/Windsurf

1. **One phase at a time** — paste the phase prompt, let it finish, then move to next.
2. **After Phase 3**, run `dotnet ef migrations add InitialCreate` and `dotnet ef database update`.
3. **Always test with Swagger** before starting the React phase.
4. **If Cursor loses context**, re-paste the Architecture Overview at the top of each new conversation.
5. **Naming conventions**: Use PascalCase for C# classes, camelCase for React components props, kebab-case for routes.

---

*Plan version 1.0 — Hospital Appointment Booking System*

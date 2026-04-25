# Clinic Microservices Architecture Baseline (Implementation)

## Purpose
This document is the implementation baseline for the clinic system architecture. It aligns service boundaries with all approved user stories and resolves previously identified ownership and integration conflicts.

## Service Boundaries and Story Ownership

- **MS-01 Identity & Access Service**
  - Owns authentication, token/session lifecycle, security policy, and authorization policy source of truth.
  - Stories: `US-042`, `US-043`, `US-044`, `US-045`, policy ownership for `US-006`.

- **MS-02 Configuration Service**
  - Owns global runtime configuration (working hours, consultation duration, reminder timing, configurable operational policies).
  - Stories: `US-007`.

- **MS-03 Unified Clinic Operations Service**
  - Owns setup, operations, consultation lifecycle, medical records, billing/payments, and reporting domains as one consolidated business service.
  - Stories: `US-001..US-041`.

- **MS-10 Notification Service**
  - Owns outbound patient/staff notification dispatch, retries, deduplication, and logs.
  - Stories: `US-034..US-037`.

## Canonical Cross-Service Rules

- **RBAC ownership rule**
  - Authorization policy and permission matrix are owned only by `MS-01`.
  - Other services enforce authorization by consuming JWT claims and policy decisions from `MS-01`.

- **Data ownership rule**
  - Each service owns its database and does not directly read/write another service database.
  - Cross-service data exchange is via API contracts or domain events.

- **Audit rule**
  - Security-sensitive and business-critical state transitions must emit auditable events and be queryable through audit capabilities.

## Canonical US-014 Room Assignment Logic

When an appointment is confirmed, room assignment executes atomically within the booking transaction:

1. Try doctor's fixed default room (if available and department-valid).
2. If unavailable, assign least-recently-used available room in the same department.
3. If none available, create appointment as unassigned (`roomId = null`, `roomAssigned = false`) and raise receptionist alert.

Additional mandatory behaviors:
- Cancellation or reschedule must release room occupancy immediately for the slot.
- Receptionist may manually override room assignment before consultation starts.
- Setting a room to maintenance with future bookings must trigger reassignment flow and alerts.
- Assignment, reassignment, override, and release actions must be audit-logged.

### Superseded Rule Clarification

The following legacy wording is superseded and must not be implemented:
- "Room-type preference first, then least-recently-used."

Authoritative precedence is:
- Doctor fixed default room first.
- Same-department least-recently-used fallback second.
- Unassigned with receptionist alert third.

## Required Event Contracts

- **From Unified Clinic Operations (MS-03)**
  - `AppointmentBooked`
  - `AppointmentRescheduled`
  - `AppointmentCancelled`
  - `RoomAutoAssigned`
  - `RoomAssignmentCleared`
  - `RoomManuallyOverridden`
  - `PatientCheckedIn`
  - `QueueUpdated`
  - `ConsultationStarted`
  - `ConsultationCompleted`
  - `MedicalRecordDrafted`
  - `InvoiceIssued`
  - `InvoiceTotalCalculated`
  - `PaymentRecorded`
  - `ReceiptGenerated`

- **Consumers**
  - `MS-10 Notification` consumes booking/cancellation/reminder/payment/receipt events.

## Authorization Boundary Contract

- `MS-01` is the only service allowed to define/modify role-permission matrices.
- `MS-03` and `MS-10` must treat permissions as external policy input from `MS-01`.
- No downstream service may store a competing permission matrix as an authoritative source of truth.
- All write endpoints in downstream services must enforce JWT-based server-side authorization checks before business logic execution.

## Deployment Waves

1. **Wave 1 (Foundation):** `MS-01`, `MS-02`
2. **Wave 2 (Unified Core Delivery):** `MS-03`
3. **Wave 3 (Engagement):** `MS-10`

## Readiness Decision

Architecture is approved for implementation only if:
- This baseline is treated as the authoritative boundary document.
- Service API and event schemas are versioned from this baseline before development starts.

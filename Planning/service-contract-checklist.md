# Service Contract Checklist (Pre-Implementation)

## Purpose
This checklist defines minimum API and event contracts required before implementation starts, with focus on room lifecycle and billing-notification integrations.

## 1) Room Lifecycle Contracts (MS-03)

### 1.1 API Contracts

- `POST /appointments`
  - Creates appointment and runs atomic room assignment algorithm.
  - Required request fields: `patientId`, `doctorId`, `slotStart`, `slotEnd`, `departmentId`.
  - Required response fields: `appointmentId`, `status`, `roomAssigned`, `roomId` (nullable), `roomAssignmentSource` (`FIXED`, `DEPARTMENT_FALLBACK`, `UNASSIGNED`).

- `PUT /appointments/{appointmentId}/cancel`
  - Cancels appointment and releases room occupancy for the slot.
  - Required request fields: `reason`.
  - Required response fields: `appointmentId`, `status`, `releasedRoomId` (nullable), `releasedSlot`.

- `PUT /appointments/{appointmentId}/reschedule`
  - Reschedules appointment and re-runs assignment atomically for new slot.
  - Required request fields: `newSlotStart`, `newSlotEnd`.
  - Required response fields: `appointmentId`, `oldRoomId` (nullable), `newRoomId` (nullable), `roomAssigned`.

- `PUT /appointments/{appointmentId}/room`
  - Manual room override by receptionist/admin before consultation starts.
  - Required request fields: `targetRoomId`, `reason`.
  - Required response fields: `appointmentId`, `previousRoomId` (nullable), `currentRoomId`, `overriddenBy`.

- `PUT /rooms/{roomId}/status`
  - Updates room status (`AVAILABLE`, `UNDER_MAINTENANCE`, `INACTIVE`).
  - If future appointments exist, triggers reassignment workflow and alerts.
  - Required response fields: `roomId`, `status`, `affectedFutureAppointmentsCount`.

### 1.2 Event Contracts

- `AppointmentBooked`
  - Required fields: `eventId`, `occurredAt`, `appointmentId`, `patientId`, `doctorId`, `departmentId`, `slotStart`, `slotEnd`, `roomAssigned`, `roomId` (nullable).

- `RoomAutoAssigned`
  - Required fields: `eventId`, `occurredAt`, `appointmentId`, `roomId`, `assignmentSource`.

- `RoomAssignmentCleared`
  - Required fields: `eventId`, `occurredAt`, `appointmentId`, `previousRoomId`, `reason` (`MAINTENANCE`, `CANCELLATION`, `RESCHEDULE`).

- `RoomManuallyOverridden`
  - Required fields: `eventId`, `occurredAt`, `appointmentId`, `previousRoomId` (nullable), `newRoomId`, `overriddenByUserId`, `overrideReason`.

- `AppointmentCancelled`
  - Required fields: `eventId`, `occurredAt`, `appointmentId`, `patientId`, `doctorId`, `slotStart`, `slotEnd`, `releasedRoomId` (nullable), `cancellationReason`.

## 2) Billing -> Notification Contracts (MS-06 to MS-10)

### 2.1 API Contracts

- `POST /invoices`
  - Creates invoice for completed appointment.
  - Required request fields: `appointmentId`, `patientId`, `lineItems[]` (`serviceId`, `qty`, `unitPrice`).
  - Required response fields: `invoiceId`, `status`, `subtotal`, `discount`, `tax`, `total`.

- `POST /invoices/{invoiceId}/payments`
  - Records payment and transitions invoice status.
  - Required request fields: `amount`, `method`, `paidAt`.
  - Required response fields: `paymentId`, `invoiceId`, `paymentStatus`, `invoiceStatus`.

- `POST /receipts`
  - Generates immutable receipt after successful payment.
  - Required request fields: `invoiceId`, `paymentId`.
  - Required response fields: `receiptId`, `receiptNumber`, `pdfUrl`, `issuedAt`.

### 2.2 Event Contracts

- `InvoiceIssued`
  - Required fields: `eventId`, `occurredAt`, `invoiceId`, `patientId`, `appointmentId`, `total`, `currency`, `status`.

- `PaymentRecorded`
  - Required fields: `eventId`, `occurredAt`, `paymentId`, `invoiceId`, `patientId`, `amount`, `currency`, `method`, `status`.

- `ReceiptGenerated`
  - Required fields: `eventId`, `occurredAt`, `receiptId`, `receiptNumber`, `invoiceId`, `paymentId`, `patientId`, `downloadUrl`.

### 2.3 Notification Consumption Rules

- On `AppointmentBooked`: send booking confirmation.
- On `AppointmentCancelled`: send cancellation notification.
- On `PaymentRecorded` + `ReceiptGenerated`: send payment confirmation with receipt reference.
- Ensure deduplication key format:
  - Booking: `BOOKING:{appointmentId}`
  - Cancellation: `CANCEL:{appointmentId}`
  - Payment: `PAYMENT:{paymentId}`

## 3) Cross-Cutting Technical Constraints

- Event delivery semantics: at-least-once with consumer idempotency.
- Event schema versioning: include `schemaVersion` in every event payload.
- Correlation and tracing: include `correlationId` and `causationId` for distributed flows.
- Auditability: all room assignment and payment state transitions must be audit logged.
- Authorization boundary enforcement:
  - `MS-01` owns role-permission matrix definition.
  - Other services only consume JWT claims/policy decisions and must not define independent permission truth.
  - Every write endpoint must validate permission before business logic and return `403` on policy denial.

## 4) Definition of Ready for Coding

Implementation can start only when:
- API request/response DTOs are reviewed and frozen for `MS-03`, `MS-06`, and `MS-10`.
- Event schemas listed above are versioned and published.
- Ownership boundaries match the baseline architecture document.

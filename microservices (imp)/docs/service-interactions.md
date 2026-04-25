# Service Interactions

## Services
- `MS-01 Identity & Access`
- `MS-02 System Configuration`
- `MS-03 Unified Clinic Operations`
- `MS-10 Notification`

## Communication Pattern
- **Synchronous REST**
  - Client apps authenticate through `MS-01`.
  - Client apps execute core workflows through `MS-03`.
  - `MS-03` and `MS-10` read runtime settings from `MS-02`.
- **Asynchronous Events**
  - `MS-03` publishes booking/cancellation/payment/receipt events to message broker.
  - `MS-10` consumes events and dispatches notifications.

## Dependency Handling
- Startup dependency order:
  1. `MS-01`, `MS-02`
  2. `MS-03`
  3. `MS-10`
- Failure behavior:
  - `MS-03` business transactions do not block on `MS-10`.
  - Notification delivery retries with bounded backoff.
  - At-least-once delivery requires idempotent consumers.

## Service Architecture Graph

```mermaid
flowchart LR
  clientApps[ClientApps] --> ms01[MS01IdentityAccess]
  clientApps --> ms03[MS03UnifiedClinicOperations]
  ms03 --> ms02[MS02SystemConfiguration]
  ms10[MS10Notification] --> ms02
  ms03 --> broker[MessageBroker]
  broker --> ms10
```

## User Workflow Graph

```mermaid
flowchart TD
  userLogin[UserLogin] --> authMs01[AuthenticateInMS01]
  authMs01 --> bookMs03[BookAppointmentInMS03]
  bookMs03 --> roomAssign[RunUS014RoomAssignment]
  roomAssign --> checkinQueue[CheckInAndQueue]
  checkinQueue --> consultation[CompleteConsultation]
  consultation --> billingPayment[InvoiceAndPayment]
  billingPayment --> publishEvents[PublishDomainEvents]
  publishEvents --> notifyMs10[SendNotificationsInMS10]
```

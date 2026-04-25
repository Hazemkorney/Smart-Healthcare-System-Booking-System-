# MS-10 Notification

## Purpose
Asynchronous notification dispatch and delivery orchestration.

## User Story Mapping
- `US-034..US-037`

## Base API and Operations
- Base path: `/api/v1/notifications`
- Health endpoint: `/actuator/health`
- Default port: `8090`

## Notes
- Consumes booking/cancellation/payment events.
- Handles deduplication and retry policy in dispatch pipeline.

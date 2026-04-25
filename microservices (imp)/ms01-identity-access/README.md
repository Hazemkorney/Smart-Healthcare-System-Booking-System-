# MS-01 Identity & Access

## Purpose
Authentication, session lifecycle, and RBAC source of truth.

## User Story Mapping
- `US-042..US-045`
- RBAC policy ownership for `US-006`

## Base API and Operations
- Base path: `/api/v1/auth`, `/api/v1/rbac`
- Health endpoint: `/actuator/health`
- Default port: `8081`

## Notes
- This service defines role-permission matrix policy.
- Other services consume authorization decisions and do not own RBAC truth.

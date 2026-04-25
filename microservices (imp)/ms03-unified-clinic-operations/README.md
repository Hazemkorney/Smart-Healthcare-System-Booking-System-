# MS-03 Unified Clinic Operations

## Purpose
Unified business domain service for clinic setup, operations, consultation, records, billing, and reporting.

## User Story Mapping
- `US-001..US-041`

## Base API and Operations
- Base path: `/api/v1/*`
- Health endpoint: `/actuator/health`
- Default port: `8083`

## Domain Modules
- `setup` (`US-001..US-007` operational setup items)
- `patients` (`US-008..US-010`)
- `appointments` (`US-011..US-015`, canonical `US-014` assignment flow)
- `queue` (`US-016..US-019`)
- `consultation` (`US-020..US-026`)
- `records` (`US-027..US-028`)
- `billing` (`US-029..US-033`)
- `reporting` (`US-038..US-041`)

## Event Contract Reference
- See `../../Planning/service-contract-checklist.md` for payload and contract baseline.

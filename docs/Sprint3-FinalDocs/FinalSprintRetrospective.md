# Final Sprint Retrospective

## Sprint 3 Overview

Sprint 3 was the final delivery sprint of the Support Ticketing System. The focus was on completing the remaining user-facing workflows, strengthening reporting and security, testing the application, preparing the production deployment, and completing the final documentation and evidence.

The sprint moved the project from a feature-complete application toward a more production-ready solution.


## What Went Well

### 1. Customer and Agent Workflows

The Customer Portal and Agent Portal workflows were completed and connected to the existing CQRS application structure.

Customers can interact with their tickets while respecting ownership rules, and support agents can work with tickets assigned to them. This helped demonstrate that the system was not only implementing CRUD operations but enforcing the actual support-business workflow.

### 2. Reporting and Analytics

Support analytics and reporting features were completed.

The reporting queries were designed to return DTOs instead of exposing domain entities directly. Server-side aggregation was also used for report calculations where appropriate, reducing unnecessary data processing on the application side.

### 3. Security and Authorization

Security was treated as an important part of the final hardening process.
Negative authorization scenarios were tested, including cases such as:
- Customers attempting to access administrative reports.
- Customers attempting to access another customer's ticket.
- Agents attempting to work with tickets that are not assigned to them.
- Users attempting actions outside their role.
- Missing authentication.
- Invalid ownership or team scope.

These tests helped verify that authorization was enforced at the application/business level and not only through controller attributes.

### 4. Automated Testing

Automated application tests were added for important business workflows.

Integration tests were also completed with the team to verify critical API workflows against the real application infrastructure.

The project also included regression coverage for important business rules and negative scenarios. This gave the team more confidence that later refactoring would not silently break existing behavior.

### 5. Audit and History Tracking

Audit logging was added for important business actions.

The system now records significant actions such as ticket creation, cancellation, assignment, reassignment, status changes, priority changes, starting work, resolving tickets, and attachment metadata creation.

Status-history tracking also provides a history of ticket status transitions.

This improved traceability and makes it easier to understand how a ticket moved through the support workflow.

### 6. Validation and Business Rules

Final hardening included reviewing validation and business rules across the application.
Examples included:
- Attachment file-type validation.
- Customer ticket ownership checks.
- Agent assignment checks.
- Support-team scope checks.
- Inactive-agent protection.
- Closed/cancelled ticket restrictions.
- Invalid ticket-status transitions.
- SLA policy validation.

These rules helped ensure that invalid operations were rejected before changing application state.

### 7. Error Handling

The global exception-handling mechanism was reviewed and verified.

Expected application exceptions are converted into safe API responses without exposing internal implementation details, stack traces, or sensitive information to API consumers.

This provided a consistent error-handling experience across the API.

### 8. Controller Review

The controllers were reviewed to ensure that business logic remained inside the application layer.

Controllers are primarily responsible for receiving requests and sending them through MediatR, while handlers contain the application workflow and business rules.

This maintained the Clean Architecture and CQRS approach established earlier in the project.

### 9. Production Deployment

The application was deployed to MonsterASP and the production environment was verified.

Deployment evidence was prepared as part of the final delivery requirements, including the deployed API and production Swagger access.

This confirmed that the application was not only working locally but could also be delivered to a real hosting environment.

## What Could Be Improved

### 1. Testing Earlier

Although the final application has automated and integration tests, some testing activities happened later in the development process.

### 2. Earlier Identification of Audit Requirements

Audit logging was added during the quality and hardening stage.

For future projects, audit requirements should be identified during requirements analysis so that important actions and required audit information are clear before implementation begins.

## Final Reflection

Sprint 3 successfully completed the transition from a working development project to a tested, secured, documented, and deployed application.

The most important lesson from the final sprint was that production readiness is more than finishing the remaining features. It also requires testing, authorization verification, error handling, auditability, documentation, deployment, and regression protection.

The final result demonstrates the complete development lifecycle: requirements -> architecture -> implementation -> testing -> hardening -> deployment -> documentation.

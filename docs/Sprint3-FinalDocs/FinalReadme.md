# Support Ticketing System — Final Project Documentation

## 1. Project Overview
- The Support Ticketing System is a backend application developed as the final TechMaster ASP.NET Backend Career Training capstone project.
- The system provides a structured platform for managing customer support tickets from creation through resolution and closure. It supports customer communication, agent assignment, ticket status workflows, SLA monitoring, escalation, attachments, reporting, audit tracking, and role-based access control.
- The project was developed by a team using Agile/Scrum practices, with work organized into three sprints and managed through Jira.

### Business Goal
The system is designed to solve common support-management problems such as:

- Unstructured ticket handling
- Incorrect ticket ownership
- Unauthorized access to customer information
- Uncontrolled ticket status changes
- Assignment of tickets to inactive agents
- Missing SLA visibility
- Lack of operational reporting
- Missing audit history for important actions
- Inconsistent handling of business-rule violations

The application enforces these rules at the application layer instead of relying only on the API controllers or client applications.

## 2. Main Business Capabilities
The system provides the following major capabilities:
- User authentication and authorization
- Customer ticket creation
- Ticket cancellation
- Ticket categorization and prioritization
- Ticket assignment and reassignment
- Ticket status workflow
- Ticket start and resolution
- Customer public comments
- Agent public replies
- Internal support notes
- Ticket attachments metadata
- SLA policy management and SLA-risk reporting
- Ticket status history
- Activity/audit logging
- Customer ownership protection
- Agent assignment protection
- Support-team scope protection
- Administrative and support analytics
- Customer and agent portal capabilities

# 3. User Roles
The application uses role-based authorization to control access to business operations.

## Customer
Customers can:
- Create their own tickets
- View their own tickets
- Cancel eligible tickets
- Add public comments to their tickets
- Add attachments to their own tickets
- Close resolved tickets
- Access their own ticket-related information

Customers cannot:
- Access another customer's tickets
- Add internal notes
- Assign or reassign tickets
- Change arbitrary ticket statuses
- Access administrative reports
- Reopen tickets

## Support Agent
Support agents can:
- View tickets assigned to them
- Start assigned tickets
- Add public replies
- Add attachments to tickets they are assigned to
- Resolve tickets assigned to them
- Work with ticket conversations

Agents cannot:
- Assign tickets
- Reassign tickets
- Access administrative operations outside their scope
- Modify tickets belonging to other agents
- Access restricted administrative reports

## Support Lead
Support Leads can:
- Assign tickets
- Reassign tickets
- Manage ticket workflow within their responsibility
- Close resolved tickets
- Reopen closed tickets
- Add internal notes
- Access SLA-risk information
- Operate within their support-team scope

## Admin
Admins can:

- Manage operational ticket data
- Assign tickets
- Change ticket priority
- Access administrative reports
- Monitor system activity
- Perform administrative operations

# 4. Delivery Through Three Sprints
The project was delivered through three development sprints.

## Sprint 1 — Foundation, Identity & Ticket Intake

- Goal : Establish the application architecture, authentication, database foundation, and core ticket lifecycle.

- Delivered :
- Clean Architecture foundation
- CQRS architecture using MediatR
- Domain entities and relationships
- Entity Framework Core database foundation
- Identity/authentication implementation
- JWT authentication
- Role-based authorization
- Ticket intake
- Ticket creation
- Ticket categorization
- Ticket triage
- Ticket assignment
- Assignment business rules
- Initial reporting foundation
- End-to-end CQRS features

- Sprint Outcome :
The database foundation became stable and the core ticket-management workflow was available through complete application/API flows.
At least two major epics contained end-to-end CQRS features merged into the main development branch.

# 5. Sprint 2 — Conversation, Workflow, SLA & Escalation

- Goal : Implement the main operational workflow and protect the highest-risk business rules.

- Delivered :
- Customer public comments
- Agent public replies
- Internal support notes
- Ticket status workflow
- Start ticket workflow
- Resolve ticket workflow
- Close/reopen rules
- Ticket status history
- Assignment/reassignment workflow
- Ticket priority management
- SLA policies
- SLA-risk reporting
- Escalation-related business rules
- Customer ownership checks
- Agent assignment checks
- Support-team scope checks
- Negative authorization tests
- Business-rule tests

- Sprint Outcome : The main support workflow and its highest-risk business rules were implemented with role, ownership, and business-rule protection.

# 6. Sprint 3 — Customer & Agent Portals, Analytics, Hardening & Production

- Goal : Complete the user-facing operational capabilities, strengthen quality, deploy the system, and prepare the final release.

- Delivered
- Customer portal capabilities
- Agent portal capabilities
- Support analytics
- Reporting queries
- SLA-risk reports
- Server-side report aggregation
- Projection DTOs for reporting
- Automated application tests
- Integration tests
- Negative authorization tests
- Regression testing
- Audit logging
- Status-history tracking
- Attachment validation
- Global exception-handling verification
- Controller review
- Production deployment
- Final documentation
- Final evidence collection
- Release preparation

- Sprint Outcome: The application reached production-ready delivery with testing, hardening, deployment, evidence, and final documentation completed.

# 7. Architecture
The application follows Clean Architecture principles.


src/
     SupportTicketing.Api
     SupportTicketing.Application
     SupportTicketing.Domain
     SupportTicketing.Infrastructure

tests/
     SupportTicketing.Application.Tests
     SupportTicketing.IntegrationTests

## Domain
Contains the core business model:
- Entities
- Enums
- Domain concepts
- Domain-level rules

## Application
Contains application use cases and business orchestration:
- Commands
- Queries
- MediatR handlers
- Validators
- DTOs
- Application interfaces
- Exceptions
- Business rules

## Infrastructure
Contains implementation details such as:
- Entity Framework Core
- Database access
- Identity implementation
- External infrastructure concerns
- Persistence

## API
Responsible for:
- HTTP endpoints
- Controllers
- Authentication/authorization configuration
- Dependency injection
- Middleware
- Swagger
- HTTP-level concerns

# 8. CQRS
The application uses CQRS — Command Query Responsibility Segregation.

- Commands are used for operations that change application state.
- Queries are used to retrieve information without performing business state changes.
- MediatR is used to dispatch commands and queries to their corresponding handlers.

# 9. Validation
FluentValidation is used to validate commands before business processing.

Validation examples include:
- Required fields
- Ticket IDs
- Agent IDs
- Team IDs
- Comment content
- Attachment metadata
- File size
- Supported attachment content types
- Valid ticket statuses
- Valid request values

# 10. Authorization & Security
Security is implemented at multiple levels.

### Authentication
JWT authentication is used to identify authenticated users.

### Role Authorization
Roles include: Admin - SupportLead - SupportAgent -Customer

### Ownership Authorization
The application does not rely only on roles.
- Business ownership is also checked.
- EX : A customer cannot access a ticket simply because they are authenticated.

### Assignment Authorization
Agents must be assigned to a ticket before performing agent-specific operations.

### Team Authorization
Support Leads are checked against their assigned support team before accessing team-scoped ticket operations.
This prevents users from bypassing business ownership rules by manipulating IDs in requests.

# 11. Ticket Status Workflow
The ticket lifecycle is controlled by business rules.
- A simplified workflow is: New -> Assigned -> InProgress -> Resolved -> Closed 
- Additional workflow behavior includes cancellation and reopening where permitted.

# 12. Conversations
The system distinguishes between public and internal communication.

- Public Comments : Visible to customers and support staff (Customer comments - Agent public replies )
- Internal Notes  : Used by support staff for internal communication.Customers are explicitly prevented from creating internal notes.

# 13. Attachments
- The application stores attachment metadata associated with tickets.
- attachment types are validated at the application layer.
- Ownership and assignment checks are also applied before attachment metadata can be created.

# 14. SLA & Escalation
The system supports SLA policies based on ticket characteristics such as:
- Category
- Priority
- Resolution target

The SLA-risk query evaluates active tickets against their applicable SLA policies.
Tickets approaching their SLA target are identified, and breached tickets are marked accordingly.
SLA-risk access is restricted to authorized support roles.

# 15. Audit Logging
- Important business actions are recorded using 'ActivityLog'.
- Audit logging is applied selectively to important state-changing operations. 
- This provides traceability for important operational changes.

# 16. Status History
- Ticket status changes are also recorded separately through 'TicketStatusHistory'.
- This allows the application to maintain a chronological history of ticket workflow changes.

# 17. Global Exception Handling
The API uses centralized exception handling so that application exceptions are converted into consistent HTTP responses.
The system handles cases such as:
- Validation errors
- Unauthorized requests
- Forbidden operations
- Missing resources
- Business-rule violations
- Invalid input
The response does not expose sensitive internal implementation details such as stack traces or database internals.

# 18. Database & ERD
Entity Framework Core is used for persistence.
The database contains the core support-ticketing concepts, including:

- Users
- Customer profiles
- Agent profiles
- Support teams
- Ticket categories
- Tickets
- Ticket assignments
- Ticket comments
- Ticket attachments
- Ticket status history
- SLA policies
- Activity logs

The ERD documents the relationships between these entities and represents the database foundation used by the application.

# 19. Reporting
The application provides support and operational reporting.
Reporting queries were optimized by:
- Using server-side aggregation
- Returning only required fields
- Using projection DTOs
- Avoiding unnecessary entity loading

This reduces unnecessary application-side processing and keeps reporting queries focused on the required data.

# 20. Testing Strategy
Testing was treated as part of the final delivery rather than an optional activity.

## Application Tests
Automated application tests cover important business rules and failure scenarios.
Examples include:
- Create ticket happy path
- Invalid status transitions
- Inactive agent assignment
- Unauthorized customer operations
- Attachment ownership protection
- Customer report authorization
- Conversation business rules
- Ticket workflow restrictions

## Integration Tests
Integration tests cover critical API workflows and verify interaction across application components.

## Negative Security Tests
Negative tests verify that unauthorized users cannot perform protected actions.
Examples:
- Customer cannot add internal notes
- Customer cannot reopen tickets
- Customer cannot access another customer's ticket
- Customer cannot access administrative reports
- Unassigned agents cannot access agent-only ticket operations

# 21. Local Setup

## Prerequisites
The project requires:
- .NET 8 SDK
- SQL Server
- Git
- Visual Studio or VS Code

1- Clone the Repository
2- Restore Dependencies
3- Build
4- Configure the application database connection string in the appropriate configuration/environment settings.
5- Then apply migrations:
6- Run the API
7-Swagger can then be opened through the configured API URL.


# 22. Configuration
Sensitive configuration values should not be committed to source control.
- ConnectionStrings
- JWT settings
- Authentication secrets
- External service configuration
- Production secrets are configured through the hosting environment rather than stored directly in the repository.

# 23. Production Deployment
The application was deployed to MonsterASP as the production hosting environment.
The production delivery included:
- API deployment
- Production database configuration
- Environment-specific configuration
- Database availability verification
- Production Swagger verification
- API endpoint verification
- Production safety checks

Production credentials and secrets are not stored in the repository.
# 24. Production Safety
Before final delivery, the production environment was checked to ensure:
- The application starts successfully.
- The production database is reachable.
- Swagger is accessible.
- Authentication works.
- Protected endpoints require authentication.
- Role restrictions are enforced.
- Sensitive configuration is not exposed.
- Database changes are applied successfully.
- Critical API workflows work in production.

# 25. Jira & Agile Workflow
The project was developed using Agile/Scrum practices.
Jira was used as the official task-management system for:
- Epics
- User stories
- Tasks
- Bugs
- Sprint planning
- Sprint tracking
- Blockers
- Delivery evidence

- Development work was divided into three sprints.
- The final Jira board and sprint evidence document the progression from foundation to production delivery.

# 26. Final Quality Improvements
The final hardening stage included more than the minimum required quality improvements.
Major improvements included:
1. Added automated application tests.
2. Added integration tests for critical API workflows.
3. Added negative authorization tests.
4. Added audit logging for important actions.
5. Added assignment/reassignment audit tracking.
6. Added status-history tracking.
7. Refactored SLA-risk query around reusable SLA policy logic.
8. Added server-side aggregation for reports.
9. Added projection DTOs for reporting queries.
10. Added attachment file-type validation.
11. Added ownership checks for customer ticket access.
12. Added assignment/team-scope checks for agents and leads.
13. Verified global exception handling returns safe responses.
14. Reviewed controllers to keep business logic inside handlers.

# 27. Final Delivery Status

The Support Ticketing System completed the three planned delivery sprints.

## Final Result

The project delivers a role-aware support ticketing backend built around Clean Architecture and CQRS, with protected business workflows, database-backed ticket management, SLA monitoring, reporting, auditability, automated testing, and production deployment.


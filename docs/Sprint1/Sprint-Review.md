# Sprint 1 — Sprint Review
# Project: Support Ticketing Platform
# Sprint: Sprint 1
# Release: v0.1
# Branch: main
# Team: Rowida + Sama
# Sprint Goal: 
- Establish the production-ready backend foundation and implement the first end-to-end ticket intake and assignment workflows.

# 1. Sprint Goal
The goal of Sprint 1 was to establish the core architecture and backend foundation of the Support Ticketing Platform and deliver the first working business workflows.
By the end of the sprint, the team delivered:
    - Clean Architecture solution structure.
    - CQRS/MediatR application structure.
    - Entity Framework Core database foundation.
    - API foundation and Swagger documentation.
    - Global exception/error handling.
    - Domain model and database relationships.
    - Identity and JWT authentication.
    - Current-user context.
    - Customer and Agent profiles.
    - Authentication workflows.
    - Customer ticket intake workflows.
    - Ticket categorization and priority.
    - Ticket assignment and reassignment workflows.
    - Agent's assigned-ticket view.

# 2. What We Delivered
Architecture & Application Foundation
The team established the backend architecture using Clean Architecture and CQRS.
Delivered:
    - Clean Architecture solution structure.
    - Separation between Domain, Application, Infrastructure, and API.
    - CQRS using MediatR.
    - Commands and Queries organized by feature/use case.
    - Application interfaces separated from infrastructure implementations.
    - Dependency injection configuration.
This gives the project a structure that can support the additional workflows planned for later sprints without putting business logic directly inside controllers.

# Database & EF Core Foundation
The team configured the persistence layer using Entity Framework Core.
Delivered:
    - AppDbContext.
    - Entity configurations.
    - EF Core relationships.
    - Fluent API configurations.
    - Database constraints.
    - Foreign keys.
    - Required/optional properties.
    - Indexes for important query paths.
    - Initial database/migration foundation.
The database was designed around the actual support-ticketing domain rather than treating the project as a collection of independent CRUD tables.

# API Foundation
The team established the API layer.
Delivered:
    - Controllers.
    - Routing conventions.
    - Dependency injection.
    - Swagger/OpenAPI.
    - Authentication configuration.
    - Authorization configuration.
    - API request/response models.
Swagger was also configured to support Bearer JWT authentication so protected endpoints can be demonstrated directly.

# Global Error Handling
The team implemented centralized exception handling through middleware.
The API now converts application exceptions such as:
    - Unauthorized requests
    - Forbidden requests
    - Not-found cases
    - Business-rule violations
    - Invalid arguments
    - Unexpected exceptions
into consistent HTTP responses instead of exposing raw exceptions to API consumers.

# 3. Domain Model
The team created the main domain entities and their relationships required for Sprint 1.
This included:
    - Application users
    - Customer profiles
    - Agent profiles
    - Support teams
    - Tickets
    - Ticket categories
    - Ticket assignments
    - Ticket status history
    - Ticket comments
    - Ticket attachments
The team also configured:
    - Entity relationships.
    - Foreign keys.
    - Required fields.
    - Enum-based statuses and priorities.
    - Database constraints.
    - Indexes.

# 4. Identity & Security
The team implemented the identity and authentication foundation.
Delivered:
Application User & Roles
    - ApplicationUser.
    - Role-based access control.
    - Customer role.
    - Support Agent role.
    - Support Lead/Admin roles as part of the authorization model.
JWT Authentication
Implemented:
    - User registration.
    - Login.
    - JWT token generation.
    - JWT validation.
    - Role claims.
    - User ID claims.
    - Email/name claims.
    - Protected endpoints using [Authorize].
Current User Context
The team implemented ICurrentUserService to retrieve the authenticated user's information from the JWT claims.
This allows application code to determine:
    - Current user ID.
    - Current email.
    - Current role.
    - Authentication state.
Importantly, ticket ownership and identity are determined from the authenticated context rather than trusting a user ID supplied by the client.

# 5. Profile Management
The team implemented profile models and relationships for:
    - Customers.
    - Support Agents.
This allows the system to connect the authenticated application user to their corresponding business profile.

# 6. Authentication Use Cases
The team completed:
- Register : A new user can register and receive the appropriate account/profile setup.
- Login : A valid user can authenticate and receive a JWT containing the required identity and role claims.
- Get Current User An authenticated user can retrieve their current account information.

# 7. Ticket Intake
The team implemented the first customer-facing ticket workflows.

- Create Ticket
Customers can create a ticket with:
    - Title
    - Description
    - Category
    - Priority
Business rules include:
    - Customer identity comes from the authenticated user.
    - Ticket starts with New status.
    - Category is validated.
    - Required ticket fields are validated.
    - Priority follows the defined ticket rule.

- Get My Customer Tickets
Customers can retrieve only their own tickets.
The endpoint supports:
    - Pagination.
    - Status filtering.
    - Priority filtering.
    - Category filtering.
The response exposes customer-safe ticket information and does not expose internal support information.

- Cancel Ticket
Customers can cancel their own tickets when the ticket is in an allowed early status.
The workflow includes:
    - Ownership validation.
    - Allowed-status validation.
    - Cancellation reason.
    - Cancellation timestamp.
    - Ticket status update.
    - Status history entry.

# 8. Ticket Categorization & Priority
The team implemented ticket categorization and priority handling.
Tickets can be associated with a category and have a defined priority such as:
    - Low
    - Normal
    - High
    - Critical
This provides the foundation for later SLA and escalation functionality.

# 9. Ticket Assignment
The team implemented the first support-side ticket management workflows.
Delivered:
Assign Ticket
A ticket can be assigned to a support agent.
Reassign Ticket
An existing assignment can be changed according to the defined business rules.
View My Assigned Tickets
A support agent can retrieve the tickets assigned to them.
The assignment logic uses the authenticated support agent rather than allowing the client to impersonate another agent.

# 10. Sprint Outcome
Sprint 1 goal: Achieved.
The team successfully moved from an empty backend foundation to a working support-ticketing backend with:
Authentication → Customer Profile → Ticket Creation → Ticket Tracking → Cancellation → Agent Assignment → Agent Ticket View
This gives the project a complete initial business flow and establishes the foundation required for the conversation, status workflow, SLA, and escalation features planned for future sprints.
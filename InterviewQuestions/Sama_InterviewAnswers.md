Interview Answers
-----------------

1- Our project is a Support Ticketing Platform, which is a SaaS backend for managing customer support issues.

The main problem it solves is that customer issues can otherwise be scattered across chat messages or other communication channels. In our system, a customer creates a ticket, the support team triages it, assigns it to an agent, communicates through comments, changes its status, resolves or closes it, and the system keeps the history.

We have four main roles: Admin, SupportLead, SupportAgent, and Customer. Customers can only access their own tickets, while agents can work only on tickets assigned to them unless they have a higher-level role. Internal notes are available to staff but must never be exposed to customers. 

The backend uses Clean Architecture and CQRS with MediatR. Commands are responsible for changing state, while queries are responsible for reading data. We also have EF Core and SQL Server for persistence, JWT authentication and role authorization for security, FluentValidation for request validation, global exception handling, audit logging, and automated tests.

The business value is that the platform gives a company a controlled and auditable support workflow instead of just storing messages. Management can also see workload, ticket status, SLA risk, resolution metrics, and customers generating the most tickets. 

---

2- The dependency direction is:


              ┌──────────────────────┐
              │     SupportTicketing │
              │          API         │
              └──────────┬───────────┘
                         │
                         ▼
              ┌──────────────────────┐
              │     Application      │
              │ CQRS / Handlers/DTOs │
              └──────────┬───────────┘
                         │
                         ▼
              ┌──────────────────────┐
              │        Domain        │
              │ Entities / Enums /   │
              │ Business concepts    │
              └──────────────────────┘

              Infrastructure
                    │
                    ▼
              Application
                    │
                    ▼
                 Domain
```

More precisely, Domain is the center and should not depend on Infrastructure or API.

Application contains our use cases, Commands, Queries, handlers, DTOs, validators, and interfaces.

Infrastructure implements things such as the database and persistence services.

API is responsible for HTTP concerns: controllers, authentication configuration, middleware, Swagger, and receiving requests.

So the important principle is that business logic does not depend on the outer layers.

The project requirements specifically require Clean Architecture with Domain, Application, Infrastructure, and API responsibilities. 

---

3- We implemented CQRS in the **Application layer** because Application represents our business use cases.

A Command changes state.

For example:

CreateTicketCommand
AssignTicketCommand
ReassignTicketCommand
ChangeTicketStatusCommand
AddPublicCommentCommand
AddInternalNoteCommand
CancelTicketCommand
ReopenTicketCommand
```

A Query only reads data.

For example:

GetMyCustomerTicketsQuery
GetMyActiveTicketsQuery
GetTicketDetailsQuery
GetTicketConversationQuery
GetAgentWorkloadReportQuery
GetSlaRiskReportQuery

```

This separation makes the code easier to understand and maintain.

The project specification itself separates the CQRS backlog into state-changing Commands and read/reporting Queries. 

---

4- My `CreateTicketCommandHandler` has three important dependencies:

ICurrentUserService
IApplicationDbContext
IValidator<CreateTicketCommand>

`ICurrentUserService`

This tells the handler who is currently authenticated.

I don't accept `CustomerId` from the request.

Instead, I get:

```csharp
currentUserService.UserId
```

This prevents a customer from sending another customer's ID.

`IApplicationDbContext`

This is the Application abstraction over persistence.

The handler uses it to access:

CustomerProfiles
TicketCategories
Tickets
ActivityLogs

It doesn't directly depend on a concrete controller/database workflow.

`IValidator<CreateTicketCommand>`

FluentValidation validates:

* title
* description
* category ID

Then the handler checks authentication, finds the current customer's profile, verifies the category, creates the ticket with the initial status and priority, saves it, and creates an audit/activity log.

The business rule requires a new ticket to receive an initial state automatically. 

---

5- For example, in the top-customers report, instead of loading complete customer entities and then calculating everything in memory, I create a server-side query that projects only the fields needed:

```text
CustomerId
CustomerName
TicketCount
```

The query then filters customers with tickets:

```text
TicketCount > 0
```

Then it sorts:

```text
TicketCount descending
CustomerName ascending
```

And finally applies:

```text
Skip(...)
Take(...)
```

for pagination.

So the database does the filtering, sorting, counting, and pagination rather than loading all records into memory.

This is important for performance, especially for reporting queries.

The project specifically expects server-side aggregation and pagination for analytics/reporting. 

---

6- One good example is unique identity data, such as the user's email.

At the application level, Identity validates user creation.

At the database level, SQL Server has a unique index on the email, so even if application logic accidentally tries to insert a duplicate email, the database prevents it.

Another important example is the relationship structure between tickets, customers, categories, assignments, and history. We use explicit foreign keys rather than storing names only.

The project specifically requires documenting unique constraints, indexes, and concurrency-sensitive rows. 

---

7- Assignment/reassignment is the workflow I would identify as transaction-sensitive because several related changes must remain consistent.

For example, during reassignment we may need to:

1. End the previous assignment.
2. Create the new assignment.
3. Update the ticket's state if required.
4. Create the corresponding history/audit information.

If one operation succeeds and another fails without transactional protection, we could end up with inconsistent assignment history.

The specification explicitly says assignment history must be preserved and reassignment should capture old/new assignment information. 

---

8- We use the current authenticated user's identity instead of trusting a user ID from the request.

For example, a customer request doesn't say:

```text
customerId = 15
```

and then allow the backend to trust that value.

Instead, the backend gets the user ID from:

```text
ICurrentUserService
        ↓
JWT claims
        ↓
Current User ID
```

Then the query/handler filters the data based on that identity.

For example:

```text
CurrentUserId → CustomerProfile → Customer's Tickets
```

So if Customer A tries to request Customer B's ticket ID, the handler checks ownership and rejects the request.

This directly protects TICKET-R01, which states that customers can access only their own tickets. 

It also follows the requirement that current-user endpoints should not accept arbitrary customer or agent IDs. 

---

9- 401 — Unauthenticated

If someone calls a protected endpoint without valid authentication:

```text
401 Unauthorized
```

The user hasn't successfully authenticated.

403 — Authenticated but forbidden

For example, a customer tries to access another customer's ticket.

The customer is authenticated, but doesn't have permission to access that resource:

```text
403 Forbidden
```

Business validation

For example:

```text
New → Closed
```

if that transition isn't allowed.

The request can reach the application, but the business rule rejects it.

That's different from authentication or authorization.

The project's negative-test requirements explicitly include cross-customer access, agent ownership, invalid status transitions, internal notes, and inactive-agent assignment. 

---

10- We don't allow the client to freely change the status to any enum value.

The workflow is controlled according to business rules.

Our main lifecycle is:

```text
New
 ↓
Assigned
 ↓
InProgress
 ↓
Resolved
 ↓
Closed
```

There are also alternative paths such as:

```text
New → Cancelled
Closed → Reopened
```

depending on the role and policy.

When an important status transition happens, we also maintain status history containing information such as:

```text
TicketId
ChangedByUserId
ChangedAt
OldStatus
NewStatus
Reason
```

This gives us an audit trail.

The official workflow specifies the primary path and invalid-state rules. 

---

11- A major concurrency risk is assignment.

Imagine two SupportLeads try to assign the same ticket to different agents at almost the same time.

Without proper protection, both requests could see the ticket as available and create conflicting active assignments.

Another concurrency-sensitive operation is changing ticket status or priority while another user is changing it.

Our domain therefore needs to protect things such as:

```text
Only one active assignment
Correct assignment history
Valid status transitions
```

The project specifically identifies assignment and concurrency-sensitive rows as database/design concerns. 

---

12- Jira helped us break the project into epics, stories, tasks, and sprints instead of trying to build the whole backend at once.

For example, we separated:

```text
Ticket Intake
Triage
Assignment
Conversation
Status Workflow
SLA
Customer Portal
Support Analytics
Audit
```

We also tracked work through the different sprint stages.

The biggest benefit was visibility.

We could see:

* what was completed
* what was in progress
* what was blocked
* who owned a task
* which features belonged to which sprint

The project specifically requires one Jira Epic for each major capability and recommends vertical ownership and code review across the team. 

---

13- The problem is that simply knowing a ticket ID should not give a customer access to it.

We then added a negative integration test:

```text
Customer A
   ↓
requests Customer B's ticket
   ↓
403 / NotFound
```

The purpose of the regression test is that if somebody later changes the query and accidentally removes the ownership filter, the automated test catches it.

This corresponds directly to the project's requirement for regression tests around serious bugs and the mandatory cross-customer access negative test. 

---

14- We chose Clean Architecture with CQRS because the project has many distinct state-changing operations and read/reporting operations.

For example:

```text
Commands
CreateTicket
AssignTicket
ReassignTicket
ChangeStatus

Queries
GetMyTickets
GetAgentQueue
GetSlaRisk
GetAgentWorkload
```

The alternative would have been putting all operations directly into controllers or using one large service for everything.

We rejected that because it would make controllers business-heavy and mix HTTP concerns, business rules, persistence, and reporting logic.

The specification explicitly requires thin controllers where each HTTP operation maps to a clear command or query. 

---

15- Production secrets should not be committed into GitHub.

Our production configuration should provide:

```text
ConnectionStrings:DefaultConnection
Jwt:Key
Jwt:Issuer
Jwt:Audience
```

through the hosting environment's configuration/secrets mechanism.

For local development, we can use:

```text
appsettings.Development.json
User Secrets
```

For production, the hosting platform should provide the sensitive values as environment variables or protected configuration.

This is particularly important because our database connection string and JWT signing key are secrets.

The production acceptance criterion explicitly says connection strings and JWT secrets must not be public.

---

16- The biggest limitation of v1.0 is that some advanced SLA behavior is simplified compared with a full enterprise support platform.

For example, the stretch requirements include:

```text
SLA pause windows/business hours
Escalation rules
Email integration
Background breach notifications
Advanced search
Attachment storage
Customer satisfaction ratings
```

These are identified as stretch extensions rather than mandatory core functionality. 

So our v1.0 focuses on the core ticket lifecycle, ownership, assignment, comments, status history, SLA indicators, reports, security, auditing, testing, and deployment.

---

17- If we continued with Sprint 4, I would prioritize the features that improve the production system rather than simply adding more CRUD endpoints.

My Sprint 4 would include:

1. Advanced SLA

```text
Business hours
Pause windows
Escalation rules
Automatic SLA breach notifications
```

2. Notifications

```text
Email notifications
Ticket assignment notifications
Status-change notifications
SLA alerts
```

3. Advanced search

```text
Tags
Multiple filters
Date ranges
Agent/team filters
```

4. Attachments

The current domain contains attachment metadata, while actual file storage is an extension.

5. Customer satisfaction

After a ticket is resolved/closed, customers could provide a rating.

6. Further production hardening

```text
Concurrency handling
More integration tests
Performance testing
Monitoring
Structured logging
```

These directions align with the project's stretch extensions and the identified engineering risks. 

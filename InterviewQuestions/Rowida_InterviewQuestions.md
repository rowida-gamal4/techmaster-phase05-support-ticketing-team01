# 1. Explain the product and its business value in two minutes.
- Our project is a Support Ticketing System that helps customers report problems and allows support teams to manage those problems through their complete lifecycle
- Customer :create a ticket, add public comments and attachments, and follow the ticket status
- Support agents :work on assigned tickets, add replies, start and resolve tickets .
- Support Leads :manage assignments and certain workflow transitions
- Admins have access to management and reporting features

- The main business value is that the system gives the support team a structured workflow instead of handling customer issues manually. It also provides role-based access, ticket ownership protection, SLA monitoring, status history, assignment tracking, audit logs, and reporting.

# 2. Draw the dependency direction of your Clean Architecture solution.
- Our dependency direction follows Clean Architecture:
API -> Application -> Domain
Infrastructure-> Application-> Domain
- Domain :contains entities, enums, and domain concepts  . it doesn't depend on the other layers.
- Application layer : contains our business use cases using CQRS and MediatR ,It depends on abstractions such as IApplicationDbContext, ICurrentUserService
- Infrastructure layer : implements those abstractions, as the actual database context and authentication-related implementations.
- API layer :responsible mainly for HTTP concerns and sends commands and queries to the Application layer.

# 3. Why is CQRS implemented in Application and how do your Commands differ from Queries?
- We implemented CQRS because our system has many different business operations and reporting requirements.
- Commands represent operations that change state, such as:
Create Ticket
Cancel Ticket
Assign Ticket
Reassign Ticket
Start Ticket
Resolve Ticket
Change Status
Set Priority
Add Comment
Add Attachment Metadata
- Queries are used to read data without changing the system state, such as SLA risk reports and other reporting queries.

# 4. Open one command handler you wrote and explain every dependency.
One example is my CreateTicketCommandHandler
It depends on: ICurrentUserService , IApplicationDbContext ,IValidator<CreateTicketCommand>
- ICurrentUserService  :tells me who is currently authenticated and their identity.
- IApplicationDbContext gives the handler access to the required database abstractions without depending directly on the concrete EF Core implementation.
- IValidator<CreateTicketCommand> validates the incoming request before executing the business operation.

- The handler first validates the command, then verifies authentication, gets the current customer profile, verifies the ticket category exists, creates the ticket saves it, and returns a DTO instead of exposing the entity.

# 5. Open one query handler and explain projection, filtering and pagination.
For example, our reporting/SLA queries filter tickets based on their business requirements.
- In the SLA risk query, we don't return every ticket. We first filter out tickets that are already: Resolved , Closed , Cancelled
- Then we find the active SLA policy based on the ticket's category and priority.
- The result is projected into SlaTicketResponseDto instead of returning the database entity directly.
- For paginated queries such as customer or other list operations, pagination prevents loading the entire dataset into memory and allows the API to return a controlled amount of data.

# 6. Which business rule is protected both in code and database constraints?
- ticket assignment
- At the application level, before assigning a ticket, we check whether there is already an active assignment
- If active assignment exists : reject the operation, use Reassign instead
- We also check that the ticket isn't closed or cancelled, that the agent is active, and that the team is active.
- At the database level, important uniqueness/integrity constraints are used where appropriate to prevent invalid duplicate relationships.

# 7. Which workflow uses a transaction and what would break without it?
- A workflow where multiple related records must change together, such as assignment or status changes.
- Resolving a ticket changes the ticket itself and creates a TicketStatusHistory record.
- If one operation succeeded and the other failed, the system could contain inconsistent information.
- In our implementation, SaveChangesAsync gives us atomic persistence for the changes being saved together

# 8. How do you prevent a normal user from sending another user ID and reading private data?
- We don't trust a user-provided user ID to determine ownership , Instead, we get the authenticated user's identity from ICurrentUserService.
- Example, when a customer wants to access or modify a ticket we get:
(var currentUserId = currentUserService.UserId.Value;)
Then we find the customer profile belonging to that authenticated user.
After that, we compare ticket.CustomerId   == authenticated customer's Id
If they don't match, we return a forbidden response.

# 9. Show one 401 case, one 403 case and one business validation case.
- 401 — Unauthenticated : If the request doesn't contain an authenticated user.
- 403 — Forbidden : ex when customer tries to reopen a ticket. they don't have permission to perform that operation.
- Business rule : trying to close a ticket while it is still New ,the ticket state doesn't allow the operation.

# 10. How does your team handle status transitions?
We treat ticket statuses as a controlled workflow rather than allowing arbitrary changes.
- customer cannot simply change any status they want ; can close a ticket only when it is already: Resolved -> Closed
- Support Lead can perform supported transitions such as: Resolved ->  Closed, Closed -> Reopened

# 11. What concurrency risk exists in your domain?
- Ticket assignment and workflow state changes.
- two support users could potentially try to assign the same ticket at almost the same time. Both requests might check: No active assignment exists before either one saves.

# 12. What did Jira reveal about your delivery process?
- Jira helped us move from simply building features to managing the project as a team.
We divided the work into three sprints and organized functionality into epics and stories
- It also gave us evidence of the development process rather than only showing the final code.

# 13. Show a bug ticket that became a regression test.
- We did not have an accepted bug ticket that required a regression test during the final phase.

# 14. Why did you make one ADR decision and what alternative did you reject?
We created ADR-001 for Ownership and Authorization Resolution because our system contains sensitive customer and support data.

We decided that checking only the user's role was not enough. For example, two users can both have the Customer role, but one customer should not be able to access another customer's ticket. The same applies to SupportAgents: an agent should only access tickets currently assigned to them.

So, we resolve the current user from the authenticated JWT claims and enforce both role and ownership or assignment rules inside the Application use cases, mainly in the command and query handlers.

We specifically rejected relying only on role-based authorization because it cannot distinguish between two users with the same role. We also rejected putting these checks directly inside Controllers because we wanted to keep the business and authorization rules inside the Application layer and keep Controllers thin.

This decision also makes the ownership rules reusable across different use cases and allows us to test important security scenarios, such as a customer trying to access another customer's ticket.

# 15. Where are production secrets configured?
Production secrets not committed to the repository.
For deployment, sensitive configuration such as: database connection strings,JWT secrets, and other credentials
should be configured through the hosting environment's environment variables 

# 16. What is the most important limitation in v1.0?
- One important limitation is that our attachment feature currently handles attachment metadata and storage keys, rather than implementing a complete production-grade file storage lifecycle.

# 17. What would Sprint 4 contain if the product continued?
1. Stronger concurrency protection : 
Add optimistic concurrency for tickets and assignments to prevent race conditions during simultaneous updates.
2. Complete attachment storage :
Integrate proper object storage and secure file download/upload handling.
3. More automated tests : 
Increase integration and security coverage, especially around concurrent requests and complex workflow transitions.
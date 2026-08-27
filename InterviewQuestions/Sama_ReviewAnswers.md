Review Answers 
-----------------

1- The hardest business invariant is ticket ownership and access control. A customer must only be able to access their own tickets, 
and an agent should only be able to work with tickets assigned to them. This is enforced in the Application layer inside the command and query 
handlers using `ICurrentUserService` to identify the authenticated user and applying ownership/assignment checks before returning or modifying data. 
The API also uses role-based authorization at the controller level.

2- The reassignment command is the most transaction-sensitive command because reassignment involves more than one related change. 
The previous assignment needs to be ended and a new assignment needs to be created while preserving the assignment history. If one operation succeeded and another failed,
the ticket could have inconsistent assignment information. Therefore, this workflow should be treated as atomic.

3- The GetTopCustomersByTicketCountQuery was designed as a projection. Instead of loading complete `CustomerProfile` and `Ticket` entities, 
it selects only the required information: `CustomerId`, `CustomerName`, and `TicketCount`. It also performs filtering, ordering, and pagination at the database level. 
This reduces unnecessary data loading and makes the reporting query more efficient.

4- The API gets the authenticated user's identity from the JWT claims through `ICurrentUserService`. The handlers use `currentUserService.UserId` rather than trusting a customer ID supplied by the client. 
For example, when a customer requests a ticket, the handler checks that the ticket belongs to the customer's profile. Therefore, Customer A cannot simply send Customer B's ID or ticket ID and access their private data.

5- The unique email constraint on `ApplicationUser` is a good example. The application and ASP.NET Identity validate email uniqueness, 
but the database also has a unique index on the email column. Therefore, even if application code accidentally attempts to create the same 
email twice, SQL Server rejects the duplicate record.

6- A possible race condition is during ticket assignment or reassignment. Two Support Leads could try to assign the same ticket to different agents at nearly the same time. 
This could result in conflicting active assignments or inconsistent history. The team addressed the workflow through controlled assignment logic and assignment history, 
and this is also an area that should receive stronger concurrency protection as a production-hardening improvement.

7- The ticket workflow was divided into smaller stories rather than treating the entire ticket lifecycle as one large story. For example, ticket intake, triage, assignment/reassignment, 
comments, status changes, resolution/closure, and customer history were handled as separate pieces. This made the work easier to estimate, assign, review, and demonstrate within the sprint structure.

8- One of the most important security issues we focused on was cross-customer ticket access. It demonstrated that simply protecting an endpoint with `[Authorize]` is not enough. 
A user can be authenticated and still not have permission to access a particular resource.

We added an integration test where Customer A attempts to access Customer B's ticket. The test expects a forbidden/not-found response. 
This ensures that a future change cannot accidentally remove the ownership check.

9- The architectural decision around Clean Architecture + CQRS is one of the clearest trade-offs. We chose to separate Domain, Application, Infrastructure, 
and API and put Commands and Queries in the Application layer. The alternative would have been putting database workflows and business 
logic directly into controllers or using large service classes.

The trade-off is that CQRS requires more files and structure, but it gives us clearer separation of responsibilities, easier testing, 
and thinner controllers. This was especially useful because our system has many different ticket commands and reporting queries.

10- For the second release, I would focus on production hardening and advanced support features.

The main improvements would be:

* More advanced SLA handling with business hours and pause windows.
* Automatic SLA breach notifications and escalation.
* Email notifications for assignment and status changes.
* Advanced ticket search and filtering.
* Stronger concurrency handling for assignment and status changes.
* More integration and performance tests.
* Real attachment storage rather than only attachment metadata.
* Customer satisfaction/rating functionality.
* More advanced reporting and monitoring.

The goal would be to move the platform from a strong v1 support workflow toward a more complete enterprise support system.

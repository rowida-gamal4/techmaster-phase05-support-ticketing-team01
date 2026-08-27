# 1. What is the hardest business invariant in the Support Ticketing Platform API and where is it enforced?
The hardest invariant is the ticket status workflow. A ticket cannot move from any status to any other status. For example, only a 'Resolved' ticket can be closed, and only a 'Closed' ticket can be reopened. These rules are enforced in the Application command handlers.

# 2. Which command required a transaction and why?
The ReassignTicket command is a good example because it deactivates the old assignment and creates a new assignment. These operations should happen together so we don't end up with a ticket having no active assignment or multiple active assignments.

# 3. Which query was designed as a projection instead of loading entities?
The SLA Risk Report and reporting queries use DTO projections/selected fields instead of returning complete entities. This avoids loading unnecessary data and makes the query more efficient.

# 4. How does the API resolve the current user and prevent cross-user access?
The API gets the current user's ID and role from the authenticated JWT through 'ICurrentUserService'. Then the Application handlers check ownership or assignment. For example, a customer can only access tickets where 'ticket.CustomerId' belongs to the current customer.

# 5. Which database constraint protects a rule even if application logic fails?
The database uses unique constraints/indexes for rules that must remain unique, such as preventing duplicate relationships where applicable. This gives us an extra layer of protection beyond application validation.

# 6. What race condition could happen in this domain and what did the team do about it?
A possible race condition is two users trying to assign or reassign the same ticket at almost the same time. Both requests could see the ticket as unassigned. We protect the workflow with application checks and database constraints where appropriate, although stronger concurrency handling would be a future improvement.

# 7. Which story was split because it was too large for one sprint?
The larger ticket workflow was divided into smaller stories such as assignment, reassignment, starting, resolving, and status changes. This allowed each part to be developed, tested, and reviewed separately.

# 8. Which bug created the most learning and what regression test prevents it now?
The most important learning came from security and ownership scenarios, especially making sure a user cannot access another user's ticket by changing an ID in the request. We added negative authorization/ownership tests to make sure these scenarios continue to return '403 Forbidden'.

# 9. What ADR best explains a trade-off your team made?
ADR-001: Ownership and Authorization Resolution.
We decided to resolve the current user from JWT claims and enforce role plus ownership/assignment rules in the Application layer. We rejected relying only on roles or putting the checks directly inside Controllers.

# 10. What would you implement next if this product received a second release cycle?
I would focus on stronger concurrency protection, notifications, better analytics, more automated tests, and additional production monitoring. I would also improve the customer and agent portals based on real user feedback.

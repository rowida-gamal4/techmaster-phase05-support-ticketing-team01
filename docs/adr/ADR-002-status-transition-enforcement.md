# ADR-002: Ticket Status Transition Enforcement

# Context: 
Tickets follow a controlled lifecycle. Allowing clients to change a ticket directly to any status could create invalid business states.

The main workflow is:

New → Assigned → InProgress → Resolved → Closed

Additional transitions such as cancellation and reopening are allowed only under specific business rules.

# Decision: 
The team will enforce ticket status transitions through explicit application/domain business rules. A command that changes the ticket status must validate the current status and the requested new status before making the change.

Invalid transitions will be rejected and will not update the ticket. Valid transitions will create a corresponding TicketStatusHistory record.

The API will not allow clients to freely assign arbitrary status values without transition validation.

# Alternatives: 
- Allow any status value sent by the client.
- Validate status transitions only inside Controllers.
- Store the transition rules only in the database.
- Use a dedicated domain/application transition rule to validate allowed states.

# Consequences: 
- Invalid workflows are prevented consistently.
- Status changes become easier to test.
- Status history provides an audit trail of important workflow changes.
- Adding a new status or transition requires updating the transition rules and related tests.
- The implementation contains more business logic than a simple status update.

# Status:
- Accepted
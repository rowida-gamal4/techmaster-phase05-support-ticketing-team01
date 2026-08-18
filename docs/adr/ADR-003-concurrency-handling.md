# ADR-003: Concurrency Handling for Ticket Updates

# Context: 
Multiple support users may work with the same ticket at the same time. For example, two SupportLeads could attempt to assign or reassign the same ticket concurrently.

Application-level checks alone cannot guarantee consistency because another request may modify the same record between the initial check and the database update.

# Decision: 
The team will use database-level protection together with application validation for concurrency-sensitive ticket operations.

Concurrency-sensitive updates will be designed so that conflicting changes are detected and handled safely. Where necessary, transactions will be used for multi-step operations such as assignment changes and history creation.

The application will not rely only on a previous read/check to guarantee that the final database state is valid.

# Alternatives: 
- Rely only on application-level checks before saving.
- Use database constraints only.
- Use optimistic concurrency with a concurrency token.
- Use transactions for all ticket operations regardless of need.
- Combine optimistic concurrency, database constraints, and transactions only where the business operation requires them.

# Consequences: 
- Important ticket and assignment data is better protected from race conditions.
- Multi-step business operations can remain consistent.
- Concurrency failures need to be handled and returned as appropriate API errors.
- The implementation and testing are more complex than simple CRUD operations.
- Not every operation will require a transaction; transactions will be used where multiple related changes must succeed or fail together.

# Status:
- Accepted
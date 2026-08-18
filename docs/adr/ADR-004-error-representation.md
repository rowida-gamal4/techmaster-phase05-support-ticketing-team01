# ADR-004: Consistent API Error Representation

# Context:
The Support Ticketing Platform contains multiple API operations that can fail because of validation errors, authentication and authorization failures, missing resources, business rule violations, conflicts, or unexpected server errors.

If each Controller handles errors differently, clients may receive inconsistent response structures and status codes.

# Decision:
The team will use centralized error handling with a consistent API error response format.

Expected HTTP status codes will include:

- 400 Bad Request for invalid input or validation failures.
- 401 Unauthorized when authentication is missing or invalid.
- 403 Forbidden when the authenticated user does not have permission.
- 404 Not Found when the requested resource does not exist.
- 409 Conflict when a business or data conflict prevents the operation.
- 500 Internal Server Error for unexpected server errors.

Unexpected exceptions will be handled through global error-handling middleware rather than duplicating exception handling in every Controller.

Sensitive implementation details, stack traces, database information, and secrets will not be exposed to API clients.

# Alternatives:
- Handle errors separately inside every Controller.
- Return different response structures from different endpoints.
- Use exceptions without centralized handling.
- Return generic 400 responses for most business and validation failures.

# Consequences:
- API clients receive predictable error responses.
- Error handling is easier to maintain and test.
- Controllers remain thin and focused on HTTP concerns.
- The team must maintain a consistent error contract across the API.
- Developers need to map business failures to the appropriate HTTP status codes.

# Status:#  Accepted
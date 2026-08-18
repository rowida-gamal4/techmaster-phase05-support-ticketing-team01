# ADR-001: Ownership and Authorization Resolution

# Context:  
The Support Ticketing Platform contains sensitive customer and support data. Customers must only access their own tickets, and SupportAgents must only access tickets assigned to them unless a broader role such as SupportLead or Admin allows it.

Checking only the user's role is not sufficient because two users can have the same role but different ownership or assignment permissions.

# Decision:  
The team will resolve the current user from the authenticated JWT claims and enforce both role and ownership/assignment rules in the Application use cases.

Customer access will be restricted to tickets belonging to the current customer. SupportAgent access will be restricted to currently assigned tickets. SupportLead and Admin access will follow their defined scope in the access matrix.

Controllers will not contain the main ownership/business authorization logic.

# Alternatives:  
- Rely only on role-based authorization.
- Put ownership checks directly inside Controllers.
- Allow clients to provide CustomerId or AgentId and trust the value.

# Consequences:  
- Cross-user access is harder to perform accidentally.
- Ownership rules remain close to the business use cases.
- Current-user resolution can be reused across commands and queries.
- Some use cases require additional ownership checks, which adds application logic.
- Security tests will be required for wrong-owner scenarios.

# Status:
- Accepted
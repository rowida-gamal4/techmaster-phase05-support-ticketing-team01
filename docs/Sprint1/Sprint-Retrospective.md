# Sprint 1 — Sprint Retrospective
# Project: Support Ticketing Platform
# Sprint: Sprint 1
# Team: Rowida + Sama

-----------------------------------------

# 1. What Went Well
# 1.1 Architecture was established early
The team started with the architecture rather than immediately creating controllers and CRUD endpoints.
Using Clean Architecture and CQRS gave us a clear separation between:
    • Domain logic.
    • Application use cases.
    • Infrastructure.
    • API.
This made it easier to add new use cases as the sprint progressed.

# 1.2 We focused on business workflows
Instead of implementing isolated CRUD operations, we implemented complete use cases such as:
    • Create Ticket.
    • Get My Tickets.
    • Cancel Ticket.
    • Assign Ticket.
    • Reassign Ticket.
    • Get My Assigned Tickets.
This helped us keep the implementation aligned with the actual user stories and acceptance criteria.

# 1.3 Authentication and current-user context became a strong foundation
Implementing JWT authentication and ICurrentUserService early helped the team enforce ownership and authorization correctly.
For example, customers do not provide their own CustomerId when creating or viewing tickets.
The system derives the identity from the authenticated token.

# 1.4 Database design supported the business rules
The team invested time in:
    • Relationships.
    • Foreign keys.
    • Constraints.
    • Indexes.
    • Fluent API configuration.
This reduced ambiguity when implementing the application workflows.

# 1.5 We identified and fixed integration issues during implementation
During the sprint, the team encountered issues involving:
    • EF Core migrations/schema synchronization.
    • Identity relationships.
    • JWT configuration.
    • Current-user claims.
    • Validation registration.
    • Ticket status history foreign keys.
These issues helped us better understand how the layers interact instead of treating each layer independently.

# 1.6 Team collaboration
Both team members contributed to the project across the architecture, infrastructure, domain, authentication, and ticket workflows.
We reviewed implementation decisions together and used the sprint requirements and acceptance criteria to guide development.

-----------------------------------------

# 2. What Could Be Improved
# 2.1 We should define some shared conventions earlier
Some implementation decisions were made while development was already underway, particularly around:
    • Error response structure.
    • Validation handling.
    • Exception handling.
    • Result/response patterns.
    • Naming conventions.
Improvement: Agree on these conventions before implementing the majority of the features.

# 2.2 Database changes should be synchronized earlier
Some EF Core/database issues occurred because the application model and database schema were temporarily out of sync.
Improvement:
Before integrating a feature:
    1. Update entity/model.
    2. Update Fluent API.
    3. Create migration.
    4. Apply migration.
    5. Verify the database.
    6. Then continue with the application workflow.

# 2.3 More automated tests should be added earlier
Manual Swagger testing helped us validate the workflows, but automated tests would have caught some issues earlier.
Improvement for upcoming sprints:
Add focused tests for important business rules such as:
    • Customer can only access own tickets.
    • Customer cannot cancel a ticket in a prohibited status.
    • Agent can only perform actions on assigned tickets.
    • Invalid status transitions are rejected.
    • Unauthorized users cannot access protected workflows.

# 2.4 Validation strategy should be standardized
We encountered a case where FluentValidation existed but was not automatically executed because a validation pipeline behavior had not been configured.
Improvement: Agree at the beginning of the project whether validation will be:
    • Executed through MediatR ValidationBehavior, or
    • Explicitly executed inside handlers.
For the next sprint, we should follow one consistent approach.

# 2.5 Evidence should be collected continuously
Some evidence was left until the end of the sprint.
Improvement: Capture evidence during each completed story:
    • Swagger request/response.
    • Database evidence where relevant.
    • Authorization/negative test.
    • Jira story completion.
    • Commit/PR evidence.
This makes the Sprint Review much easier to prepare.

-----------------------------------------

# 3. What We Learned
During Sprint 1, we learned that implementing a backend feature is not only about writing the endpoint.
A complete use case requires coordination between:
Requirement → Domain → Database → Application → Authorization → API → Error Handling → Testing
We also learned that authentication and authorization need to be considered when designing the use case, not added after the endpoint is finished.

# 4. Action Items for the Next Sprint
- Action                                                : Purpose
- Standardize validation approach                       : Avoid inconsistent validation behavior
- Add more automated tests                              : Catch business-rule regressions earlier
- Capture evidence per story                            : Make sprint review and demo preparation easier
- Keep DB migrations synchronized                       : Prevent schema/model mismatch
- Continue using current-user context                   : Prevent identity/ownership spoofing
- Define status-transition rules before implementation  : Make workflow behavior explicit
- Continue feature-based CQRS structure                 : Keep use cases isolated and maintainable

5. Sprint Retrospective Summary
- Keep
    • Clean Architecture.
    • CQRS/MediatR.
    • Feature-based organization.
    • Current-user context.
    • Business-rule-driven use cases.
    • Fluent API/database constraints.
    • Team code review and collaboration.
- Improve
    • Validation consistency.
    • Automated testing.
    • Database migration discipline.
    • Evidence collection.
    • Early agreement on shared coding conventions.
- Start
    • Writing tests alongside important business rules.
    • Capturing acceptance-criteria evidence while implementing each story.
    • Reviewing the database migration after every significant model change.
- Stop
    • Relying mainly on manual Swagger testing.
    • Making architectural conventions while already implementing features.
    • Leaving evidence collection until the end of the sprint.

-----------------------------------------

# Final Statement
Sprint 1 successfully delivered the foundation and first business workflows of the Support Ticketing Platform. The team established Clean Architecture, CQRS, EF Core persistence, authentication, authorization, customer and agent profiles, ticket intake, cancellation, categorization, priority, and assignment workflows. The sprint also exposed areas for improvement around validation consistency, automated testing, database synchronization, and evidence collection, which will be addressed as the project continues.
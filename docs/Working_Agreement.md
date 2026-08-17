# Support Ticketing Platform API
## Team Working Agreement

### 1. Team

| Member | Primary Role   | Additional Responsibility      |
|        |                |                                |
| Rowida | Tech Lead      | Documentation & Release Owner  |
| Sama   | Backend Engineer | Scrum Master & Quality Owner |

All team members remain active Backend Engineers and are responsible for implementing code.



## 2. Jira Workflow

All feature work must have a Jira story or task before implementation begins.

The agreed workflow is:

Backlog → Ready → In Progress → In Review → Testing → Done

If work is blocked, the issue should be marked as:

Blocked

Jira tickets should contain enough information for the team to understand:
- Business goal
- Acceptance criteria
- Authorization requirements
- Ownership rules
- Testing expectations



## 3. Git Branching

No direct feature development is allowed on 'main'.

Each feature or fix must use a separate branch.

Branches should be related to the corresponding Jira ticket whenever possible.

## 4. Commits

Commits should be small, meaningful and related to one logical change.


## 5. Pull Requests

Completed work must be submitted through a Pull Request.

A Pull Request should:

- Reference the Jira ticket.
- Describe what was implemented.
- Explain important business rules.
- Include relevant tests.
- Keep the change focused.
- Be reviewed by the other team member.

No feature should be merged without review.

## 6. Code Review

Each team member must review code written by the other member.

Reviews should consider:

- Correctness
- Business rules
- Authorization
- Ownership
- Security
- Database impact
- CQRS structure
- Validation
- Testing
- Maintainability

The purpose of review is improvement and shared understanding, not only approval.

## 7. Database Changes

Database changes must be coordinated between team members.

Any migration should be:

- Related to a Jira ticket.
- Included in the corresponding Pull Request.
- Reviewed before merging.
- Tested against the expected application behavior.

Shared database changes should not be made silently.


## 8. Architecture

The project follows Clean Architecture.

The main layers are:

- Domain
- Application
- Infrastructure
- API

Application use cases follow CQRS:

- Commands for state-changing operations.
- Queries for read operations.

Controllers should remain thin and should not contain business logic or direct database queries.



## 9. Testing

Critical business and security rules must be tested.

The team will prioritize tests for:

- Ownership violations
- Wrong roles
- Invalid status transitions
- Internal note visibility
- Assignment rules
- Closed-ticket behavior
- Invalid input
- Important business workflows

Serious bugs should receive regression tests when practical.



## 10. Communication

Team members should communicate when:

- A task is blocked.
- A shared file or contract is being changed.
- A database change may affect another feature.
- A Pull Request needs review.
- A requirement is unclear.

Blockers should not remain hidden until the end of a sprint.


## 11. Definition of Done

A story is considered Done only when:

- The required Command/Query is implemented.
- Validation is implemented.
- Business rules are enforced.
- Authorization and ownership are handled.
- Database changes are complete if required.
- Tests are added where appropriate.
- Pull Request is reviewed.
- Changes are merged into 'main'.
- Required API/evidence is updated.
- Jira ticket is moved to Done.


## 12. Team Principle

The team will prioritize:

- Build → Review → Test → Merge → Document

Both members are expected to understand the shared architecture and review work outside their own implementation.
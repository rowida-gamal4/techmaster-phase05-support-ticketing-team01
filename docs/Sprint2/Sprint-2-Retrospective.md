# Sprint 2 — Retrospective
# Keep
- Keep using small, focused CQRS handlers for each business use case.
- Separating customer comments, agent replies, status transitions, and SLA monitoring made the business rules easier to understand and allowed each team member to work on independent stories without putting unrelated logic into one large handler.
- The team should also keep using Jira to track stories, bugs, and unfinished work so that deferred work is visible instead of being forgotten.

# Stop
- Stop treating a successful API response as proof that the business rule is correct.
- Several issues showed that an endpoint can technically work while still having an incorrect business result—for example:
    - incorrect CreatedAt values affecting SLA calculations
- The team should stop relying only on the happy path and verify acceptance criteria and negative cases for every story.

# Start
- Start writing the main acceptance and negative test cases before considering a story Done.
- For every future story, we should verify:
    - Happy path
    - Unauthorized user
    - Wrong role
    - Wrong owner
    - Invalid state
    - Invalid input
- This should become part of the  Definition of Done rather than something checked only at the end of the sprint.

# Technical Debt
- Technical debt: strengthen automated integration and business-rule tests.
- The most important debt from Sprint 2 is test coverage around the ticket lifecycle.
- Tests should cover:
    - Customer can comment only on their own ticket.
    - Agent can reply only to assigned tickets.
    - Invalid status transitions are rejected.
    - Customer cannot reopen tickets when policy prohibits it.
    - Customer can not close another customer's ticket.
    - Lead can reopen a closed ticket.
    - Status history is created correctly.
    - SLA targets use the correct category/priority policy.
    - Resolved/Closed/Cancelled tickets are excluded from approaching-SLA results.
    - Internal comments are never exposed to customer queries.

# Team Health
- Work distribution
- Sprint 2 was divided by complete functional areas rather than individual isolated methods:
    - Conversation
        - S07 Customer public comment
        - S08 Agent public reply
        - S09 Internal note
    - Status Workflow
        - S10 Move to InProgress
        - S11 Resolve
        - S12 Close/Reopen
    - SLA & Escalation
        - S13 SLA monitoring
- This allowed each team member to own a coherent part of the domain while still reviewing each other's work.

# Review load
The team should continue reviewing each other's pull requests rather than having one person become the only reviewer. Each member should understand the business rules behind the features they review, especially authorization, ownership, status transitions, and data visibility.

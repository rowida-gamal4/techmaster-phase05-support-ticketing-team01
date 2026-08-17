# Support Ticketing Platform — Requirements

## 1. Purpose

This document defines the functional, security and business requirements for the Support Ticketing Platform.

The requirements are derived from the approved product scope and are used to guide Jira stories, CQRS use cases, implementation and testing.

Detailed database relationships are documented separately in the ERD.


# 2. Functional Requirements

## 2.1 Identity & Access

The system shall:

- Authenticate users using JWT.
- Support Admin, SupportLead, SupportAgent and Customer roles.
- Resolve the current user from the authenticated request.
- Enforce role-based authorization.
- Prevent inactive users or agents from performing prohibited operations.
- Prevent clients from supplying arbitrary customer or agent identities for current-user operations.


## 2.2 Ticket Intake

The system shall allow customers to:

- Create tickets.
- View their own tickets.
- Cancel eligible new tickets.

When a ticket is created:

- The customer is taken from the authenticated user.
- Required ticket data is validated.
- The initial status is assigned by the server.
- Priority follows the configured business rule.
- Ticket timestamps are controlled by the server.


## 2.3 Triage

Authorized SupportLead and Admin users shall be able to:

- Set ticket category.
- Set ticket priority.

Priority values must be controlled and validated.

Important classification changes must be auditable.


## 2.4 Assignment

The system shall allow authorized users to:

- Assign tickets to active agents.
- Reassign tickets.
- View assignment history.
- Allow agents to view their assigned queue.

An inactive agent cannot receive a new assignment.

Assignment changes must preserve historical information.


## 2.5 Conversation

The system shall support:

- Customer public comments.
- Agent public replies.
- Internal staff notes.

Internal notes must never be returned through customer-facing endpoints.

Customers cannot create internal notes.

Comments must follow ticket state and ownership rules.


## 2.6 Status Workflow

Tickets shall follow controlled status transitions.

Main lifecycle:

New -> Assigned-> InProgress-> Resolved-> Closed
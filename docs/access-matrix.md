Access Matrix — Support Ticketing Platform
-----------------------------------------


| Module / Resource                  | Admin                         | SupportLead                         | SupportAgent                               | Customer                           |
| ---------------------------------- | ----------------------------- | ----------------------------------- | ------------------------------------------ | ---------------------------------- |
| **Identity & Profiles**            | **Full** — manage users/roles | **View** support profiles           | **View Own** profile                       | **View/Edit Own** profile          |
| **Customer Profiles**              | **Full**                      | **View**                            | **View** when related to assigned ticket   | **View Own**                       |
| **Tickets – Create**               | Yes                           | Yes*                                | No                                         | **Yes – Own**                      |
| **Tickets – View**                 | **All**                       | **All**                             | **Assigned Only**                          | **Own Only**                       |
| **Tickets – Update Details**       | Full                          | Full                                | Limited / assigned                         | Limited / own                      |
| **Ticket Category**                | **Manage**                    | **Manage**                          | View                                       | View                               |
| **Ticket Priority**                | **Manage**                    | **Manage**                          | View                                       | View                               |
| **Ticket Assignment**              | **Assign / Reassign**         | **Assign / Reassign**               | View Own Assignment                        | No                                 |
| **Ticket Comments – Public**       | Full                          | Full                                | **Assigned Tickets**                       | **Own Tickets**                    |
| **Internal Notes**                 | View/Create                   | View/Create                         | **View/Create – Assigned**                 | **No Access**                      |
| **Attachments Metadata**           | Full                          | Full                                | Assigned Tickets                           | Own Tickets                        |
| **Status Changes**                 | Full                          | Full                                | **Assigned Tickets / Allowed transitions** | Limited / policy-based             |
| **Close Ticket**                   | Yes                           | Yes                                 | Yes – assigned                             | Policy-based                       |
| **Reopen Ticket**                  | Yes                           | Yes                                 | Policy-based                               | Policy-based / normally restricted |
| **Cancel Ticket**                  | Yes                           | Yes                                 | No / policy-based                          | **Own New Tickets**                |
| **Assignment History**             | View                          | View                                | Own Assigned Tickets                       | No                                 |
| **Status History**                 | View                          | View                                | Assigned Tickets                           | **Own Tickets**                    |
| **SLA Policies**                   | **Manage**                    | **View / Configure if permitted**   | View                                       | No                                 |
| **SLA Risk / Escalation**          | **Full**                      | **Full**                            | Assigned tickets only                      | No                                 |
| **Ticket Tags**                    | Manage                        | Manage                              | Use/View                                   | View if customer-visible           |
| **Agent Workload Reports**         | **Full**                      | **Full**                            | No                                         | No                                 |
| **Ticket Status/Priority Reports** | **Full**                      | **Full**                            | Limited / own queue                        | No                                 |
| **Resolution-Time Reports**        | **Full**                      | **Full**                            | Own/team scope                             | No                                 |
| **Customer Ticket History**        | **Full**                      | **Full**                            | Assigned tickets only                      | **Own Only**                       |
| **Audit Logs**                     | **Full**                      | **View relevant operational audit** | No / limited                               | No                                 |
| **System Configuration**           | **Full**                      | No                                  | No                                         | No                                 |

* If your implementation follows the core brief strictly, customer is the primary ticket-creation actor; 
* SupportLead/Admin creation can be treated as an administrative capability rather than a core requirement.

### Access Scope Rules

| Role             | Access Boundary                                                                                     |
| ---------------- | --------------------------------------------------------------------------------------------------- |
| **Admin**        | Global system scope                                                                                 |
| **SupportLead**  | Global ticket/team workload scope, but no system-wide user administration unless explicitly granted |
| **SupportAgent** | Assigned resources only                                                                             |
| **Customer**     | Own tickets and own customer data only                                                              |

These boundaries come directly from the project's role definitions. 

### Sensitive Modules

security-sensitive:

| Sensitive Resource            | Required Protection                                          |
| ----------------------------- | ------------------------------------------------------------ |
| **Internal Notes**            | Never returned by customer endpoints                         |
| **Ticket Assignment**         | Agent can only work assigned tickets                         |
| **Customer Tickets**          | Customer can access only own tickets                         |
| **Ticket History**            | Scope history to authorized ticket/customer                  |
| **Audit Logs**                | Staff/admin only; do not expose comment bodies unnecessarily |
| **SLA Data**                  | Staff reporting scope                                        |
| **Customer Profile**          | Prevent cross-customer access                                |
| **Priority / Status Changes** | Role authorization + valid workflow transition               |
| **User/Role Administration**  | Admin only                                                   |

The brief specifically states that internal notes must never reach customers, 
agents cannot work another agent's ticket without an authorized lead/admin policy, 
and current-user endpoints must not accept arbitrary customer/agent IDs. 

### API-Level Access Matrix

This is useful to include beside the main matrix because your project is an **ASP.NET Core API**:

| HTTP Endpoint                           | Admin	| Lead	|    Agent    | Customer	|
| --------------------------------------- | :---:	| :--:	| :---------: | :------:	|
| `POST /api/tickets`                     |   ✅	|   —	|      ❌	  |     ✅		|
| `GET /api/customers/me/tickets`         |   —		|   —	|      ❌	  |   ✅ Own	|
| `GET /api/agents/me/tickets`            |   —		|   —	| ✅ Own Queue	|     ❌	|
| `PUT /api/tickets/{id}/assign`          |   ✅	|   ✅  |      ❌      |     ❌    |
| `PUT /api/tickets/{id}/reassign`        |   ✅	|   ✅  |      ❌      |     ❌    |
| `PUT /api/tickets/{id}/priority`        |   ✅	|   ✅  |      ❌      |     ❌    |
| `POST /api/tickets/{id}/comments`       |   ✅	|   ✅  |  ✅ Assigned |   ✅ Own  |
| `POST /api/tickets/{id}/internal-notes` |   ✅	|   ✅  |  ✅ Assigned |     ❌    |
| `PUT /api/tickets/{id}/status`          |   ✅	|   ✅  | ✅ Assigned* |  Policy	|
| `PUT /api/tickets/{id}/reopen`          |   ✅	|   ✅  |    Policy   |  Policy		|
| `GET /api/reports/agent-workload`       |   ✅	|   ✅  |      ❌      |     ❌    |
| `GET /api/reports/sla-risk`             |   ✅	|   ✅  |      ❌      |     ❌    |

The API contract in the brief gives the same core mapping for ticket creation, customer tickets, 
agent queue, assignment, priority, comments, internal notes, status, reopen, workload and SLA reports. 

### Important Authorization Rules

security rules:

1. **Customer → own tickets only.**
2. **Agent → assigned tickets only.**
3. **Lead/Admin → broader ticket management scope.**
4. **Customer → never access internal notes.**
5. **Customer → cannot create internal notes.**
6. **Agent → cannot work on another agent's ticket without Lead/Admin authorization.**
7. **Only active agents can receive assignments.**
8. **Status changes must follow the allowed workflow.**
9. **Reopen is role/policy controlled.**
10. **Audit assignment, status and priority changes.**
11. **Current-user endpoints derive identity from the authenticated user/token rather than accepting arbitrary user IDs.**
12. **Historical ticket identity must remain even if a customer or agent is deleted/deactivated.**

These correspond closely to the project's business rules R01–R24. 


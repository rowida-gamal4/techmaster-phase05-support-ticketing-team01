# Production Deployment and Environment Configuration

## 1. Overview

The Support Ticketing Platform is an ASP.NET Core Web API built using Clean Architecture, CQRS, Entity Framework Core, SQL Server, 
ASP.NET Core Identity, and JWT authentication.

The production deployment uses:

* ASP.NET Core Web API
* SQL Server remote database
* Entity Framework Core migrations
* JWT authentication
* Environment-based configuration
* Swagger/OpenAPI for API documentation
* Global exception handling
* Identity and role seeding

Production API:

**http://supportticketingapi.runasp.net/**

---

## 2. Environment Configuration

The application supports environment-specific configuration through ASP.NET Core configuration.

The main environments are:

| Environment | Purpose                         |
| ----------- | ------------------------------- |
| Development | Local development and debugging |
| Testing     | Automated integration tests     |
| Production  | Deployed live API               |

The application determines the environment using the ASP.NET Core `ASPNETCORE_ENVIRONMENT` setting.

Production must use:

```text
ASPNETCORE_ENVIRONMENT=Production
```

---

## 3. Configuration Files

The application uses:

```text
appsettings.json
appsettings.Development.json
appsettings.Production.json
```

Environment-specific configuration overrides the base configuration when the corresponding environment is active.

Sensitive production configuration must not be committed to the repository.

Examples of sensitive values include:

* Production SQL Server connection strings
* JWT signing keys
* Database passwords
* Other authentication secrets

The repository should contain only safe configuration or placeholders.

---

## 4. Database Configuration

The application uses Entity Framework Core with SQL Server.

The database connection is configured through:

```text
ConnectionStrings:DefaultConnection
```

The application infrastructure reads the connection string from configuration:

```csharp
options.UseSqlServer(
    configuration.GetConnectionString("HostConnection"));
```

For production, the connection string points to the remote SQL Server database rather than the local development database.

The production database is therefore independent from the developer's local SQL Server instance.

---

## 5. Production Database

A remote SQL Server database is used for the deployed application.

The production database contains the application's EF Core schema and migration history.

The production database was updated using Entity Framework Core migrations.

The migration history includes the project's existing migrations, including the production release migration:

```text
20260821003256_InitialCreate
20260821144401_AddTriageAssignmentUpdates
20260822171930_UpdatingBaseEntity
20260822205648_AddResolutionNotes
20260826212243_ProductionRelease
```

The production database should be updated using migrations rather than manually changing the database schema.

Example command:

```powershell
dotnet ef database update `
    --project src\SupportTicketing.Infrastructure `
    --startup-project src\SupportTicketing.API
```

---

## 6. JWT Configuration

The API uses JWT Bearer authentication.

JWT configuration contains:

```text
Jwt:Key
Jwt:Issuer
Jwt:Audience
Jwt:ExpirationMinutes
```

Production JWT secrets must not be stored in source control.

The production JWT signing key should be supplied through the hosting provider's environment variables or secure configuration settings.

Example configuration structure:

```text
Jwt__Key=<PRODUCTION_SECRET>
Jwt__Issuer=<PRODUCTION_ISSUER>
Jwt__Audience=<PRODUCTION_AUDIENCE>
Jwt__ExpirationMinutes=60
```


---

## 7. Local Development Configuration

Local development uses a local SQL Server database.

A developer can configure a local connection string through local configuration or user secrets.

Example:

```text
Server=localhost;
Database=SupportTicketingDb;
Trusted_Connection=True;
TrustServerCertificate=True;
```

Local development values must not be confused with production configuration.

---

## 8. Testing Environment

Automated integration tests use the `Testing` environment.

The application contains a check that prevents normal production seed execution during integration tests:

```csharp
if (!app.Environment.IsEnvironment("Testing"))
{
    // Identity and application data seeding
}
```

This prevents the normal production/development seed process from interfering with automated tests.

Integration tests provide their own test data.

---

## 9. Identity and Role Seeding

When the application starts outside the Testing environment, the application initializes required Identity roles and application seed data.

The startup process executes:

```text
IdentitySeeder.SeedRolesAsync(...)
DataSeeder.SeedAsync(...)
```

This ensures that required roles and initial application data are available in the deployed environment.

The main application roles are:

```text
Admin
SupportLead
SupportAgent
Customer
```

---

## 10. Production Startup

The production application performs the following startup sequence:

1. Load application configuration.
2. Configure the Infrastructure layer.
3. Configure controllers and JSON serialization.
4. Configure Swagger.
5. Register application services.
6. Configure JWT authentication.
7. Initialize Identity roles and seed data when appropriate.
8. Configure Swagger/OpenAPI.
9. Configure global exception handling.
10. Enable HTTPS redirection.
11. Enable authentication.
12. Enable authorization.
13. Map API controllers.
14. Start the web application.

---

## 11. Security Configuration

Production configuration follows these rules:

* Production connection strings are not committed to Git.
* Production JWT signing keys are not committed to Git.
* Database passwords are not committed to Git.
* Secrets are supplied through hosting-provider configuration/environment variables.
* Authentication is handled using JWT.
* Authorization is enforced using application roles.
* Global exception handling prevents internal exception details from being returned to API clients.

Sensitive values must never be placed directly in source code.

---

## 12. Swagger / Live API

Swagger/OpenAPI is enabled by the deployed API.

Production API:

```text
http://supportticketingapi.runasp.net/
```

The deployment provides live API access that can be used as release evidence.

Swagger can be used to verify that the deployed API exposes the expected controllers and endpoints.

---

## 13. Deployment Process

The production deployment process is:

```text
Local Development
       ↓
Build
       ↓
Automated Tests
       ↓
EF Core Migration
       ↓
Production Configuration
       ↓
Publish ASP.NET Core API
       ↓
Deploy to Hosting Provider
       ↓
Configure Production Environment Variables
       ↓
Connect Remote SQL Server
       ↓
Run/verify Database Migrations
       ↓
Start API
       ↓
Verify Live Swagger/API
       ↓
Release Candidate Tag
```

---

## 14. Production Verification

After deployment, the following checks should be performed:

### API

* API starts successfully.
* Live API URL is reachable.
* Swagger loads successfully.
* Controllers are available.
* Authentication works.
* Authorization roles work.

### Database

* Remote SQL Server is reachable.
* EF Core migrations are applied.
* Required tables exist.
* Identity tables exist.
* Seeded roles/data are available.

### Security

* Production JWT secret is not present in Git.
* Production database credentials are not present in Git.
* Unauthorized requests are rejected.
* Role-based authorization works.
* Global exceptions return safe responses.

---

## 15. Secrets and Git

The following files/configuration must be reviewed before the final commit:

```text
appsettings.json
appsettings.Development.json
appsettings.Production.json
```

Production secrets must not be committed.
appsettings.json secrets must not be committed.

If local or production configuration contains credentials, those values must be removed from tracked files and supplied through secure environment configuration.

The `.gitignore` should prevent local secret/configuration files from being accidentally committed when appropriate.

```

and confirm that no database password, JWT signing key, or other secret is included.

---

## 16. Release Candidate

Before the final demonstration, the production-ready version should be committed and tagged as a release candidate.

Example:

```powershell
git add .
git commit -m "chore: prepare production release candidate"
git push origin <branch-name>
```

Create the release candidate tag:

---

## 17. Deployment Evidence

The following evidence should be available for the final demo:

1. Live API URL.
2. Live Swagger/OpenAPI page.
3. Remote SQL Server database evidence.
4. Successful EF Core migration evidence.
5. Production environment configuration evidence without exposing secrets.
6. Successful authentication/authorization request.
7. Successful business endpoint request.
8. Git release-candidate tag.
9. Automated test results.
10. Final Git commit.

Sensitive credentials must be hidden in screenshots and documentation.

---


## 18. Production Release Status

The Support Ticketing Platform has a deployed API available at:

**http://supportticketingapi.runasp.net/**

The project uses a remote SQL Server database and Entity Framework Core migrations for production database schema management.

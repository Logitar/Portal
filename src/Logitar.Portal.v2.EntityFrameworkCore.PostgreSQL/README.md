# Logitar.Portal.v2.EntityFrameworkCore.PostgreSQL

EntityFrameworkCore PostgreSQL store integration for Portal.

## Migrations

This project is setup to use migrations. All the commands below must be executed in the solution directory.

### Create a migration

Do not forget to provide a migration name!

`dotnet ef migrations add <YOUR_MIGRATION_NAME> --project src/Logitar.Portal.v2.EntityFrameworkCore.PostgreSQL --startup-project src/Logitar.Portal --context PortalContext`

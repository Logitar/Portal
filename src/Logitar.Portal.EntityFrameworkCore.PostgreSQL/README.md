# Logitar.Portal.EntityFrameworkCore.PostgreSQL

EntityFrameworkCore PostgreSQL store integration of the Portal system.

## Migrations

This project is setup to use migrations. All the commands below must be executed in the solution directory.

### Create a migration

To create a new migration, execute the following command. Do not forget to provide a migration name!

`dotnet ef migrations add <YOUR_MIGRATION_NAME> --project src/Logitar.Portal.EntityFrameworkCore.PostgreSQL --startup-project src/Logitar.Portal --context PortalContext`

### Remove a migration

To remove the latest unapplied migration, execute the following command.

`dotnet ef migrations remove --project src/Logitar.Portal.EntityFrameworkCore.PostgreSQL --startup-project src/Logitar.Portal --context PortalContext`

### Generate a script

To generate a script, execute the following command. Do not forget to provide a source migration name!

`dotnet ef migrations script <SOURCE_MIGRATION> --project src/Logitar.Portal.EntityFrameworkCore.PostgreSQL --startup-project src/Logitar.Portal --context PortalContext`

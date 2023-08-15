# Logitar.Portal.EntityFrameworkCore.SqlServer

TODO

## Migrations

This project is setup to use migrations. You must execute the following commands in the solution
directory.

### Create a new migration

Execute the following command to create a new migration. Do not forget to specify a migration name!

`dotnet ef migrations add <YOUR_MIGRATION_NAME> --context PortalContext --project src/Logitar.Portal.EntityFrameworkCore.SqlServer --startup-project src/Logitar.Portal`

###Generate a script

Execute the following command to generate a new script. Do not forget to specify a source migration
name!

`dotnet ef migrations script <FROM_MIGRATION_NAME> --context IdentityContext --project src/Logitar.Portal.EntityFrameworkCore.SqlServer --startup-project src/Logitar.Portal`

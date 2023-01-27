# Portal API

Identity management Web API.

## Migrations

This project is setup to use migrations. All the commands below must be executed in the solution directory.

### Create a migration

To create a new migration, execute the following command. Do not forget to specify a name!

`dotnet ef migrations add <YOUR_MIGRATION_NAME> --project src/Logitar.Portal.Infrastructure2 --startup-project src/Logitar.Portal.Web2`

### Remove a migration

To remove the latest new migration, execute the following command.

`dotnet ef migrations remove --project src/Logitar.Portal.Infrastructure2 --startup-project src/Logitar.Portal.Web2`

### Generate a script

To generate a script, execute the following command. You can optionally specify a _from_ migration name.

`dotnet ef migrations script <FROM_MIGRATION_NAME>? --project src/Logitar.Portal.Infrastructure2 --startup-project src/Logitar.Portal.Web2`

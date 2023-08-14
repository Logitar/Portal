# Portal

Identity provider system.

## Dependencies

To run the Portal application, you need a running database. Supported database providers are listed
in the `DatabaseProvider` enumeration.

The default database provider is `EntityFrameworkCorePostgreSQL`. You can override it by adding it
to your user secrets. Right-click the `Logitar.Portal` project, then click `Manage User Secrets`.
You can copy the variable in the `secrets.example.json` file and replace the `DatabaseProvider` key
by your desired database provider enumeration value.

You can use a Docker container by executing one of the following commands. The connection strings
are already configured in the `appsettings.Development.json` file.

### PostgreSQL

`docker run --name Logitar.Portal_postgres -e POSTGRES_PASSWORD=P@s$W0rD -p 7829:5432 -d postgres`

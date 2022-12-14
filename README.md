# Prerequisites

1. SQL Server
2. .NET 7 SDK to build and run

# Installation

First step is to migrate the database. Edit the `appsettings.json` file in the Server project by adding these lines:

```
"Memes": {
	"ConnectionString": "paste-connection-string"
}
```

Paste the connection string from the SQL Server database. It should look like this:

`Data Source=GRIZZLLY-PC;Initial Catalog=MemeIT;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False`

Next open your favourite terminal navigate to the Server project directory and run.

`dotnet ef database update`

This applies the migrations to the database.

Finally, to run the project do:

`dotnet run`
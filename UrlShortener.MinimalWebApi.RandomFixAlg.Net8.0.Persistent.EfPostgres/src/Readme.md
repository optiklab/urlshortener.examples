# App

This app based on .NET Core Minimal Web API 8.0 implements Base62 coding for generating "unique" shortened URLs.
It persists state in SQL Lit file db.

Problems known:
1. Getting the redirect link (from Short to Long URL) is not very performant. We must use Cache (or better, Distributed Cache here).
2. We might want better generation algorithm:
	- Pregenerate random values in advance
	- Use some other algorithm
	- etc.
3. A click counter for the tokens in question
4. Freeing up tokens based of off last activity

## Creating

dotnet new webapi -n "UrlShortener.MinimalWebApi.RandomFixAlg.Net7.0.Persistent.EfPostgres" -lang "C#" -au none -f net7.0

Add Docker Compose Support https://learn.microsoft.com/en-us/visualstudio/containers/tutorial-multicontainer?view=vs-2022

https://www.twilio.com/blog/containerize-your-aspdotnet-core-application-and-sql-server-with-docker

## DB storage

$> dotnet tool install --global dotnet-ef
$> dotnet add package Microsoft.EntityFrameworkCore.Sqlite -v 7.0.15
$> dotnet add package Microsoft.EntityFrameworkCore.Tools -v 7.0.15

$> dotnet ef database update   - just in case we need to create db file and check its content. But it is not needed cuz db will be created in container.

OR in VS Developer Console

$> NuGet\Install-Package Microsoft.EntityFrameworkCore.Sqlite -Version 7.0.15
$> NuGet\Install-Package Microsoft.EntityFrameworkCore.Tools -Version 7.0.15

## DB migrations

$> dotnet ef migrations add InitialCreate

## Database location

Database created at c:\Users\[USER]\AppData\Local\urlshortener.db


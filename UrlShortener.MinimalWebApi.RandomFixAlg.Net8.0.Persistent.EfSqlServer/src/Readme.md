# App

This app based on .NET Core Minimal Web API 8.0 implements Base62 coding for generating "unique" shortened URLs.
It persists state in SQL Server running in docker container.

Problems known:
1. We do execute migrations from the code, which is not safe for production purpose. 
   Not recommended to use. Better to use EF migration scripts like "$> dotnet ef database update ..." or even native SQL Scripts (or DbUp).
2. Getting the redirect link (from Short to Long URL) is not very performant. We must use Cache (or better, Distributed Cache here).
3. We might want better generation algorithm:
	- Pregenerate random values in advance
	- Use some other algorithm
	- etc.
4. We don't use any complex API DTO's, so we don't use any mapping here. But we might need it in real prod app.

## Project creation

dotnet new webapi -n "UrlShortener.MinimalWebApi.RandomFixAlg.Net7.0.Persistent.EfSqlServer" -lang "C#" -au none -f net7.0

Add Docker Compose Support https://learn.microsoft.com/en-us/visualstudio/containers/tutorial-multicontainer?view=vs-2022

https://www.twilio.com/blog/containerize-your-aspdotnet-core-application-and-sql-server-with-docker

## DB storage

$> dotnet tool install --global dotnet-ef
$> dotnet add package Microsoft.EntityFrameworkCore.SqlServer -v 7.0.13
$> dotnet add package Microsoft.EntityFrameworkCore.Tools -v 7.0.14

OR in VS Developer Console

$> NuGet\Install-Package Microsoft.EntityFrameworkCore.SqlServer -Version 7.0.13
$> NuGet\Install-Package Microsoft.EntityFrameworkCore.Tools -Version 7.0.14

## DB migrations

$> dotnet ef migrations add InitialCreate

## Database location

Docker Container

## How to Run

1. Select docker-compose as the Profile to run
2. Press F5
3. Wait a bit, as docker container will need to download packages with SQL Server and run everything


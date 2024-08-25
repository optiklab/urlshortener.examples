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

## DB Postgres storage

$> dotnet tool install --global dotnet-ef
$> dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL -v 8.0.4
$> dotnet add package Microsoft.EntityFrameworkCore.Tools -v 8.0.2

$> dotnet ef database update   - just in case we need to create db file and check its content. But it is not needed cuz db will be created in container.

OR in VS Developer Console

$> dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL -v 8.0.4
$> dotnet add package Microsoft.EntityFrameworkCore.Tools -v 8.0.2

## DB migrations

$> dotnet ef migrations add InitialCreate

## Database location

Docker Container

## Docker connection documentation

https://hub.docker.com/_/postgres

Examples:
$> docker pull postgres:alpine
$> docker run --name urlshortener-service -e POSTGRES_PASSWORD=my_super_pass -e POSTGRES_USER=sa -p 5432:5432 postgres

$> docker ps -a
$> docker rm CONTAINER_ID

$> docker images
$> docker images rm IMAGE_ID

## Datatypes in Postgres

https://www.postgresql.org/docs/current/datatype.html

https://maciejwalkowiak.com/blog/postgres-uuid-primary-key/

## DB migrations

$> dotnet ef migrations add InitialCreate

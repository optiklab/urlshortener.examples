# App

This app based on .NET Core 7.0 Orleans engine implements simple Guid coding for generating "unique" shortened URLs.

Url Shortener uses RAM as a storage (which is NOT persistent).

dotnet new web -o "UrlShortener.Orleans.WebApp" -f net7.0

dotnet add package Microsoft.Orleans.Server -v 7.0.0

Example:
https://localhost:7262/v1/shorten/https://google.com

Result:
https://localhost:7262/v1/go/4729B70E

Go:
https://localhost:7262/v1/go/4729B70E
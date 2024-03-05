# App

The idea of this Url Shortener is to use very simple Lite DB storage for URLs we WANT to shorten (i.e. FULL URLs) and external web service Hashids.net,
which is storing the mapping between Shortened URL and our Lite DB Row Id.

This is very weird combination, which violates a number of design principles, by adding unnecessary dependencies (where at least one of them is external and not controlled by us),
but it is still interesting in terms of using external service to generate unique shortened URLs.

Made based on https://dev.to/infobipdev/how-to-write-url-shortener-in-net5-weve-made-it-seem-easy-1-5c56.

The LIMIT of this application is a Int32 row DB Id number (LIMITING the number of records in DB) of URL stored in our Lite DB, 
which then used to generate Shortened URL by Hashids.net. In fact, Hashids.net generates an array of Int32.
But we store hashed URL in the database, so we can't use this to extend potential number of records.

## Compilation

dotnet new webapi -n "UrlShortener.WebApp.Hashi" -lang "C#" -au none -f net6.0

## Dependencies

dotnet add package LiteDB
dotnet add package Hashids.net




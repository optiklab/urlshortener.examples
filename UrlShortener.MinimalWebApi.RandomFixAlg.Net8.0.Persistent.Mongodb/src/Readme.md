# App

This app based on .NET Core Minimal Web API 8.0 implements Base62 coding for generating "unique" shortened URLs.
It persists state in Mongo DB.

Problems known:
1. Getting the redirect link (from Short to Long URL) is not very performant. We must use Cache (or better, Distributed Cache here).
2. We might want better generation algorithm:
	- Pregenerate random values in advance
	- Use some other algorithm
	- etc.
    - 
## Project creation

dotnet new webapi -n "UrlShortener.MinimalWebApi.RandomFixAlg.Net7.0.Persistent.Mongodb" -lang "C#" -au none -f net7.0

Add Docker Compose Support https://learn.microsoft.com/en-us/visualstudio/containers/tutorial-multicontainer?view=vs-2022

https://www.twilio.com/blog/containerize-your-aspdotnet-core-application-and-sql-server-with-docker

## Add Mongo DB

$> dotnet add package MongoDb.Driver

### Run Mongo in docker quickly

Run mongo db instance:
```bash
$>docker run -d -p 27017:27017 mongo
```

Check MongoDb tables:
```bash
Robo 3T
```

Read https://dev.to/arantespp/mongodb-shell-commands-running-on-docker-101-1l73
https://www.toptal.com/mongodb/interview-questions


### Run Mongo using Docker Compose

#### WORKS (Authenticated). 
#### Connnection string for the appsettings.json:"mongodb://user:password@urlshortenerdb:27017"
#### Connnection string for Robot3T viewer:"mongodb://user:password@localhost:27017"
version: '3.8'

services:
  urlshortenerwebapi:
    image: ${DOCKER_REGISTRY-}urlshortenerwebapi
    container_name: urlshortenerwebapi
    depends_on:
    - urlshortenerdb
    build:
      context: .
      dockerfile: src/Dockerfile
    ports:
      - "5001:443"
  urlshortenerdb:
    image: mongo:latest
    restart: always
    ports:
      - 27017:27017
    volumes:
      - ./data/db:/data/db
    environment:
      - MONGO_INITDB_ROOT_USERNAME=user
      - MONGO_INITDB_ROOT_PASSWORD=password
	  
#### WORKS (NON-Authenticated).
#### Connnection string for the appsettings.json:"mongodb://urlshortenerdb:27017"
#### Connnection string for Robot3T viewer:"mongodb://localhost:27017"

version: '3.8'

services:
  urlshortenerwebapi:
    image: ${DOCKER_REGISTRY-}urlshortenerwebapi
    container_name: urlshortenerwebapi
    depends_on:
    - urlshortenerdb
    build:
      context: .
      dockerfile: src/Dockerfile
    ports:
      - "5001:443"
  urlshortenerdb:
    image: mongo:latest
    restart: always
    ports:
      - 27017:27017
    volumes:
      - ./data/db:/data/db
	   
## Below are Mongo DB configurations that I tried and it didn't work, but should be working. To check...

#### TO CHECK

version: '3.8'

services:
  urlshortenerwebapi:
    image: ${DOCKER_REGISTRY-}urlshortenerwebapi
    container_name: urlshortenerwebapi
    depends_on:
    - urlshortenerdb
    build:
      context: .
      dockerfile: src/Dockerfile
    ports:
      - "5001:443"
  urlshortenerdb:
    image: mongodb/mongodb-community-server:6.0-ubi8
    restart: always
    ports:
      - 27017:27017
    environment:
      - MONGO_INITDB_ROOT_USERNAME=user
      - MONGO_INITDB_ROOT_PASSWORD=password
    volumes:
      - type: bind
        source: ./data
        target: /data/db
		
		

#### TO CHECK

version: '3.8'

services:
  urlshortenerwebapi:
    image: ${DOCKER_REGISTRY-}urlshortenerwebapi
    container_name: urlshortenerwebapi
    depends_on:
    - urlshortenerdb
    build:
      context: .
      dockerfile: src/Dockerfile
    ports:
      - "5001:443"
  mongo:
    image: mongo
    container_name: UrlShortenerMinimalDb
    restart: always
    environment:
      MONGO_INITDB_ROOT_USERNAME: user
      MONGO_INITDB_ROOT_PASSWORD: password
  mongo-express:
    image: mongo-express
    restart: always
    ports:
      - 8081:8081
    environment:
      ME_CONFIG_MONGODB_ADMINUSERNAME: user
      ME_CONFIG_MONGODB_ADMINPASSWORD: password
      ME_CONFIG_MONGODB_URL: mongodb://user:password@mongo:27017/
	  

#### TO CHECK

version: '3.8'

services:
  urlshortenerwebapi:
    image: ${DOCKER_REGISTRY-}urlshortenerwebapi
    container_name: urlshortenerwebapi
    depends_on:
    - urlshortenerdb
    build:
      context: .
      dockerfile: src/Dockerfile
    ports:
      - "5001:443"
  urlshortenerdb:
    image: mongo:latest
    restart: always
    ports:
      - 27017:27017
    environment:
      - MONGO_INITDB_ROOT_USERNAME=user
      - MONGO_INITDB_ROOT_PASSWORD=password
    volumes:
      - mongodb_data_container:/data/db
volumes:
  mongodb_data_container:
  
  
  
  
  
  

# Postgres

## Docker connection documentation

https://hub.docker.com/_/postgres

docker pull postgres:alpine

docker run --name urlshortener-service -e POSTGRES_PASSWORD=urlshortener -e POSTGRES_USER=sa -p 5432:5432 postgres

docker ps -a
docker rm CONTAINER_ID

docker images

docker images rm IMAGE_ID

## Datatypes in Postgres

https://www.postgresql.org/docs/current/datatype.html

https://maciejwalkowiak.com/blog/postgres-uuid-primary-key/



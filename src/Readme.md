## Setup of Services

From root of the project "~/wheelin-and-dealin"

run `docker-compose up -d` - this will start the database and the services

For each service

- change to the service directory
- install the dependencies `Automapper`, `EntityFrameworkCore.Design`, `EntityFrameworkCore.Tools`, `Npgsql.EntityFrameworkCore.PostgreSQL` (or any other db provider)

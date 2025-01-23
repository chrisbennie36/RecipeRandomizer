## Recipe Randomizer

# .Net Core based Microservice which performs the following functions:

- Adds/Removes preferences for users
- Fetches the configured selectable preferences to display on the front end
- Translates the configured recipe preferences based on the culture code of the user's device
- Creates a recipe query based on a random selection of the user's saved preferences, and returns the result from Google to the front end

# Libraries used

    - MediatR
    - EntityFramework
    - Refit
    - XUnit
    - AutoMapper
    - AWS CDK
    - Serilog
    - NSwag

# Database

Makes use of a PostgreSQL database and uses Migrations for a code first approach

# Architecture

Microservice is deployed in AWS as an ElasticBeanstalk application. The database is deployed in AWS as a Postgres 16.3 Engine with a security group which cotains an ingress rule allowing this Microservice to communicate with it

# Docker

Build image => docker build -f Dockerfile -t recipe-randomizer .
Run Container => docker run --rm -p 5179:8000 recipe-randomizer


# EF Core migrations commands
Perform the below in a powershell window, from within the API Web Application project directory
1.1) dotnet ef migrations add <MigrationName> (Generates a Migration file)
1.2) dotnet ef migrations script AddNewTables AddAuditTable (Generates an SQL script from the first specified migration to the second specified migration)
2.1) dotnet ef database update (Applies all Migrations)
2.2) dotnet ef database update <MigrationName> (Applies specific Migration) 
# Projet AmourConnect - BackEND - API in .NET Core

Dating site to match with a man VS woman and look for his love❤️

# CONFIG .env at the root of the project

```
#identifier for mail

EMAIL_USER=""
EMAIL_MDP=""
SERVICE="gmail"

# Auth Google
ClientId=""
ClientSecret=""

IP_NOW_FRONTEND="http://192.168.1.21:3000"

#Database
ConnectionDB="Host=localhost;Port=5433;Username=tchoulo;Password=123tchoulo123;Database=amourconnect_dev;"
```

# To start API

*⛔ Start the Database first before*

*If you have Docker URL DataBase =>*

```
Host=postgresdb;Port=5432;Username=tchoulo;Password=123tchoulo123;Database=amourconnect_dev;
```

Start API .NET Core
```
docker-compose -f .\compose.yaml up -d
```

OR not recommended, because the .env file will not be included in the build, you will need to put the ConnectionDb value directly in options.UseNpgsql();
```
docker build . -t apinetcore
```

```
docker run --network database_amour_connect -p 3110:5267 -e ASPNETCORE_URLS=http://+:5267 apinetcore
```

**Clean the caches if that doesn't work :**

```
docker builder prune --force
```

```
docker image prune --force
```

**Otherwise do this manually if you don't have Docker**

If you use .NET Core CLI
```
dotnet restore
```

Else if you use VS
```
nuget restore
```

*To play Migration*
```
dotnet tool install --global dotnet-ef && dotnet build && dotnet ef migrations add InitialCreate && dotnet ef database update
```

*Start*
```
dotnet run
```

*Upgrade all dependances*
```
dotnet upgrade-assistant
```
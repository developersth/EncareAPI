dotnet build -c Release

docker compose -f docker-compose-mssql.yml up --build -d 
docker compose -f docker-compose-mssql-arm.yml up --build -d

docker compose down -d
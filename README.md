dotnet build -c Release

docker compose -f docker-compose-mssql.yml up --build -d 
docker compose -f docker-compose-mssql-arm.yml up --build -d


docker compose up --build -d 
docker compose down -d

--add cert
dotnet dev-certs https --trust
dotnet dev-certs https -ep ./https/aspnetapp.pfx -p hoOkqJB9zu


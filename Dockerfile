# Use the official .NET SDK image to build the app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy the project file and restore any dependencies (via dotnet restore)
COPY ["EncareAPI.csproj", "EncareAPI/"]
RUN dotnet restore "EncareAPI/EncareAPI.csproj"

# Copy the entire project and build the app
COPY . .
WORKDIR /src/EncareAPI
RUN dotnet build "EncareAPI.csproj" -c Release -o /app/build

# Publish the app to the /app directory
RUN dotnet publish "EncareAPI.csproj" -c Release -o /app/publish

# Use the official .NET runtime image to run the app
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Copy the published files from the build container
COPY --from=build /app/publish .

# Expose the port the app will run on
EXPOSE 8080

# Set the entry point for the container
ENTRYPOINT ["dotnet", "EncareAPI.dll"]

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5297 7297

ENV ASPNETCORE_URLS="http://+:5297;https://+:7297"

USER app

FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG configuration=Release
WORKDIR /src
COPY ["EncareAPI.csproj", "./"]
RUN dotnet restore "EncareAPI.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "EncareAPI.csproj" -c $configuration -o /app/build

FROM build AS publish
ARG configuration=Release
RUN dotnet publish "EncareAPI.csproj" -c $configuration -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "EncareAPI.dll"]

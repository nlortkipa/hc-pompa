# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy all project files to maintain structure
COPY ["API/API.csproj", "API/"]
COPY ["Dal/Dal.csproj", "Dal/"]
COPY ["Data/Data.csproj", "Data/"]
COPY ["Data.PostgresMigrations/Data.PostgresMigrations.csproj", "Data.PostgresMigrations/"]
COPY ["Service/Service.csproj", "Service/"]

RUN dotnet restore "API/API.csproj"

# Copy the rest of the source code
COPY . .

WORKDIR "/src/API"
# Build the production artifact.
RUN dotnet build "API.csproj" -c Release -o /app/build

# Stage 2: Publish
FROM build AS publish
# Publish a portable production artifact.
RUN dotnet publish "API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 3: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:8080
COPY --from=publish /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "API.dll"]

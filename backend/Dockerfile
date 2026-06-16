# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy all project files to maintain structure
COPY ["API/API.csproj", "API/"]
COPY ["Dal/Dal.csproj", "Dal/"]
COPY ["Data/Data.csproj", "Data/"]
COPY ["Service/Service.csproj", "Service/"]

RUN dotnet restore "API/API.csproj"

# Copy the rest of the source code
COPY . .

WORKDIR "/src/API"
# Build in Debug mode for Development
RUN dotnet build "API.csproj" -c Debug -o /app/build

# Stage 2: Publish
FROM build AS publish
# Publish in Debug mode for Development
RUN dotnet publish "API.csproj" -c Debug -o /app/publish /p:UseAppHost=false

# Stage 3: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
# Set environment to Development
ENV ASPNETCORE_ENVIRONMENT=Development
COPY --from=publish /app/publish .
# Copy .env file if it exists in the build context
COPY .env* .
ENTRYPOINT ["dotnet", "API.dll"]

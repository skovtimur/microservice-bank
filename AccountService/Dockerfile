# .NET Core SDK
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

# Sets the working directory
WORKDIR /app

ARG ENVIRONMENT=Development

#COPY *.sln .
COPY ["AccountService/AccountService.csproj", "AccountService/"]

# .NET Core Restore1
RUN dotnet restore AccountService/AccountService.csproj

# Copy All Files
COPY . .
EXPOSE 80

# .NET Core Build and Publish
RUN dotnet publish AccountService/AccountService.csproj -c Development -o /publish

# ASP.NET Core Runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /publish ./

ENTRYPOINT ["dotnet", "AccountService.dll"]
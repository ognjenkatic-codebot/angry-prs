#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["AngryPullRequests.Console/AngryPullRequests.Console.csproj", "AngryPullRequests.Console/"]
COPY ["AngryPullRequests.Application/AngryPullRequests.Application.csproj", "AngryPullRequests.Application/"]
COPY ["AngryPullRequests.Infrastructure/AngryPullRequests.Infrastructure.csproj", "AngryPullRequests.Infrastructure/"]
COPY ["AngryPullRequests.Domain/AngryPullRequests.Domain.csproj", "AngryPullRequests.Domain/"]
RUN dotnet restore "AngryPullRequests.Console/AngryPullRequests.Console.csproj"
COPY . .
WORKDIR "/src/AngryPullRequests.Console"
RUN dotnet build "AngryPullRequests.Console.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AngryPullRequests.Console.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AngryPullRequests.Console.dll"]
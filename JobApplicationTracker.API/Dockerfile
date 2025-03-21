FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["JobApplicationTracker.API/JobApplicationTracker.API.csproj", "JobApplicationTracker.API/"]
COPY ["JobApplicationTracker.Application/JobApplicationTracker.Application.csproj", "JobApplicationTracker.Application/"]
COPY ["JobApplicationTracker.Domain/JobApplicationTracker.Domain.csproj", "JobApplicationTracker.Domain/"]
COPY ["JobApplicationTracker.Infrastructure/JobApplicationTracker.Infrastructure.csproj", "JobApplicationTracker.Infrastructure/"]
RUN dotnet restore "JobApplicationTracker.API/JobApplicationTracker.API.csproj"
COPY . .
WORKDIR "/src/JobApplicationTracker.API"
RUN dotnet build "JobApplicationTracker.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "JobApplicationTracker.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "JobApplicationTracker.API.dll"]
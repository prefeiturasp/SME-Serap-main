#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/runtime:3.1-buster-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["GestaoAvaliacao.Worker.StudentTestsSent/GestaoAvaliacao.Worker.StudentTestsSent.csproj", "GestaoAvaliacao.Worker.StudentTestsSent/"]
COPY ["GestaoAvaliacao.Worker.IoC/GestaoAvaliacao.Worker.IoC.csproj", "GestaoAvaliacao.Worker.IoC/"]
COPY ["GestaoAvaliacao.Worker.Database.MongoDB/GestaoAvaliacao.Worker.Database.MongoDB.csproj", "GestaoAvaliacao.Worker.Database.MongoDB/"]
COPY ["GestaoAvaliacao.MongoEntities/GestaoAvaliacao.MongoEntities.csproj", "GestaoAvaliacao.MongoEntities/"]
COPY ["GestaoAvaliacao.Entities/GestaoAvaliacao.Entities.csproj", "GestaoAvaliacao.Entities/"]
COPY ["GestaoAvaliacao.Util/GestaoAvaliacao.Util.csproj", "GestaoAvaliacao.Util/"]
COPY ["GestaoAvaliacao.Worker.IoC.Contracts/GestaoAvaliacao.Worker.IoC.Contracts.csproj", "GestaoAvaliacao.Worker.IoC.Contracts/"]
COPY ["GestaoAvaliacao.Worker.Repository.MongoDB/GestaoAvaliacao.Worker.Repository.MongoDB.csproj", "GestaoAvaliacao.Worker.Repository.MongoDB/"]
COPY ["GestaoAvaliacao.Worker.Database/GestaoAvaliacao.Worker.Database.csproj", "GestaoAvaliacao.Worker.Database/"]
COPY ["GestaoAvaliacao.Worker.Domain/GestaoAvaliacao.Worker.Domain.csproj", "GestaoAvaliacao.Worker.Domain/"]
COPY ["GestaoAvaliacao.Worker.Repository/GestaoAvaliacao.Worker.Repository.csproj", "GestaoAvaliacao.Worker.Repository/"]
RUN dotnet restore "GestaoAvaliacao.Worker.StudentTestsSent/GestaoAvaliacao.Worker.StudentTestsSent.csproj"
COPY . .
WORKDIR "/src/GestaoAvaliacao.Worker.StudentTestsSent"
RUN dotnet build "GestaoAvaliacao.Worker.StudentTestsSent.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "GestaoAvaliacao.Worker.StudentTestsSent.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GestaoAvaliacao.Worker.StudentTestsSent.dll"]
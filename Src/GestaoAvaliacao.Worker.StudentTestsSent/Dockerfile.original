#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/runtime:3.1-buster-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["GestaoAvaliacao.Worker.StudentTestSentWorker/GestaoAvaliacao.Worker.StudentTestSentWorker.csproj", "GestaoAvaliacao.Worker.StudentTestSentWorker/"]
RUN dotnet restore "GestaoAvaliacao.Worker.StudentTestSentWorker/GestaoAvaliacao.Worker.StudentTestSentWorker.csproj"
COPY . .
WORKDIR "/src/GestaoAvaliacao.Worker.StudentTestSentWorker"
RUN dotnet build "GestaoAvaliacao.Worker.StudentTestSentWorker.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "GestaoAvaliacao.Worker.StudentTestSentWorker.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GestaoAvaliacao.Worker.StudentTestSentWorker.dll"]
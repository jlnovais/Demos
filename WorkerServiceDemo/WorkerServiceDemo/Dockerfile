FROM mcr.microsoft.com/dotnet/core/runtime:3.0-buster-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:3.0-buster AS build
WORKDIR /src
COPY ["WorkerServiceDemo/WorkerServiceDemo.csproj", "WorkerServiceDemo/"]
RUN dotnet restore "WorkerServiceDemo/WorkerServiceDemo.csproj"
COPY . .
WORKDIR "/src/WorkerServiceDemo"
RUN dotnet build "WorkerServiceDemo.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "WorkerServiceDemo.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WorkerServiceDemo.dll"]
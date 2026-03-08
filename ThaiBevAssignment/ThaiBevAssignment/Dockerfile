# See https://aka.ms/customizecontainer
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
RUN apt-get update && apt-get install -y curl unzip && \
    curl -sSL https://aka.ms/getvsdbgsh | bash /dev/stdin -v latest -l /remote_debugger
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Project file is in the current build context (project folder)
COPY ["ThaiBevAssignment.csproj", "./"]
RUN dotnet restore "ThaiBevAssignment.csproj"

# Copy the rest of the sources (project root context)
COPY . .
WORKDIR "/src"

# Build the project (project file is at /src/ThaiBevAssignment.csproj or adjust path if needed)
RUN dotnet build "ThaiBevAssignment.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "ThaiBevAssignment.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ThaiBevAssignment.dll"]
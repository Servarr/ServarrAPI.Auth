FROM mcr.microsoft.com/dotnet/sdk:6.0.408-focal AS sdk
WORKDIR /app
ARG config=Release

COPY src ./src

RUN dotnet publish -c $config --no-self-contained src/*.sln

FROM mcr.microsoft.com/dotnet/aspnet:6.0.16-focal
WORKDIR /app
COPY --from=sdk /app/_output/net6.0/publish/. ./

# Docker Entry
ENTRYPOINT ["dotnet", "ServarrAuthAPI.dll"]

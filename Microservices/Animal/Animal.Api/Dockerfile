FROM microsoft/dotnet:2.2-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /src
COPY Animal.Api/MicroServices.Animal.Api.csproj Animal.Api/
COPY Animal.Api/Nuget.config ./
RUN dotnet restore Animal.Api/MicroServices.Animal.Api.csproj --configfile Nuget.config -nowarn:msb3202,nu1503
COPY . .
WORKDIR /src/Animal.Api
RUN dotnet build -c Release -o /app

FROM build AS publish
RUN dotnet publish -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "MicroServices.Animal.Api.dll"]

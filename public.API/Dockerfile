FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Public.API.csproj", "Public.API/"]
RUN dotnet restore "Public.API/Public.API.csproj"
COPY . ./Public.API/
WORKDIR "/src/Public.API"
RUN dotnet build "Public.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Public.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Public.API.dll"]
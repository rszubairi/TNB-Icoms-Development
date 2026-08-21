FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG TARGETARCH
WORKDIR /src
COPY ["TnbIcoms.Domain/TnbIcoms.Domain.csproj", "TnbIcoms.Domain/"]
COPY ["TnbIcoms.Infrastructure/TnbIcoms.Infrastructure.csproj", "TnbIcoms.Infrastructure/"]
COPY ["TnbIcoms.Application/TnbIcoms.Application.csproj", "TnbIcoms.Application/"]
COPY ["TnbIcoms.Api/TnbIcoms.Api.csproj", "TnbIcoms.Api/"]
RUN dotnet restore "TnbIcoms.Api/TnbIcoms.Api.csproj" -a $TARGETARCH
COPY . .
WORKDIR "/src/TnbIcoms.Api"
RUN dotnet build "TnbIcoms.Api.csproj" -c Release -o /app/build -a $TARGETARCH

FROM build AS publish
ARG TARGETARCH
RUN dotnet publish "TnbIcoms.Api.csproj" -c Release -o /app/publish -a $TARGETARCH

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TnbIcoms.Api.dll"]

#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.
FROM mcr.microsoft.com/dotnet/aspnet:6.0.15 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0.407 AS build
WORKDIR /src
COPY ["src/PTTDigital.Authentication.Api/PTTDigital.Authentication.Api.csproj", "src/PTTDigital.Authentication.Api/"]
COPY ["src/PTTDigital.Authentication.Application/PTTDigital.Authentication.Application.csproj", "src/PTTDigital.Authentication.Application/"]
COPY ["src/Infrastructures/PTTDigital.Authentication.Data/PTTDigital.Authentication.Data.csproj", "src/Infrastructures/PTTDigital.Authentication.Data/"]
COPY ["src/PTTDigital.Authentication.Common/PTTDigital.Authentication.Common.csproj", "src/PTTDigital.Authentication.Common/"]
RUN dotnet restore "src/PTTDigital.Authentication.Api/PTTDigital.Authentication.Api.csproj"
COPY . .
WORKDIR "/src/src/PTTDigital.Authentication.Api"
RUN dotnet build "PTTDigital.Authentication.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PTTDigital.Authentication.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PTTDigital.Authentication.Api.dll"]
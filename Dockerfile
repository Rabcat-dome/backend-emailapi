#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0.14-alpine3.17-amd64 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["src/PTTDigital.Email.Api/PTTDigital.Email.Api.csproj", "src/PTTDigital.Email.Api/"]
COPY ["src/PTTDigital.Email.Application/PTTDigital.Email.Application.csproj", "src/PTTDigital.Email.Application/"]
COPY ["src/Infrastructures/PTTDigital.Email.Data.SqlServer/PTTDigital.Email.Data.SqlServer.csproj", "src/Infrastructures/PTTDigital.Email.Data.SqlServer/"]
COPY ["src/PTTDigital.Email.Common/PTTDigital.Email.Common.csproj", "src/PTTDigital.Email.Common/"]
COPY ["src/Infrastructures/PTTDigital.Email.Data/PTTDigital.Email.Data.csproj", "src/Infrastructures/PTTDigital.Email.Data/"]
RUN dotnet restore "src/PTTDigital.Email.Api/PTTDigital.Email.Api.csproj"
COPY . .
WORKDIR "/src/src/PTTDigital.Email.Api"
RUN dotnet build "PTTDigital.Email.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PTTDigital.Email.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PTTDigital.Email.Api.dll"]
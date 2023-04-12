FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /build
COPY / .
RUN dotnet publish "src/PTTDigital.Email.Api/PTTDigital.Email.Api.csproj" -c Release -o /build/publish

FROM mcr.microsoft.com/dotnet/aspnet:6.0
RUN apt-get update \
  && rm -rf /var/lib/apt/lists/*
WORKDIR /app
COPY --from=build /build/publish .
ENV ASPNETCORE_URLS="http://*:3000" TZ=Asia/Bangkok
ENV ASPNETCORE_ENVIRONMENT="Development"
EXPOSE 3000
ENTRYPOINT ["dotnet", "PTTDigital.Email.Api.dll"]
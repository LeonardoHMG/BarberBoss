FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /app

COPY src/ .

WORKDIR /app/BarberBoss.Api

RUN dotnet restore

RUN dotnet publish -c Release -o /app/out
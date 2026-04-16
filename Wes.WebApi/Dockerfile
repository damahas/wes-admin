#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443
COPY ./bin/Release/net8.0 .
COPY ./dist ../front
ENTRYPOINT ["dotnet", "Wes.WebApi.dll"]
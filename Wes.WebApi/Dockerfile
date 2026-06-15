FROM mcr.microsoft.com/dotnet/aspnet:10.0-alpine AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443
RUN apk add --no-cache nginx
COPY ./bin/Release/net10.0 .
COPY ../Zero.UI/dist ../front
COPY ../Zero.UI/nginx.conf /etc/nginx/
COPY ../Zero.WebApi/UploadFile /app/UploadFile
ENTRYPOINT ["nginx"]
ENTRYPOINT ["dotnet", "Wes.WebApi.dll"]
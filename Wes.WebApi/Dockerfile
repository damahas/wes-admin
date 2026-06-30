FROM mcr.microsoft.com/dotnet/aspnet:10.0-alpine AS base
WORKDIR /app
EXPOSE 80
EXPOSE 8088
ENV ASPNETCORE_URLS=http://+:8088

RUN apk add --no-cache nginx

# API 发布输出（构建上下文 = 解决方案根目录）
COPY ./Wes.WebApi/bin/Release/net10.0/publish/ ./

# 前端静态文件
COPY ./Wes.UI/dist /front/

# Nginx 配置
COPY ./Wes.UI/nginx.conf /etc/nginx/nginx.conf

# 上传文件目录
COPY ./Wes.WebApi/UploadFile /app/UploadFile/

# nginx 后台运行 + dotnet 前台运行
ENTRYPOINT ["sh", "-c", "nginx && exec dotnet Wes.WebApi.dll"]
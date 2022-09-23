#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0-bullseye-slim AS base
RUN apt-get update && apt-get install -y libgdiplus
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM node:lts-slim AS npmbuild
COPY . .
WORKDIR "/web"
RUN export NODE_OPTIONS=--openssl-legacy-provider
RUN npm install -S --registry https://registry.npm.taobao.org --legacy-peer-deps
RUN npm run build

FROM mcr.microsoft.com/dotnet/sdk:6.0-bullseye-slim AS restore-all
COPY . .
COPY --from=npmbuild /web/dist /GensouSakuya.QQGuildLiveChat.App/web
RUN dotnet restore "qqguildlivechat.sln"

FROM restore-all AS build-livechat
RUN dotnet build "qqguildlivechat.sln" -c Release 

FROM build-livechat AS publish-livechat
RUN dotnet publish "qqguildlivechat.sln" -c Release

FROM base AS livechat
WORKDIR /app
COPY --from=publish-livechat /GensouSakuya.QQGuildLiveChat.App/bin/Release/net6.0/publish livechat/

WORKDIR /app/livechat
ENTRYPOINT [ "dotnet", "GensouSakuya.QQGuildLiveChat.App.dll" ]
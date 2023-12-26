﻿FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["ContactsAPI/ContactsAPI.csproj", "ContactsAPI/"]
RUN dotnet restore "ContactsAPI/ContactsAPI.csproj"
COPY . .
WORKDIR "/src/ContactsAPI"
RUN dotnet build "ContactsAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ContactsAPI.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ContactsAPI.dll"]

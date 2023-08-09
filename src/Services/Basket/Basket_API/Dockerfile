#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Services/Basket/Basket_API/Basket_API.csproj", "Services/Basket/Basket_API/"]
COPY ["BuildingBlocks/EventBus_Message/EventBus_Message.csproj", "BuildingBlocks/EventBus_Message/"]
RUN dotnet restore "Services/Basket/Basket_API/Basket_API.csproj"
COPY . .
WORKDIR "/src/Services/Basket/Basket_API"
RUN dotnet build "Basket_API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Basket_API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Basket_API.dll"]
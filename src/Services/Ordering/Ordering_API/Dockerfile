#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Services/Ordering/Ordering_API/Ordering_API.csproj", "Services/Ordering/Ordering_API/"]
COPY ["BuildingBlocks/EventBus_Message/EventBus_Message.csproj", "BuildingBlocks/EventBus_Message/"]
COPY ["Services/Ordering/Ordering_Application/Ordering_Application.csproj", "Services/Ordering/Ordering_Application/"]
COPY ["Services/Ordering/Ordering_Domain/Ordering_Domain.csproj", "Services/Ordering/Ordering_Domain/"]
COPY ["Services/Ordering/Ordering_Infrastructure/Ordering_Infrastructure.csproj", "Services/Ordering/Ordering_Infrastructure/"]
RUN dotnet restore "Services/Ordering/Ordering_API/Ordering_API.csproj"
COPY . .
WORKDIR "/src/Services/Ordering/Ordering_API"
RUN dotnet build "Ordering_API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Ordering_API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Ordering_API.dll"]
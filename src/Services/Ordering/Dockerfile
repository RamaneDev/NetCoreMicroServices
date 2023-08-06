#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
COPY ["Ordering_API/Ordering_API.csproj", "Ordering_API/"]
COPY ["Ordering_Application/Ordering_Application.csproj", "Ordering_Application/"]
COPY ["Ordering_Domain/Ordering_Domain.csproj", "Ordering_Domain/"]
COPY ["Ordering_Infrastructure/Ordering_Infrastructure.csproj", "Ordering_Infrastructure/"]
RUN dotnet restore "Ordering_API/Ordering_API.csproj"
COPY . .
WORKDIR "/Ordering_API"
RUN dotnet build "Ordering_API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Ordering_API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Ordering_API.dll"]
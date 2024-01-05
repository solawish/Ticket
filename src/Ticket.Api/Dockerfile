#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["src/Ticket.Api/Ticket.Api.csproj", "src/Ticket.Api/"]
COPY ["src/Ticket.Application/Ticket.Application.csproj", "src/Ticket.Application/"]
COPY ["src/ddddocrsharp/ddddocrsharp.csproj", "src/ddddocrsharp/"]
COPY ["src/Ticket.Domain/Ticket.Domain.csproj", "src/Ticket.Domain/"]
COPY ["src/Ticket.Infrastructure/Ticket.Infrastructure.csproj", "src/Ticket.Infrastructure/"]
RUN dotnet restore "src/Ticket.Api/Ticket.Api.csproj"
COPY . .
WORKDIR "/src/src/Ticket.Api"
RUN dotnet build "Ticket.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Ticket.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Ticket.Api.dll"]
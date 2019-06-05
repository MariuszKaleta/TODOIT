FROM mcr.microsoft.com/dotnet/core/aspnet:2.2-stretch-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:2.2-stretch AS build
WORKDIR /src
COPY ["ManyForMany/ManyForMany.csproj", "ManyForMany/"]
RUN dotnet restore "ManyForMany/ManyForMany.csproj"
COPY . .
WORKDIR "/src/ManyForMany"
RUN dotnet build "ManyForMany.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "ManyForMany.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "ManyForMany.dll"]
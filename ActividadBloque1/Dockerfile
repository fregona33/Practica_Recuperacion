FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ActividadBloque1/ActividadBloque1.csproj ActividadBloque1/
RUN dotnet restore ActividadBloque1/ActividadBloque1.csproj

COPY ActividadBloque1/ ActividadBloque1/
RUN dotnet publish ActividadBloque1/ActividadBloque1.csproj -c Release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/runtime:10.0
WORKDIR /app
ENV LANG=es_ES.UTF-8

COPY --from=build /app .
ENTRYPOINT ["dotnet", "ActividadBloque1.dll"]
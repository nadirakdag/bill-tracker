FROM mcr.microsoft.com/dotnet/sdk:5.0-alpine AS build
WORKDIR /src
COPY ["src/API/API.csproj", "API/"]
COPY ["src/Application/Application.csproj", "Application/"]
COPY ["src/Domain/Domain.csproj", "Domain/"]
COPY ["src/Infrastructure/Infrastructure.csproj", "Infrastructure/"]
RUN dotnet restore "API/API.csproj"  --runtime alpine-x64
COPY . .
WORKDIR "src/API/"
RUN dotnet build "API.csproj" -c Release -o /app/build  --runtime alpine-x64

FROM build AS publish
RUN dotnet publish "API.csproj" -c Release -o /app/publish --no-restore --runtime alpine-x64 /p:PublishTrimmed=true /p:PublishSingleFile=true

FROM mcr.microsoft.com/dotnet/runtime-deps:5.0-alpine AS final
EXPOSE 80
ENV ASPNETCORE_ENVIRONMENT="production"
WORKDIR /app
COPY --from=publish /app/publish .
HEALTHCHECK --interval=30s --timeout=60s --retries=3 CMD wget --no-verbose --tries=1 --spider http://localhost/health || exit
ENTRYPOINT ["./API", "--urls", "http://0.0.0.0:80"]
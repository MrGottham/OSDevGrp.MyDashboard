FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS builder
WORKDIR /src

# Copy .sln and .csproj files and restore as distinct layers
COPY OSDevGrp.MyDashboard.sln .
COPY OSDevGrp.MyDashboard.Core/OSDevGrp.MyDashboard.Core.csproj ./OSDevGrp.MyDashboard.Core/
COPY OSDevGrp.MyDashboard.Core.Tests/OSDevGrp.MyDashboard.Core.Tests.csproj ./OSDevGrp.MyDashboard.Core.Tests/
COPY OSDevGrp.MyDashboard.Web/OSDevGrp.MyDashboard.Web.csproj ./OSDevGrp.MyDashboard.Web/
COPY OSDevGrp.MyDashboard.Web.Tests/OSDevGrp.MyDashboard.Web.Tests.csproj ./OSDevGrp.MyDashboard.Web.Tests/
RUN dotnet restore

# Copy everything else and build app
COPY . .
WORKDIR /src/OSDevGrp.MyDashboard.Web
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS runtime
WORKDIR /app
COPY --from=builder /src/OSDevGrp.MyDashboard.Web/out .

ENV Authentication:Reddit:ClientId=[TBD] Authentication:Reddit:ClientSecret=[TBD]

EXPOSE 80

ENTRYPOINT ["dotnet", "OSDevGrp.MyDashboard.Web.dll"]
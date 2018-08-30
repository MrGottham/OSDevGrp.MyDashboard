FROM microsoft/aspnetcore-build AS builder
WORKDIR /src

# Copy .sln and .csproj files and restore as distinct layers
COPY *.sln .
COPY OSDevGrp.MyDashboard.Core/*.csproj ./OSDevGrp.MyDashboard.Core/
COPY OSDevGrp.MyDashboard.Web/*.csproj ./OSDevGrp.MyDashboard.Web/
RUN dotnet restore

# Copy everything else and build app
COPY OSDevGrp.MyDashboard.Core/. ./OSDevGrp.MyDashboard.Core/
COPY OSDevGrp.MyDashboard.Web/. ./OSDevGrp.MyDashboard.Web/
WORKDIR /src/OSDevGrp.MyDashboard.Web
RUN dotnet publish -c Release -o out

FROM microsoft/aspnetcore
WORKDIR /app
COPY --from=builder /src/OSDevGrp.MyDashboard.Web/out ./

EXPOSE 80
ENTRYPOINT ["dotnet", "OSDevGrp.MyDashboard.Web.dll"]
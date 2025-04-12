FROM mcr.microsoft.com/dotnet/sdk:9.0 AS builder
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

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
RUN apt-get update
RUN apt-get -y upgrade
RUN apt-get install -y supervisor openssh-server

WORKDIR /app
COPY --from=builder /src/OSDevGrp.MyDashboard.Web/out .

ENV DOTNET_RUNNING_IN_CONTAINER=true
ENV ASPNETCORE_HTTP_PORTS=8080

# Downgrade the SECLEVEL to 1 so we can communicate with the Version2 RSS Feed 
RUN sed -i 's/DEFAULT@SECLEVEL=2/DEFAULT@SECLEVEL=1/g' /etc/ssl/openssl.cnf

# Setup OpenSSH server
ARG sshPassword=[TBD]
RUN mkdir /var/run/sshd
RUN echo "root:${sshPassword}" | chpasswd
RUN sed -i "s/#PermitRootLogin prohibit-password/PermitRootLogin yes/g" /etc/ssh/sshd_config
RUN sed -i "s/#Port 22/Port 2222/g" /etc/ssh/sshd_config

# Setup Supervisor
RUN mkdir -p /var/log/supervisor
COPY supervisord.conf /etc/supervisor/conf.d/supervisord.conf

EXPOSE 80 2222

ENTRYPOINT ["/usr/bin/supervisord"]
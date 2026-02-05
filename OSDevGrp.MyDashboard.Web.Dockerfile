FROM mcr.microsoft.com/dotnet/sdk:10.0 AS builder
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

# Build the application
WORKDIR /src/OSDevGrp.MyDashboard.Web
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
RUN apt-get update \
    && apt-get -y upgrade \
    && apt-get install -y supervisor openssh-server sudo \
    && apt-get install -y locales

RUN sed -i "s/^# *\(da_DK\)/\1/" /etc/locale.gen
RUN dpkg-reconfigure --frontend=noninteractive locales
RUN update-locale LANG=da_DK.UTF-8
ENV LANG=da_DK.UTF-8
ENV LANGUAGE=da_DK.da
ENV TZ=Europe/Copenhagen

ARG appUserGroup
ARG nonRootUser
ARG nonRootPassword
ENV NON_ROOT_USER=${nonRootUser}
RUN groupadd ${appUserGroup}
RUN useradd -m -g ${appUserGroup} ${nonRootUser}
RUN usermod -a -G tty ${nonRootUser}
RUN usermod -a -G sudo ${nonRootUser}
RUN echo "${nonRootUser}:${nonRootPassword}" | chpasswd 

RUN mkdir -p /var/log/supervisor
RUN chmod g+rwx /var/run && chgrp ${appUserGroup} /var/run
RUN chmod g+rwx /var/log/supervisor && chgrp ${appUserGroup} /var/log/supervisor

ARG sshPassword
RUN mkdir /var/run/sshd
RUN echo "root:${sshPassword}" | chpasswd
RUN sed -i "s/#PermitRootLogin prohibit-password/PermitRootLogin yes/g" /etc/ssh/sshd_config
RUN sed -i "s/#Port 22/Port 2222/g" /etc/ssh/sshd_config

RUN echo "#!/bin/sh\nprintf \"${nonRootPassword}\"" > /usr/local/bin/askpw
RUN chmod 750 /usr/local/bin/askpw && chgrp ${appUserGroup} /usr/local/bin/askpw
ENV SUDO_ASKPASS=/usr/local/bin/askpw

# Downgrade the SECLEVEL to 1 so we can communicate with the Version2 RSS Feed 
RUN sed -i 's/DEFAULT@SECLEVEL=2/DEFAULT@SECLEVEL=1/g' /etc/ssl/openssl.cnf

ENV DOTNET_RUNNING_IN_CONTAINER=true
ENV ASPNETCORE_HTTP_PORTS=8080

WORKDIR /app
COPY --from=builder /src/OSDevGrp.MyDashboard.Web/out .
RUN chmod g+rwx /app && chgrp ${appUserGroup} /app

COPY supervisord.conf /etc/supervisor/conf.d/supervisord.conf

EXPOSE 8080 2222

USER ${nonRootUser}

ENTRYPOINT ["/usr/bin/supervisord", "-n", "-c", "/etc/supervisor/conf.d/supervisord.conf"]
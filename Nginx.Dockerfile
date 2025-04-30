FROM emberstack/openssl AS certificatebuilder
WORKDIR /certificate

COPY Nginx/certificate.conf certificate.conf

ARG certificateCommonName
ARG certificateCountryName
ARG certificateStateOrProvinceName
ARG certificateLocalityName
ARG certificateOrganizationName
ARG certificateOrganizationalUnitName
ARG certificateDns1
ARG certificateDns2
ARG certificateDns3
ARG certificatePassword

RUN sed -i "s/\[certificateDns1\]/${certificateDns1}/g" certificate.conf
RUN sed -i "s/\[certificateDns2\]/${certificateDns2}/g" certificate.conf
RUN sed -i "s/\[certificateDns3\]/${certificateDns3}/g" certificate.conf

RUN openssl req -x509 -nodes -days 365 -newkey rsa:4096 -keyout ${certificateCommonName}.key -out ${certificateCommonName}.crt -config certificate.conf -subj "/C=${certificateCountryName}/ST=${certificateStateOrProvinceName}/L=${certificateLocalityName}/O=${certificateOrganizationName}/OU=${certificateOrganizationalUnitName}/CN=${certificateCommonName}" -passin pass:${certificatePassword}
RUN openssl pkcs12 -export -out ${certificateCommonName}.pfx -inkey ${certificateCommonName}.key -in ${certificateCommonName}.crt -password pass:${certificatePassword}

FROM nginx:latest AS webserver
COPY Nginx/nginx.conf /etc/nginx/conf.d/default.conf

ARG certificateCommonName
COPY --from=certificatebuilder /certificate/${certificateCommonName}.crt /etc/ssl/certs/localhost.crt
COPY --from=certificatebuilder /certificate/${certificateCommonName}.key /etc/ssl/private/localhost.key
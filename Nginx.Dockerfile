FROM emberstack/openssl AS certificatebuilder
WORKDIR /certificate

COPY Nginx/localhost.conf localhost.conf

ARG certificateSubject
ARG certificatePassword
RUN openssl req -x509 -nodes -days 365 -newkey rsa:2048 -keyout localhost.key -out localhost.crt -config localhost.conf -subj "${certificateSubject}" -passin pass:${certificatePassword}
RUN openssl pkcs12 -export -out localhost.pfx -inkey localhost.key -in localhost.crt -password pass:${certificatePassword}

FROM nginx:latest AS webserver
COPY Nginx/nginx.conf /etc/nginx/nginx.conf
COPY --from=certificatebuilder /certificate/localhost.crt /etc/ssl/certs/localhost.crt
COPY --from=certificatebuilder /certificate/localhost.key /etc/ssl/private/localhost.key
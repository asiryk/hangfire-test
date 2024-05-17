FROM mysql:latest

ENV MYSQL_ROOT_PASSWORD=your_password
ENV MYSQL_DATABASE=hangfire_db

EXPOSE 3306

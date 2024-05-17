## Run docker mysql db

```shell
docker run --name mysql-hangfire -e MYSQL_ROOT_PASSWORD=password -e MYSQL_DATABASE=hangfire_db -p 3306:3306 -d mysql:latest
```

## Run docker compose

```shell
docker-compose build
docker-compose up -d
```

## MySql

```shell
mysql -u root -p
USE hangfire_db;
SHOW TABLES;
```

## Swagger

http://localhost:5555/swagger/index.html

## Dashboard

http://localhost:5555/hangfire

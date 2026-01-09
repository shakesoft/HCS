docker network create hc --label=hc
docker-compose -f containers/rabbitmq.yml up -d
docker-compose -f containers/redis.yml up -d
docker-compose -f containers/minio.yml up -d
exit $LASTEXITCODE
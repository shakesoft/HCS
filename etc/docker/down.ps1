docker-compose -f containers/rabbitmq.yml down
docker-compose -f containers/redis.yml down
docker-compose -f containers/minio.yml down
exit $LASTEXITCODE
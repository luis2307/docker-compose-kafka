# docker-compose-kafka
This is a docker compose file for kafka up.
```
docker compose up -d
```
```
docker compose down
```


# Docker Commands for Running Kafka

## 1. Create a Docker Network

First, create a Docker network for Kafka and Zookeeper to communicate.

```sh
docker network create kafka-network
```
## 2. Run Zookeeper
Run the Zookeeper container, which Kafka uses for coordination.
```sh
docker run -d \
  --name zookeeper \
  --network kafka-network \
  -p 2181:2181 \
  -e ALLOW_ANONYMOUS_LOGIN=yes \
  bitnami/zookeeper:3.9.2
```

## 3. Run Kafka
Run the Kafka container with the necessary environment variables for configuration.
```sh
docker run -d \
  --name kafka \
  --network kafka-network \
  -p 9092:9092 \
  -p 9093:9093 \
  -e KAFKA_ADVERTISED_HOST_NAME=kafka \
  -e KAFKA_CFG_ZOOKEEPER_CONNECT=zookeeper:2181 \
  -e KAFKA_CFG_ADVERTISED_LISTENERS=PLAINTEXT://localhost:9092,INTERNAL://kafka:9093 \
  -e KAFKA_CFG_LISTENER_SECURITY_PROTOCOL_MAP=PLAINTEXT:PLAINTEXT,INTERNAL:PLAINTEXT \
  -e KAFKA_CFG_LISTENERS=PLAINTEXT://0.0.0.0:9092,INTERNAL://0.0.0.0:9093 \
  -e KAFKA_INTER_BROKER_LISTENER_NAME=INTERNAL \
  -e ALLOW_PLAINTEXT_LISTENER=yes \
  -e KAFKA_LOG_RETENTION_HOURS=1 \
  -e KAFKA_LOG_RETENTION_BYTES=2097152 \
  -e KAFKA_AUTO_CREATE_TOPICS_ENABLE=true \
  -e KAFKA_DELETE_TOPIC_ENABLE=true \
  -e KAFKA_CLEANUP_POLICY=delete \
  bitnami/kafka:3.7.0
```
##4. Run Kafdrop
Run the Kafdrop container to provide a web interface for managing Kafka.
```sh
docker run -d \
  --name kafdrop \
  --network kafka-network \
  -p 9000:9000 \
  -e KAFKA_BROKERCONNECT=kafka:9093 \
  obsidiandynamics/kafdrop:4.0.1
```

## 5. For Stop and Remove Containers
```sh
docker stop kafdrop kafka zookeeper;
docker rm kafdrop kafka zookeeper;
docker network rm kafka-network;
```

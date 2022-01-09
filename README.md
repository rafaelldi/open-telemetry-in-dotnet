# OpenTelemetry in .NET

1. To start application, run the command:
```
docker-compose up -d
```

2. After that, navigate to [Grafana](http://localhost:3000/explore?orgId=1&left=%5B%22now-15m%22,%22now%22,%22Loki%22,%7B%22refId%22:%22A%22,%22expr%22:%22%7Bservice_name%3D%5C%22MyService%5C%22%7D%22%7D%5D) to see logs.
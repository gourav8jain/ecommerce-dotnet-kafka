# Quick Start Guide

This guide will help you get the Event-Driven Mini E-Commerce System up and running quickly.

## Prerequisites

### Required Software
- **.NET 8.0 SDK**: [Download here](https://dotnet.microsoft.com/download/dotnet/8.0)
- **Docker Desktop**: [Download here](https://www.docker.com/products/docker-desktop)
- **Azure CLI**: [Download here](https://docs.microsoft.com/en-us/cli/azure/install-azure-cli)
- **kubectl**: [Download here](https://kubernetes.io/docs/tasks/tools/)
- **Git**: [Download here](https://git-scm.com/)

### Azure Account
- **Azure Subscription**: Required for cloud deployment
- **Azure DevOps**: For CI/CD pipeline (optional)

## Local Development Setup

### 1. Clone the Repository
```bash
git clone <repository-url>
cd ecommerce-dotnet-kafka
```

### 2. Start Infrastructure Services
```bash
# Start Kafka, SQL Server, and Redis
docker-compose up -d zookeeper kafka sqlserver redis
```

Wait for all services to be healthy:
```bash
docker-compose ps
```

### 3. Build the Solution
```bash
# Restore packages
dotnet restore

# Build the solution
dotnet build
```

### 4. Run Database Migrations
```bash
# Product Service
dotnet ef database update --project src/Services/ProductService

# Order Service
dotnet ef database update --project src/Services/OrderService

# Payment Service
dotnet ef database update --project src/Services/PaymentService
```

### 5. Run the Services

#### Option A: Run Individually
```bash
# Terminal 1 - Product Service
dotnet run --project src/Services/ProductService

# Terminal 2 - Order Service
dotnet run --project src/Services/OrderService

# Terminal 3 - Payment Service
dotnet run --project src/Services/PaymentService

# Terminal 4 - Notification Service
dotnet run --project src/Services/NotificationService

# Terminal 5 - API Gateway
dotnet run --project src/Services/ApiGateway
```

#### Option B: Run with Docker Compose
```bash
# Build and run all services
docker-compose up --build
```

### 6. Verify the Setup

#### Check Service Health
```bash
# Product Service
curl http://localhost:5001/health

# Order Service
curl http://localhost:5002/health

# Payment Service
curl http://localhost:5003/health

# Notification Service
curl http://localhost:5004/health

# API Gateway
curl http://localhost:5000/health
```

#### Access Swagger Documentation
- Product Service: http://localhost:5001/swagger
- Order Service: http://localhost:5002/swagger
- Payment Service: http://localhost:5003/swagger
- API Gateway: http://localhost:5000/swagger

#### Kafka Manager
- URL: http://localhost:9000
- Add cluster: `zookeeper:2181`

## Testing the System

### 1. Create a Product
```bash
curl -X POST "http://localhost:5001/api/products" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Sample Product",
    "description": "A sample product for testing",
    "price": 29.99,
    "stockQuantity": 100,
    "category": "Electronics",
    "brand": "Sample Brand"
  }'
```

### 2. Get Products
```bash
curl "http://localhost:5001/api/products"
```

### 3. Create an Order
```bash
curl -X POST "http://localhost:5002/api/orders" \
  -H "Content-Type: application/json" \
  -d '{
    "customerId": "123e4567-e89b-12d3-a456-426614174000",
    "items": [
      {
        "productId": "product-id-here",
        "quantity": 2
      }
    ]
  }'
```

### 4. Process Payment
```bash
curl -X POST "http://localhost:5003/api/payments" \
  -H "Content-Type: application/json" \
  -d '{
    "orderId": "order-id-here",
    "amount": 59.98,
    "paymentMethod": "CreditCard",
    "cardNumber": "4111111111111111"
  }'
```

## Azure Deployment

### 1. Login to Azure
```bash
az login
```

### 2. Set Environment Variables
```bash
export RESOURCE_GROUP="ecommerce-rg"
export LOCATION="East US"
export AKS_CLUSTER_NAME="ecommerce-aks"
export ACR_NAME="ecommerceacr"
```

### 3. Run Deployment Script
```bash
# Make script executable
chmod +x scripts/deploy-azure.sh

# Run deployment
./scripts/deploy-azure.sh
```

### 4. Verify Deployment
```bash
# Get AKS credentials
az aks get-credentials --resource-group $RESOURCE_GROUP --name $AKS_CLUSTER_NAME

# Check pods
kubectl get pods -n ecommerce

# Check services
kubectl get services -n ecommerce

# Get external IP
kubectl get service api-gateway -n ecommerce
```

## Development Workflow

### 1. Making Changes
```bash
# Create feature branch
git checkout -b feature/new-feature

# Make changes
# ...

# Run tests
dotnet test

# Commit changes
git add .
git commit -m "Add new feature"

# Push to remote
git push origin feature/new-feature
```

### 2. Running Tests
```bash
# Unit tests
dotnet test tests/Unit/

# Integration tests
dotnet test tests/Integration/

# E2E tests
dotnet test tests/E2E/
```

### 3. Code Quality
```bash
# Run code analysis
dotnet build --verbosity normal

# Check for vulnerabilities
dotnet list package --vulnerable
```

## Monitoring and Debugging

### 1. Application Logs
```bash
# View service logs
docker-compose logs product-service
docker-compose logs order-service
docker-compose logs payment-service
```

### 2. Kafka Topics
```bash
# List topics
docker exec kafka kafka-topics --list --bootstrap-server localhost:9092

# View messages in topic
docker exec kafka kafka-console-consumer \
  --bootstrap-server localhost:9092 \
  --topic product-events \
  --from-beginning
```

### 3. Database
```bash
# Connect to SQL Server
docker exec -it sqlserver /opt/mssql-tools/bin/sqlcmd \
  -S localhost -U sa -P YourStrong@Passw0rd
```

## Troubleshooting

### Common Issues

#### 1. Port Already in Use
```bash
# Find process using port
lsof -i :5001

# Kill process
kill -9 <PID>
```

#### 2. Docker Issues
```bash
# Restart Docker
docker-compose down
docker-compose up -d
```

#### 3. Database Connection Issues
```bash
# Check SQL Server status
docker-compose logs sqlserver

# Restart SQL Server
docker-compose restart sqlserver
```

#### 4. Kafka Issues
```bash
# Check Kafka status
docker-compose logs kafka

# Restart Kafka
docker-compose restart kafka
```

### Performance Issues

#### 1. Memory Usage
```bash
# Check container memory usage
docker stats
```

#### 2. Database Performance
```bash
# Check database connections
docker exec sqlserver /opt/mssql-tools/bin/sqlcmd \
  -S localhost -U sa -P YourStrong@Passw0rd \
  -Q "SELECT * FROM sys.dm_exec_connections"
```

## Next Steps

### 1. Explore the Codebase
- Review the architecture documentation
- Understand the event-driven patterns
- Study the CQRS implementation

### 2. Add Features
- Implement new microservices
- Add new event types
- Extend the API

### 3. Production Deployment
- Set up Azure DevOps pipeline
- Configure monitoring and alerting
- Implement security best practices

### 4. Testing
- Write unit tests for new features
- Add integration tests
- Implement E2E test scenarios

## Support

### Documentation
- [Architecture Guide](./architecture.md)
- [API Documentation](./api.md)
- [Deployment Guide](./deployment.md)

### Issues
- Create GitHub issues for bugs
- Use discussions for questions
- Check existing issues first

### Community
- Join our Discord server
- Follow our blog
- Attend our meetups

---

**Happy coding! 🚀** 
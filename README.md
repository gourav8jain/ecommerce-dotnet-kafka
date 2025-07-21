# Event-Driven Mini E-Commerce System with .NET 8.0, Kafka, and Azure

A modern, scalable e-commerce system built with .NET 8.0, Apache Kafka for event-driven architecture, and Azure cloud services with Azure DevOps CI/CD pipeline.

## 🏗️ Architecture Overview

### Microservices Architecture
- **Product Service**: Manages product catalog and inventory
- **Order Service**: Handles order processing and management
- **Payment Service**: Processes payments and transactions
- **Notification Service**: Sends notifications via email/SMS
- **API Gateway**: Single entry point for all client requests

### Event-Driven Communication
- **Apache Kafka**: Message broker for event streaming
- **Event Sourcing**: Maintains event history for audit trails
- **CQRS**: Command Query Responsibility Segregation pattern

### Azure Services Integration
- **Azure Service Bus**: Alternative message broker
- **Azure SQL Database**: Primary data storage
- **Azure Cosmos DB**: NoSQL for product catalog
- **Azure Key Vault**: Secrets management
- **Azure Application Insights**: Monitoring and logging
- **Azure Container Registry**: Docker image storage

## 🚀 Features

### Core E-Commerce Features
- Product catalog with categories and search
- Shopping cart management
- Order processing with status tracking
- Payment processing (stripe integration)
- User authentication and authorization
- Real-time inventory updates
- Order notifications

### Technical Features
- Event-driven microservices
- RESTful APIs with OpenAPI/Swagger
- Health checks and monitoring
- Distributed tracing
- Rate limiting and caching
- Database migrations
- Unit and integration tests

## 📁 Project Structure

```
ecommerce-dotnet-kafka/
├── src/
│   ├── Services/
│   │   ├── ProductService/
│   │   ├── OrderService/
│   │   ├── PaymentService/
│   │   ├── NotificationService/
│   │   └── ApiGateway/
│   ├── Shared/
│   │   ├── Common/
│   │   ├── Events/
│   │   └── Messaging/
│   └── Web/
│       └── WebApp/
├── infrastructure/
│   ├── docker/
│   ├── k8s/
│   └── azure/
├── tests/
├── docs/
└── scripts/
```

## 🛠️ Technology Stack

### Backend
- **.NET 8.0**: Core framework
- **ASP.NET Core**: Web API framework
- **Entity Framework Core**: ORM
- **Apache Kafka**: Message broker
- **MediatR**: CQRS implementation
- **AutoMapper**: Object mapping
- **FluentValidation**: Input validation

### Frontend
- **React**: Frontend framework
- **TypeScript**: Type safety
- **Material-UI**: UI components
- **Redux Toolkit**: State management

### Infrastructure
- **Docker**: Containerization
- **Kubernetes**: Orchestration
- **Azure DevOps**: CI/CD
- **Azure Cloud**: Cloud platform

## 🚀 Quick Start

### Prerequisites
- .NET 8.0 SDK
- Docker Desktop
- Apache Kafka (local or cloud)
- Azure CLI
- Azure DevOps account

### Local Development
```bash
# Clone the repository
git clone <repository-url>
cd ecommerce-dotnet-kafka

# Start Kafka and dependencies
docker-compose up -d

# Run all services
dotnet run --project src/Services/ProductService/ProductService.csproj
dotnet run --project src/Services/OrderService
dotnet run --project src/Services/PaymentService
dotnet run --project src/Services/NotificationService
dotnet run --project src/Services/ApiGateway

# Run the web application
dotnet run --project src/Web/WebApp
```

### Azure Deployment
```bash
# Deploy to Azure
./scripts/deploy-azure.sh

# Deploy to Kubernetes
kubectl apply -f infrastructure/k8s/
```

## 📊 Monitoring and Observability

- **Application Insights**: Performance monitoring
- **Azure Monitor**: Infrastructure monitoring
- **Kafka Manager**: Message broker monitoring
- **Health Checks**: Service health monitoring

## 🔧 Configuration

### Environment Variables
```bash
# Kafka Configuration
KAFKA_BOOTSTRAP_SERVERS=localhost:9092
KAFKA_TOPIC_ORDERS=orders
KAFKA_TOPIC_PAYMENTS=payments

# Azure Configuration
AZURE_SERVICE_BUS_CONNECTION_STRING=<connection-string>
AZURE_SQL_CONNECTION_STRING=<connection-string>
AZURE_KEY_VAULT_URL=<key-vault-url>

# Application Configuration
ASPNETCORE_ENVIRONMENT=Development
LOG_LEVEL=Information
```

## 🧪 Testing

```bash
# Run unit tests
dotnet test tests/Unit/

# Run integration tests
dotnet test tests/Integration/

# Run end-to-end tests
dotnet test tests/E2E/
```

## 📈 Performance

- **Response Time**: < 200ms for API calls
- **Throughput**: 1000+ requests/second
- **Availability**: 99.9% uptime
- **Scalability**: Auto-scaling based on load

## 🔒 Security

- **JWT Authentication**: Secure API access
- **HTTPS**: Encrypted communication
- **Azure Key Vault**: Secure secrets management
- **Input Validation**: Prevent injection attacks
- **Rate Limiting**: Prevent abuse

## 📚 Documentation

- [API Documentation](./docs/api.md)
- [Architecture Guide](./docs/architecture.md)
- [Deployment Guide](./docs/deployment.md)
- [Development Guide](./docs/development.md)

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Add tests
5. Submit a pull request

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🆘 Support

For support and questions:
- Create an issue in the repository
- Contact the development team
- Check the documentation

---

**Built with ❤️ using .NET 8.0, Kafka, and Azure**

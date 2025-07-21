# 🛒 Event-Driven E-Commerce System with .NET 9.0

A modern, scalable microservices-based e-commerce platform built with .NET 9.0, Apache Kafka, and Azure cloud services. This system demonstrates event-driven architecture, CQRS pattern, and modern development practices.

![.NET 9.0](https://img.shields.io/badge/.NET-9.0-512BD4?style=for-the-badge&logo=.net)
![C#](https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=c-sharp&logoColor=white)
![Apache Kafka](https://img.shields.io/badge/Apache_Kafka-231F20?style=for-the-badge&logo=apache-kafka&logoColor=white)
![Docker](https://img.shields.io/badge/Docker-2496ED?style=for-the-badge&logo=docker&logoColor=white)
![Kubernetes](https://img.shields.io/badge/Kubernetes-326CE5?style=for-the-badge&logo=kubernetes&logoColor=white)
![Azure](https://img.shields.io/badge/Azure-0089D6?style=for-the-badge&logo=microsoft-azure&logoColor=white)

## 📋 Table of Contents

- [Overview](#overview)
- [Architecture](#architecture)
- [Features](#features)
- [Technology Stack](#technology-stack)
- [Project Structure](#project-structure)
- [Quick Start](#quick-start)
- [Development Setup](#development-setup)
- [API Documentation](#api-documentation)
- [Deployment](#deployment)
- [Monitoring](#monitoring)
- [Testing](#testing)
- [Contributing](#contributing)
- [License](#license)

## 🎯 Overview

This Event-Driven E-Commerce System is a comprehensive microservices platform that demonstrates modern software architecture patterns. The system is built with .NET 9.0 and uses Apache Kafka for asynchronous communication between services, implementing the Event-Driven Architecture pattern.

### Key Highlights

- **Microservices Architecture**: Independent, scalable services
- **Event-Driven Communication**: Apache Kafka for asynchronous messaging
- **CQRS Pattern**: Separation of read and write operations
- **Modern .NET 9.0**: Latest framework with performance improvements
- **Cloud-Native**: Designed for Azure deployment
- **Containerized**: Docker support for easy deployment
- **Real-time Dashboard**: React-based monitoring interface

## 🏗️ Architecture

### System Components

```
┌─────────────────┐    ┌─────────────────┐    ┌─────────────────┐
│   WebApp        │    │  API Gateway    │    │   Load Balancer │
│   (React)       │◄──►│   (Future)      │◄──►│   (Azure)       │
└─────────────────┘    └─────────────────┘    └─────────────────┘
         │                       │                       │
         ▼                       ▼                       ▼
┌─────────────────┐    ┌─────────────────┐    ┌─────────────────┐
│  Product        │    │     Order       │    │    Payment      │
│  Service        │◄──►│    Service      │◄──►│    Service      │
└─────────────────┘    └─────────────────┘    └─────────────────┘
         │                       │                       │
         ▼                       ▼                       ▼
┌─────────────────┐    ┌─────────────────┐    ┌─────────────────┐
│ Notification    │    │   Apache Kafka  │    │   Azure SQL     │
│ Service         │◄──►│   (Events)      │◄──►│   Database      │
└─────────────────┘    └─────────────────┘    └─────────────────┘
```

### Microservices

| Service | Port | Description | Key Features |
|---------|------|-------------|--------------|
| **Product Service** | 5000 | Product catalog management | CRUD operations, inventory, search |
| **Order Service** | 5005 | Order processing & management | Order lifecycle, status tracking |
| **Payment Service** | 5003 | Payment processing | Stripe integration, payment methods |
| **Notification Service** | 5007 | Communication service | Email (SendGrid), SMS (Twilio) |
| **WebApp** | 5008 | Frontend dashboard | React, real-time monitoring |

### Event Flow

```
Product Created ──► Order Created ──► Payment Processed ──► Notification Sent
     │                   │                   │                   │
     ▼                   ▼                   ▼                   ▼
  Kafka Event      Kafka Event        Kafka Event         Kafka Event
```

## ✨ Features

### 🛍️ E-Commerce Features
- **Product Management**: Full CRUD operations for products
- **Inventory Management**: Stock tracking and updates
- **Order Processing**: Complete order lifecycle management
- **Payment Integration**: Stripe payment processing
- **Customer Notifications**: Email and SMS notifications
- **Real-time Dashboard**: Live service monitoring

### 🏗️ Technical Features
- **Event-Driven Architecture**: Apache Kafka for service communication
- **CQRS Pattern**: Command Query Responsibility Segregation
- **Health Checks**: Service monitoring and diagnostics
- **API Documentation**: Swagger/OpenAPI integration
- **Structured Logging**: Serilog with multiple sinks
- **AutoMapper**: Object-to-object mapping
- **FluentValidation**: Input validation
- **CORS Support**: Cross-origin resource sharing

### 🚀 DevOps Features
- **Docker Support**: Containerized services
- **Kubernetes Manifests**: K8s deployment configurations
- **Azure DevOps Pipeline**: CI/CD automation
- **Azure Integration**: Cloud-native deployment
- **Health Monitoring**: Application Insights integration

## 🛠️ Technology Stack

### Backend
- **.NET 9.0**: Latest framework with performance improvements
- **ASP.NET Core**: Web API framework
- **Entity Framework Core**: ORM for database operations
- **MediatR**: CQRS implementation
- **AutoMapper**: Object mapping
- **FluentValidation**: Input validation
- **Serilog**: Structured logging

### Messaging
- **Apache Kafka**: Event streaming platform
- **Confluent.Kafka**: .NET Kafka client

### Database
- **SQL Server**: Primary database
- **Entity Framework Core**: Data access layer

### External Services
- **Stripe**: Payment processing
- **SendGrid**: Email delivery
- **Twilio**: SMS notifications

### Frontend
- **React**: JavaScript library for UI
- **Bootstrap 5**: CSS framework
- **Font Awesome**: Icon library

### DevOps
- **Docker**: Containerization
- **Kubernetes**: Container orchestration
- **Azure DevOps**: CI/CD pipeline
- **Azure Cloud**: Cloud platform

## 📁 Project Structure

```
ecommerce-dotnet-kafka/
├── 📁 src/
│   ├── 📁 Services/
│   │   ├── 📁 ProductService/          # Product catalog management
│   │   ├── 📁 OrderService/            # Order processing
│   │   ├── 📁 PaymentService/          # Payment processing
│   │   └── 📁 NotificationService/     # Communication service
│   ├── 📁 Shared/
│   │   ├── 📁 Common/                  # Shared models and utilities
│   │   ├── 📁 Events/                  # Event definitions
│   │   └── 📁 Messaging/               # Kafka messaging
│   └── 📁 WebApp/                      # React frontend dashboard
├── 📁 infrastructure/
│   └── 📁 k8s/                        # Kubernetes manifests
├── 📁 docs/                           # Documentation
├── 📁 scripts/                        # Deployment scripts
├── 📄 docker-compose.yml              # Local development
├── 📄 azure-pipelines.yml             # CI/CD pipeline
└── 📄 README.md                       # This file
```

## 🚀 Quick Start

### Prerequisites

- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [Docker Desktop](https://www.docker.com/products/docker-desktop)
- [Git](https://git-scm.com/)

### Local Development Setup

1. **Clone the repository**
   ```bash
   git clone https://github.com/gourav8jain/ecommerce-dotnet-kafka.git
   cd ecommerce-dotnet-kafka
   ```

2. **Start Kafka and SQL Server**
   ```bash
   docker-compose up -d
   ```

3. **Build the solution**
   ```bash
   dotnet build
   ```

4. **Run all services**
   ```bash
   # Terminal 1 - Product Service
   dotnet run --project src/Services/ProductService/ProductService.csproj
   
   # Terminal 2 - Payment Service
   dotnet run --project src/Services/PaymentService/PaymentService.csproj
   
   # Terminal 3 - Order Service
   dotnet run --project src/Services/OrderService/OrderService.csproj
   
   # Terminal 4 - Notification Service
   dotnet run --project src/Services/NotificationService/NotificationService.csproj
   
   # Terminal 5 - WebApp
   dotnet run --project src/WebApp/WebApp.csproj
   ```

5. **Access the dashboard**
   - Open http://localhost:5008 in your browser
   - Monitor all services in real-time

## 🔧 Development Setup

### Environment Configuration

Create environment-specific configuration files:

```json
// appsettings.Development.json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=ECommerce;Trusted_Connection=true;TrustServerCertificate=true;"
  },
  "Kafka": {
    "BootstrapServers": "localhost:9092"
  },
  "Stripe": {
    "SecretKey": "your_stripe_secret_key"
  },
  "SendGrid": {
    "ApiKey": "your_sendgrid_api_key"
  },
  "Twilio": {
    "AccountSid": "your_twilio_account_sid",
    "AuthToken": "your_twilio_auth_token"
  }
}
```

### Database Setup

The system uses SQL Server with Entity Framework Core. Databases are created automatically on first run.

### Kafka Topics

The following Kafka topics are used:

- `product-created`
- `product-updated`
- `product-stock-updated`
- `order-created`
- `order-status-updated`
- `order-cancelled`
- `payment-processed`
- `payment-refunded`
- `notification-sent`

## 📚 API Documentation

### Product Service API

**Base URL**: `http://localhost:5000`

| Endpoint | Method | Description |
|----------|--------|-------------|
| `/api/products` | GET | Get all products |
| `/api/products/{id}` | GET | Get product by ID |
| `/api/products` | POST | Create new product |
| `/api/products/{id}` | PUT | Update product |
| `/api/products/{id}` | DELETE | Delete product |
| `/health` | GET | Health check |

### Order Service API

**Base URL**: `https://localhost:5005`

| Endpoint | Method | Description |
|----------|--------|-------------|
| `/api/orders` | GET | Get all orders |
| `/api/orders/{id}` | GET | Get order by ID |
| `/api/orders` | POST | Create new order |
| `/api/orders/{id}/status` | PUT | Update order status |
| `/api/orders/{id}/cancel` | POST | Cancel order |
| `/health` | GET | Health check |

### Payment Service API

**Base URL**: `https://localhost:5003`

| Endpoint | Method | Description |
|----------|--------|-------------|
| `/api/payments` | GET | Get all payments |
| `/api/payments/{id}` | GET | Get payment by ID |
| `/api/payments` | POST | Create new payment |
| `/api/payments/{id}/process` | POST | Process payment |
| `/api/payments/{id}/refund` | POST | Refund payment |
| `/health` | GET | Health check |

### Notification Service API

**Base URL**: `https://localhost:5007`

| Endpoint | Method | Description |
|----------|--------|-------------|
| `/api/notifications` | GET | Get all notifications |
| `/api/notifications/{id}` | GET | Get notification by ID |
| `/api/notifications` | POST | Send notification |
| `/api/notifications/templates` | GET | Get notification templates |
| `/health` | GET | Health check |

## 🚀 Deployment

### Docker Deployment

1. **Build Docker images**
   ```bash
   docker build -t ecommerce-product-service src/Services/ProductService/
   docker build -t ecommerce-order-service src/Services/OrderService/
   docker build -t ecommerce-payment-service src/Services/PaymentService/
   docker build -t ecommerce-notification-service src/Services/NotificationService/
   docker build -t ecommerce-webapp src/WebApp/
   ```

2. **Run with Docker Compose**
   ```bash
   docker-compose up -d
   ```

### Kubernetes Deployment

1. **Apply Kubernetes manifests**
   ```bash
   kubectl apply -f infrastructure/k8s/
   ```

2. **Check deployment status**
   ```bash
   kubectl get pods
   kubectl get services
   ```

### Azure Deployment

1. **Set up Azure resources**
   ```bash
   ./scripts/deploy-azure.sh
   ```

2. **Deploy to Azure**
   ```bash
   az pipelines run --name "ECommerce-CI-CD"
   ```

## 📊 Monitoring

### Health Checks

All services expose health check endpoints:

- Product Service: `http://localhost:5000/health`
- Order Service: `https://localhost:5005/health`
- Payment Service: `https://localhost:5003/health`
- Notification Service: `https://localhost:5007/health`

### Application Insights

The system integrates with Azure Application Insights for:

- Performance monitoring
- Error tracking
- Usage analytics
- Custom telemetry

### Dashboard

Access the real-time dashboard at `http://localhost:5008` to monitor:

- Service health status
- API response times
- Error rates
- Event flow visualization

## 🧪 Testing

### Unit Tests

```bash
dotnet test src/Tests/UnitTests/
```

### Integration Tests

```bash
dotnet test src/Tests/IntegrationTests/
```

### End-to-End Tests

```bash
dotnet test src/Tests/E2ETests/
```

## 🤝 Contributing

We welcome contributions! Please follow these steps:

1. **Fork the repository**
2. **Create a feature branch**
   ```bash
   git checkout -b feature/your-feature-name
   ```
3. **Make your changes**
4. **Add tests for new functionality**
5. **Commit your changes**
   ```bash
   git commit -m "feat: add your feature description"
   ```
6. **Push to your branch**
   ```bash
   git push origin feature/your-feature-name
   ```
7. **Create a Pull Request**

### Development Guidelines

- Follow C# coding conventions
- Use meaningful commit messages
- Add unit tests for new features
- Update documentation as needed
- Ensure all tests pass before submitting PR

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🙏 Acknowledgments

- [.NET Team](https://dotnet.microsoft.com/) for the amazing framework
- [Apache Kafka](https://kafka.apache.org/) for event streaming
- [Stripe](https://stripe.com/) for payment processing
- [SendGrid](https://sendgrid.com/) for email delivery
- [Twilio](https://www.twilio.com/) for SMS notifications

## 📞 Support

If you have any questions or need help:

- Create an [Issue](https://github.com/gourav8jain/ecommerce-dotnet-kafka/issues)
- Check the [Documentation](docs/)
- Review the [Architecture Guide](docs/architecture.md)

---

**Built with ❤️ using .NET 9.0 and modern microservices architecture**

# E-Commerce System Architecture

## Overview

This document describes the architecture of the Event-Driven Mini E-Commerce System built with .NET 8.0, Apache Kafka, and Azure cloud services.

## Architecture Principles

### 1. Microservices Architecture
- **Loose Coupling**: Services are independent and can be developed, deployed, and scaled independently
- **Single Responsibility**: Each service has a specific business domain
- **Data Isolation**: Each service owns its data and exposes APIs for data access
- **Technology Diversity**: Services can use different technologies as needed

### 2. Event-Driven Architecture
- **Asynchronous Communication**: Services communicate through events
- **Loose Coupling**: Services don't need to know about each other directly
- **Scalability**: Events can be processed by multiple consumers
- **Reliability**: Events are persisted and can be replayed

### 3. CQRS Pattern
- **Command Query Responsibility Segregation**: Separate read and write models
- **Optimized Queries**: Read models optimized for specific queries
- **Scalability**: Read and write operations can be scaled independently

## System Components

### 1. Microservices

#### Product Service
- **Purpose**: Manages product catalog and inventory
- **Responsibilities**:
  - Product CRUD operations
  - Inventory management
  - Product search and filtering
  - Product reviews and ratings
- **Database**: SQL Server (Products database)
- **Events Published**:
  - `ProductCreatedEvent`
  - `ProductUpdatedEvent`
  - `ProductStockUpdatedEvent`
  - `ProductDeletedEvent`

#### Order Service
- **Purpose**: Handles order processing and management
- **Responsibilities**:
  - Order creation and management
  - Order status tracking
  - Order history
  - Order validation
- **Database**: SQL Server (Orders database)
- **Events Published**:
  - `OrderCreatedEvent`
  - `OrderStatusUpdatedEvent`
  - `OrderCancelledEvent`
- **Events Consumed**:
  - `ProductStockUpdatedEvent`

#### Payment Service
- **Purpose**: Processes payments and transactions
- **Responsibilities**:
  - Payment processing
  - Payment validation
  - Refund processing
  - Payment history
- **Database**: SQL Server (Payments database)
- **Events Published**:
  - `PaymentProcessedEvent`
  - `PaymentFailedEvent`
  - `PaymentRefundedEvent`
- **Events Consumed**:
  - `OrderCreatedEvent`

#### Notification Service
- **Purpose**: Sends notifications via email/SMS
- **Responsibilities**:
  - Email notifications
  - SMS notifications
  - Notification templates
  - Notification history
- **Database**: SQL Server (Notifications database)
- **Events Consumed**:
  - `OrderCreatedEvent`
  - `OrderStatusUpdatedEvent`
  - `PaymentProcessedEvent`
  - `PaymentFailedEvent`

#### API Gateway
- **Purpose**: Single entry point for all client requests
- **Responsibilities**:
  - Request routing
  - Authentication and authorization
  - Rate limiting
  - Request/response transformation
  - API documentation (Swagger)
- **Technology**: ASP.NET Core with Ocelot

### 2. Event Infrastructure

#### Apache Kafka
- **Purpose**: Message broker for event streaming
- **Topics**:
  - `product-events`: Product-related events
  - `order-events`: Order-related events
  - `payment-events`: Payment-related events
  - `notification-events`: Notification-related events
- **Configuration**:
  - Bootstrap servers: `localhost:9092` (local), `kafka-service:9092` (K8s)
  - Consumer group: `ecommerce-group`
  - Auto-commit: `false`
  - Auto-offset-reset: `earliest`

#### Event Structure
```json
{
  "eventId": "guid",
  "occurredOn": "2024-01-01T00:00:00Z",
  "eventType": "ProductCreatedEvent",
  "aggregateId": "product-id",
  "version": 1,
  "data": {
    // Event-specific data
  }
}
```

### 3. Data Storage

#### SQL Server
- **Product Database**: Stores product catalog and inventory
- **Order Database**: Stores orders and order history
- **Payment Database**: Stores payment transactions
- **Notification Database**: Stores notification history

#### Cosmos DB
- **Purpose**: NoSQL database for product catalog
- **Use Cases**: Product search, recommendations, analytics
- **Partition Key**: Category

#### Redis
- **Purpose**: Caching layer
- **Use Cases**: Session storage, API response caching, rate limiting

### 4. Azure Services

#### Azure Container Registry (ACR)
- **Purpose**: Store Docker images
- **Configuration**: Private registry with admin access

#### Azure Kubernetes Service (AKS)
- **Purpose**: Container orchestration
- **Configuration**:
  - Node count: 3
  - Monitoring enabled
  - ACR integration

#### Azure Key Vault
- **Purpose**: Secrets management
- **Stored Secrets**:
  - Database connection strings
  - API keys
  - JWT signing keys

#### Azure Application Insights
- **Purpose**: Application monitoring
- **Features**:
  - Performance monitoring
  - Error tracking
  - Custom metrics
  - Distributed tracing

#### Azure Service Bus
- **Purpose**: Alternative message broker
- **Topics**:
  - `product-events`
  - `order-events`
  - `payment-events`

## Communication Patterns

### 1. Synchronous Communication
- **HTTP/REST APIs**: Direct service-to-service communication
- **API Gateway**: Centralized routing and load balancing
- **Health Checks**: Service health monitoring

### 2. Asynchronous Communication
- **Event Publishing**: Services publish events to Kafka topics
- **Event Consumption**: Services consume events from Kafka topics
- **Event Sourcing**: Events stored for audit and replay

### 3. Data Consistency
- **Eventual Consistency**: Data consistency achieved through events
- **Saga Pattern**: Distributed transactions using events
- **Compensation**: Rollback mechanisms for failed operations

## Security

### 1. Authentication
- **JWT Tokens**: Stateless authentication
- **Azure AD**: Enterprise authentication
- **API Keys**: Service-to-service authentication

### 2. Authorization
- **Role-Based Access Control (RBAC)**: User permissions
- **API Gateway**: Centralized authorization
- **Service-to-Service**: Mutual TLS authentication

### 3. Data Protection
- **Encryption at Rest**: Database encryption
- **Encryption in Transit**: HTTPS/TLS
- **Key Management**: Azure Key Vault

## Monitoring and Observability

### 1. Application Monitoring
- **Application Insights**: Performance and error tracking
- **Custom Metrics**: Business metrics
- **Distributed Tracing**: Request flow tracking

### 2. Infrastructure Monitoring
- **Azure Monitor**: Infrastructure metrics
- **Kubernetes Dashboard**: Cluster monitoring
- **Kafka Manager**: Message broker monitoring

### 3. Logging
- **Structured Logging**: JSON format logs
- **Log Aggregation**: Centralized log storage
- **Log Levels**: Debug, Info, Warning, Error

## Deployment

### 1. Local Development
- **Docker Compose**: Local environment setup
- **Kafka**: Local message broker
- **SQL Server**: Local database
- **Redis**: Local cache

### 2. Azure Deployment
- **AKS**: Kubernetes cluster
- **ACR**: Container registry
- **Azure SQL**: Managed database
- **Azure Service Bus**: Managed message broker

### 3. CI/CD Pipeline
- **Azure DevOps**: Build and deployment automation
- **Multi-Stage Pipeline**: Build, test, deploy
- **Environment Promotion**: Dev → Staging → Production

## Scalability

### 1. Horizontal Scaling
- **Kubernetes HPA**: Automatic pod scaling
- **Load Balancing**: Traffic distribution
- **Database Scaling**: Read replicas

### 2. Performance Optimization
- **Caching**: Redis for frequently accessed data
- **CDN**: Static content delivery
- **Database Optimization**: Indexing and query optimization

### 3. High Availability
- **Multi-AZ Deployment**: Availability zone redundancy
- **Health Checks**: Automatic failover
- **Backup and Recovery**: Data protection

## Disaster Recovery

### 1. Backup Strategy
- **Database Backups**: Automated SQL Server backups
- **Event Store**: Kafka topic replication
- **Configuration**: Infrastructure as Code

### 2. Recovery Procedures
- **RTO**: 4 hours recovery time objective
- **RPO**: 1 hour recovery point objective
- **Testing**: Regular disaster recovery drills

## Cost Optimization

### 1. Resource Management
- **Auto-scaling**: Scale based on demand
- **Reserved Instances**: Cost savings for predictable workloads
- **Resource Monitoring**: Track and optimize usage

### 2. Azure Cost Management
- **Budget Alerts**: Cost monitoring
- **Resource Tagging**: Cost allocation
- **Optimization Recommendations**: Azure Advisor

## Future Enhancements

### 1. Advanced Features
- **Machine Learning**: Product recommendations
- **Real-time Analytics**: Business intelligence
- **Mobile App**: Native mobile application

### 2. Technology Upgrades
- **.NET 9**: Framework upgrades
- **Kafka 3.x**: Message broker upgrades
- **Kubernetes 1.30**: Container orchestration upgrades

### 3. Architecture Evolution
- **Service Mesh**: Istio for advanced traffic management
- **GraphQL**: Alternative to REST APIs
- **Event Streaming**: Advanced event processing 
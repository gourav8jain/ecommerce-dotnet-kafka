#!/bin/bash

# Azure E-Commerce System Deployment Script
# This script deploys the entire e-commerce system to Azure

set -e

# Configuration
RESOURCE_GROUP="ecommerce-rg"
LOCATION="East US"
AKS_CLUSTER_NAME="ecommerce-aks"
ACR_NAME="ecommerceacr"
KEY_VAULT_NAME="ecommerce-kv"
SQL_SERVER_NAME="ecommerce-sql"
APP_INSIGHTS_NAME="ecommerce-appinsights"
SERVICE_BUS_NAME="ecommerce-sb"
COSMOS_DB_NAME="ecommerce-cosmos"

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

echo -e "${GREEN}Starting Azure E-Commerce System Deployment...${NC}"

# Check if Azure CLI is installed
if ! command -v az &> /dev/null; then
    echo -e "${RED}Azure CLI is not installed. Please install it first.${NC}"
    exit 1
fi

# Login to Azure
echo -e "${YELLOW}Logging in to Azure...${NC}"
az login

# Create Resource Group
echo -e "${YELLOW}Creating Resource Group...${NC}"
az group create --name $RESOURCE_GROUP --location $LOCATION

# Create Azure Container Registry
echo -e "${YELLOW}Creating Azure Container Registry...${NC}"
az acr create --resource-group $RESOURCE_GROUP --name $ACR_NAME --sku Basic --admin-enabled true

# Get ACR login server
ACR_LOGIN_SERVER=$(az acr show --name $ACR_NAME --resource-group $RESOURCE_GROUP --query "loginServer" --output tsv)

# Create Azure Key Vault
echo -e "${YELLOW}Creating Azure Key Vault...${NC}"
az keyvault create --name $KEY_VAULT_NAME --resource-group $RESOURCE_GROUP --location $LOCATION

# Create SQL Server
echo -e "${YELLOW}Creating Azure SQL Server...${NC}"
az sql server create \
    --name $SQL_SERVER_NAME \
    --resource-group $RESOURCE_GROUP \
    --location $LOCATION \
    --admin-user "ecommerceadmin" \
    --admin-password "YourStrong@Passw0rd123!"

# Create SQL Databases
echo -e "${YELLOW}Creating SQL Databases...${NC}"
az sql db create --resource-group $RESOURCE_GROUP --server $SQL_SERVER_NAME --name "ECommerce_Products"
az sql db create --resource-group $RESOURCE_GROUP --server $SQL_SERVER_NAME --name "ECommerce_Orders"
az sql db create --resource-group $RESOURCE_GROUP --server $SQL_SERVER_NAME --name "ECommerce_Payments"

# Create Application Insights
echo -e "${YELLOW}Creating Application Insights...${NC}"
az monitor app-insights component create \
    --app $APP_INSIGHTS_NAME \
    --location $LOCATION \
    --resource-group $RESOURCE_GROUP \
    --application-type web

# Get Application Insights connection string
APP_INSIGHTS_CONNECTION_STRING=$(az monitor app-insights component show \
    --app $APP_INSIGHTS_NAME \
    --resource-group $RESOURCE_GROUP \
    --query "ConnectionString" --output tsv)

# Create Service Bus Namespace
echo -e "${YELLOW}Creating Service Bus Namespace...${NC}"
az servicebus namespace create \
    --resource-group $RESOURCE_GROUP \
    --name $SERVICE_BUS_NAME \
    --location $LOCATION \
    --sku Standard

# Create Service Bus Topics
echo -e "${YELLOW}Creating Service Bus Topics...${NC}"
az servicebus topic create --resource-group $RESOURCE_GROUP --namespace-name $SERVICE_BUS_NAME --name "product-events"
az servicebus topic create --resource-group $RESOURCE_GROUP --namespace-name $SERVICE_BUS_NAME --name "order-events"
az servicebus topic create --resource-group $RESOURCE_GROUP --namespace-name $SERVICE_BUS_NAME --name "payment-events"

# Create Cosmos DB Account
echo -e "${YELLOW}Creating Cosmos DB Account...${NC}"
az cosmosdb create \
    --name $COSMOS_DB_NAME \
    --resource-group $RESOURCE_GROUP \
    --locations regionName=$LOCATION

# Create Cosmos DB Database and Container
echo -e "${YELLOW}Creating Cosmos DB Database and Container...${NC}"
az cosmosdb sql database create \
    --account-name $COSMOS_DB_NAME \
    --resource-group $RESOURCE_GROUP \
    --name "ECommerceDB"

az cosmosdb sql container create \
    --account-name $COSMOS_DB_NAME \
    --resource-group $RESOURCE_GROUP \
    --database-name "ECommerceDB" \
    --name "Products" \
    --partition-key-path "/category"

# Create AKS Cluster
echo -e "${YELLOW}Creating AKS Cluster...${NC}"
az aks create \
    --resource-group $RESOURCE_GROUP \
    --name $AKS_CLUSTER_NAME \
    --node-count 3 \
    --enable-addons monitoring \
    --generate-ssh-keys \
    --attach-acr $ACR_NAME

# Get AKS credentials
echo -e "${YELLOW}Getting AKS credentials...${NC}"
az aks get-credentials --resource-group $RESOURCE_GROUP --name $AKS_CLUSTER_NAME

# Create Kubernetes namespace
echo -e "${YELLOW}Creating Kubernetes namespace...${NC}"
kubectl create namespace ecommerce

# Store secrets in Key Vault
echo -e "${YELLOW}Storing secrets in Key Vault...${NC}"
az keyvault secret set --vault-name $KEY_VAULT_NAME --name "sql-connection-string" \
    --value "Server=tcp:$SQL_SERVER_NAME.database.windows.net,1433;Initial Catalog=ECommerce_Products;Persist Security Info=False;User ID=ecommerceadmin;Password=YourStrong@Passw0rd123!;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"

az keyvault secret set --vault-name $KEY_VAULT_NAME --name "app-insights-connection-string" \
    --value "$APP_INSIGHTS_CONNECTION_STRING"

# Create Kubernetes secrets
echo -e "${YELLOW}Creating Kubernetes secrets...${NC}"
kubectl create secret generic sql-connection-secret \
    --from-literal=connection-string="Server=tcp:$SQL_SERVER_NAME.database.windows.net,1433;Initial Catalog=ECommerce_Products;Persist Security Info=False;User ID=ecommerceadmin;Password=YourStrong@Passw0rd123!;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;" \
    --namespace ecommerce

# Get ACR credentials for Kubernetes
ACR_USERNAME=$(az acr credential show --name $ACR_NAME --query "username" --output tsv)
ACR_PASSWORD=$(az acr credential show --name $ACR_NAME --query "passwords[0].value" --output tsv)

kubectl create secret docker-registry acr-secret \
    --docker-server=$ACR_LOGIN_SERVER \
    --docker-username=$ACR_USERNAME \
    --docker-password=$ACR_PASSWORD \
    --namespace ecommerce

# Build and push Docker images
echo -e "${YELLOW}Building and pushing Docker images...${NC}"

# Product Service
docker build -t $ACR_LOGIN_SERVER/ecommerce-product-service:latest -f src/Services/ProductService/Dockerfile .
docker push $ACR_LOGIN_SERVER/ecommerce-product-service:latest

# Order Service
docker build -t $ACR_LOGIN_SERVER/ecommerce-order-service:latest -f src/Services/OrderService/Dockerfile .
docker push $ACR_LOGIN_SERVER/ecommerce-order-service:latest

# Payment Service
docker build -t $ACR_LOGIN_SERVER/ecommerce-payment-service:latest -f src/Services/PaymentService/Dockerfile .
docker push $ACR_LOGIN_SERVER/ecommerce-payment-service:latest

# Notification Service
docker build -t $ACR_LOGIN_SERVER/ecommerce-notification-service:latest -f src/Services/NotificationService/Dockerfile .
docker push $ACR_LOGIN_SERVER/ecommerce-notification-service:latest

# API Gateway
docker build -t $ACR_LOGIN_SERVER/ecommerce-api-gateway:latest -f src/Services/ApiGateway/Dockerfile .
docker push $ACR_LOGIN_SERVER/ecommerce-api-gateway:latest

# Web App
docker build -t $ACR_LOGIN_SERVER/ecommerce-webapp:latest -f src/Web/WebApp/Dockerfile .
docker push $ACR_LOGIN_SERVER/ecommerce-webapp:latest

# Deploy to Kubernetes
echo -e "${YELLOW}Deploying to Kubernetes...${NC}"
kubectl apply -f infrastructure/k8s/ --namespace ecommerce

# Wait for deployments to be ready
echo -e "${YELLOW}Waiting for deployments to be ready...${NC}"
kubectl wait --for=condition=available --timeout=300s deployment/product-service -n ecommerce
kubectl wait --for=condition=available --timeout=300s deployment/order-service -n ecommerce
kubectl wait --for=condition=available --timeout=300s deployment/payment-service -n ecommerce
kubectl wait --for=condition=available --timeout=300s deployment/notification-service -n ecommerce
kubectl wait --for=condition=available --timeout=300s deployment/api-gateway -n ecommerce
kubectl wait --for=condition=available --timeout=300s deployment/webapp -n ecommerce

# Get service URLs
echo -e "${YELLOW}Getting service URLs...${NC}"
API_GATEWAY_IP=$(kubectl get service api-gateway -n ecommerce -o jsonpath='{.status.loadBalancer.ingress[0].ip}')

echo -e "${GREEN}Deployment completed successfully!${NC}"
echo -e "${GREEN}API Gateway URL: http://$API_GATEWAY_IP${NC}"
echo -e "${GREEN}Resource Group: $RESOURCE_GROUP${NC}"
echo -e "${GREEN}AKS Cluster: $AKS_CLUSTER_NAME${NC}"
echo -e "${GREEN}ACR: $ACR_NAME${NC}"
echo -e "${GREEN}Key Vault: $KEY_VAULT_NAME${NC}"

# Health check
echo -e "${YELLOW}Performing health checks...${NC}"
sleep 30
curl -f http://$API_GATEWAY_IP/health || echo -e "${RED}Health check failed${NC}"

echo -e "${GREEN}Azure E-Commerce System deployment completed!${NC}" 
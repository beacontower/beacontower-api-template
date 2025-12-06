# Kubernetes Manifests

Production-ready Kubernetes manifests for MyApi.

## Files

- **deployment.yaml** - Main deployment with 3 replicas, security context, health checks
- **service.yaml** - ClusterIP service exposing ports 80 (http) and 8081 (metrics)
- **ingress.yaml** - NGINX ingress with TLS/SSL (cert-manager)
- **configmap.yaml** - Application configuration (non-sensitive)
- **secret.yaml.template** - Secret template (DO NOT commit with actual values)
- **hpa.yaml** - Horizontal Pod Autoscaler (3-10 replicas based on CPU/memory)

## Quick Start

### Prerequisites

- Kubernetes cluster (v1.28+)
- kubectl configured
- Container registry credentials (Azure Container Registry)

### 1. Create Namespace (optional)

```bash
kubectl create namespace myapi-prod
kubectl config set-context --current --namespace=myapi-prod
```

### 2. Create Image Pull Secret

```bash
kubectl create secret docker-registry acr-secret \
  --docker-server=btcontainerregistry.azurecr.io \
  --docker-username=<ACR_USERNAME> \
  --docker-password=<ACR_PASSWORD> \
  --docker-email=<EMAIL>
```

### 3. Create Application Secrets

```bash
kubectl create secret generic myapi-secrets \
  --from-literal=ConnectionStrings__PostgreSQL='Host=postgres;Database=myapi;Username=myapi;Password=<password>' \
  --from-literal=ConnectionStrings__Redis='redis:6379,password=<password>'
```

### 4. Deploy Application

```bash
kubectl apply -f .
```

### 5. Verify Deployment

```bash
kubectl get deployments
kubectl get pods
kubectl get services
kubectl get ingress

# Check health
kubectl port-forward svc/myapi 8080:80
curl http://localhost:8080/health
```

## Configuration

### Environment Variables

Set in `configmap.yaml`:
- `ASPNETCORE_ENVIRONMENT` - Environment name
- `Serilog__MinimumLevel__Default` - Log level
- `OTEL_SERVICE_NAME` - Service name for observability

Set in `secret` (sensitive):
- `ConnectionStrings__PostgreSQL` - PostgreSQL connection string
- `ConnectionStrings__Redis` - Redis connection string

### Resource Limits

- Requests: 100m CPU, 128Mi memory
- Limits: 500m CPU, 512Mi memory
- Replicas: 3-10 (HPA)

## Health Checks

- **/health** - Overall health (includes dependencies)
- **/alive** - Liveness probe
- **/ready** - Readiness probe

## Ingress

Update the host in `ingress.yaml`:
```yaml
spec:
  tls:
  - hosts:
    - myapi.yourcompany.com
  rules:
  - host: myapi.yourcompany.com
```

## Updating Deployment

```bash
# Update image version
kubectl set image deployment/myapi myapi=btcontainerregistry.azurecr.io/myapi:1.2.3

# Watch rollout
kubectl rollout status deployment/myapi

# Rollback if needed
kubectl rollout undo deployment/myapi
```

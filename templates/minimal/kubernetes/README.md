# Kubernetes Manifests

Production-ready Kubernetes manifests for MyApi.

## Files

- **deployment.yaml** - Main deployment with 3 replicas, security context, health checks
- **service.yaml** - ClusterIP service exposing ports 80 (http) and 8081 (metrics)
- **ingress.yaml** - NGINX ingress with TLS/SSL (cert-manager)
- **configmap.yaml** - Application configuration (non-sensitive)
- **secret.yaml.template** - Secret template (DO NOT commit with actual values)
- **hpa.yaml** - Horizontal Pod Autoscaler (3-10 replicas based on CPU/memory)

## K3s Overrides

The `k3s/` folder contains integration environment overrides:
- Lower resource limits
- Staging environment
- Traefik ingress (instead of nginx)

## Quick Start

### Prerequisites

- Kubernetes cluster (v1.28+) or K3s
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

**Option A: From template (for testing)**
```bash
# Edit secret.yaml.template with actual values
cp secret.yaml.template secret.yaml
# IMPORTANT: Add secret.yaml to .gitignore
echo "kubernetes/secret.yaml" >> ../.gitignore

kubectl apply -f secret.yaml
```

**Option B: From literals (recommended)**
```bash
kubectl create secret generic myapi-secrets \
  --from-literal=ConnectionStrings__PostgreSQL='Host=postgres;Database=myapi;Username=myapi;Password=<password>' \
  --from-literal=ConnectionStrings__Redis='redis:6379,password=<password>'
```

### 4. Deploy Application

**Production (K8s):**
```bash
kubectl apply -f deployment.yaml
kubectl apply -f service.yaml
kubectl apply -f configmap.yaml
kubectl apply -f ingress.yaml
kubectl apply -f hpa.yaml
```

**Integration (K3s):**
```bash
kubectl apply -f deployment.yaml
kubectl apply -f service.yaml
kubectl apply -f configmap.yaml
kubectl apply -f k3s/deployment.yaml  # Overrides
kubectl apply -f k3s/ingress.yaml     # Traefik ingress
```

### 5. Verify Deployment

```bash
# Check deployment status
kubectl get deployments
kubectl get pods
kubectl get services
kubectl get ingress

# Check pod logs
kubectl logs -l app=myapi --tail=50 -f

# Check health
kubectl port-forward svc/myapi 8080:80
curl http://localhost:8080/health
```

## Configuration

### Environment Variables

Set in `configmap.yaml`:
- `ASPNETCORE_ENVIRONMENT` - Environment name (Production, Staging, Development)
- `Serilog__MinimumLevel__Default` - Log level
- `OTEL_SERVICE_NAME` - Service name for observability

Set in `secret` (sensitive):
- `ConnectionStrings__PostgreSQL` - PostgreSQL connection string
- `ConnectionStrings__Redis` - Redis connection string

### Resource Limits

**Production (deployment.yaml):**
- Requests: 100m CPU, 128Mi memory
- Limits: 500m CPU, 512Mi memory
- Replicas: 3-10 (HPA)

**Integration (k3s/deployment.yaml):**
- Requests: 50m CPU, 64Mi memory
- Limits: 250m CPU, 256Mi memory
- Replicas: 2

### Autoscaling

HPA scales based on:
- CPU utilization (target: 70%)
- Memory utilization (target: 80%)

## Health Checks

The deployment uses three health check endpoints:

- **/health** - Overall health (includes dependencies)
- **/alive** - Liveness probe (is the app responsive?)
- **/ready** - Readiness probe (is the app ready to serve traffic?)

## Security

Security features enabled:
- **Non-root user** (UID 1000)
- **Read-only root filesystem**
- **No privilege escalation**
- **Dropped capabilities**
- **Security context** with seccomp profile
- **Pod anti-affinity** for better distribution

## Ingress

### Production (NGINX)

- Ingress class: `nginx`
- TLS: cert-manager with Let's Encrypt
- Rate limiting: 100 requests total, 10 RPS
- Force SSL redirect

Update the host in `ingress.yaml`:
```yaml
spec:
  tls:
  - hosts:
    - myapi.yourcompany.com  # <- Change this
  rules:
  - host: myapi.yourcompany.com  # <- Change this
```

### K3s (Traefik)

- Ingress class: `traefik`
- Local hostname: `myapi.k3s.local`
- Add to `/etc/hosts`: `<K3S_IP> myapi.k3s.local`

## Troubleshooting

### Pod not starting

```bash
# Describe pod to see events
kubectl describe pod <pod-name>

# Check logs
kubectl logs <pod-name>

# Check previous container logs (if crashed)
kubectl logs <pod-name> --previous
```

### Image pull errors

```bash
# Verify image pull secret
kubectl get secret acr-secret -o yaml

# Verify image exists
docker pull btcontainerregistry.azurecr.io/myapi:latest
```

### Health check failures

```bash
# Port-forward and test health endpoint
kubectl port-forward <pod-name> 8080:8080
curl http://localhost:8080/health
curl http://localhost:8080/alive
curl http://localhost:8080/ready
```

### Connection issues

```bash
# Test service connectivity from another pod
kubectl run debug --rm -it --image=busybox -- sh
wget -qO- http://myapi/health
```

## Updating Deployment

### Rolling Update

```bash
# Update image version
kubectl set image deployment/myapi myapi=btcontainerregistry.azurecr.io/myapi:1.2.3

# Watch rollout
kubectl rollout status deployment/myapi

# Check rollout history
kubectl rollout history deployment/myapi
```

### Rollback

```bash
# Rollback to previous version
kubectl rollout undo deployment/myapi

# Rollback to specific revision
kubectl rollout undo deployment/myapi --to-revision=2
```

## Monitoring

### Metrics

Prometheus metrics available at `:8081/metrics`

Configure ServiceMonitor:
```yaml
apiVersion: monitoring.coreos.com/v1
kind: ServiceMonitor
metadata:
  name: myapi
spec:
  selector:
    matchLabels:
      app: myapi
  endpoints:
  - port: metrics
    interval: 30s
```

### Logs

View logs:
```bash
# All pods
kubectl logs -l app=myapi --tail=100 -f

# Specific pod
kubectl logs <pod-name> -f
```

## Cleanup

```bash
# Delete all resources
kubectl delete -f deployment.yaml
kubectl delete -f service.yaml
kubectl delete -f configmap.yaml
kubectl delete -f ingress.yaml
kubectl delete -f hpa.yaml
kubectl delete secret myapi-secrets
kubectl delete secret acr-secret
```

## Next Steps

- [ ] Update ingress hostname
- [ ] Configure TLS certificates (cert-manager)
- [ ] Set up monitoring (Prometheus/Grafana)
- [ ] Configure log aggregation (Loki/ELK)
- [ ] Set up tracing (Jaeger/Tempo)
- [ ] Configure backup strategy
- [ ] Document disaster recovery procedures

# PaytrackR Kubernetes Deployment

## Prerequisites
- Kubernetes cluster (AKS, GKE, EKS, or local with minikube/kind)
- kubectl configured
- Docker Hub account (for image registry)

## Quick Start

### 1. Deploy everything at once
```bash
# Deploy the application
kubectl apply -f k8s/namespace.yaml
kubectl apply -f k8s/secret.yaml
kubectl apply -f k8s/configmap.yaml
kubectl apply -f k8s/postgres-deployment.yaml
kubectl apply -f k8s/postgres-service.yaml
kubectl apply -f k8s/deployment.yaml
kubectl apply -f k8s/service.yaml

# Deploy monitoring (optional)
kubectl apply -f k8s/monitoring/
```

### 2. Or use a single command
```bash
kubectl apply -f k8s/ --recursive
```

## Configuration

### Update Secrets
Before deploying, update the secrets in `k8s/secret.yaml`:
```bash
# Edit the file and change passwords
# Then apply:
kubectl apply -f k8s/secret.yaml
```

### Scale the application
```bash
# Scale to 5 replicas
kubectl scale deployment/paytrackr --replicas=5 -n paytrackr
```

## Access the Application

### Get the service URL
```bash
# For LoadBalancer
kubectl get svc paytrackr-service -n paytrackr

# For Ingress (if configured)
kubectl get ingress -n paytrackr
```

### Access Grafana
```bash
kubectl get svc grafana-service -n paytrackr
# Default credentials: admin/admin (change in grafana-deployment.yaml)
```

### Access Prometheus
```bash
kubectl port-forward svc/prometheus-service 9090:9090 -n paytrackr
# Open http://localhost:9090
```

## Monitoring

Prometheus is configured to scrape metrics from:
- PaytrackR application pods (on /metrics endpoint)
- Kubernetes cluster metrics

Grafana comes pre-configured with Prometheus as a datasource.

## CI/CD Pipeline

The GitHub Actions workflow (`.github/workflows/ci-cd.yaml`) will:
1. Build and test the .NET application
2. Build and push Docker image to Docker Hub
3. Deploy to Kubernetes cluster

### Required GitHub Secrets
- `DOCKER_USERNAME` - Docker Hub username
- `DOCKER_PASSWORD` - Docker Hub password/token
- `KUBECONFIG` - Base64 encoded kubeconfig file

To encode kubeconfig:
```bash
cat ~/.kube/config | base64 -w 0
```

## Troubleshooting

### Check pod logs
```bash
kubectl logs -f deployment/paytrackr -n paytrackr
```

### Check pod status
```bash
kubectl get pods -n paytrackr
kubectl describe pod <pod-name> -n paytrackr
```

### Database connection issues
```bash
# Test PostgreSQL connection
kubectl exec -it deployment/paytrackr -n paytrackr -- /bin/bash
# Then inside the pod:
# psql -h postgres-service -U paytrackr_user -d paytrackr_db
```

### Delete everything
```bash
kubectl delete namespace paytrackr
```

## File Structure

```
k8s/
├── namespace.yaml              # Namespace definition
├── secret.yaml                 # Secrets (DB passwords, connection strings)
├── configmap.yaml              # Configuration
├── postgres-deployment.yaml    # PostgreSQL database
├── postgres-service.yaml       # PostgreSQL service
├── deployment.yaml             # PaytrackR app (3 replicas)
├── service.yaml                # LoadBalancer service
├── ingress.yaml                # Ingress (optional)
├── monitoring/
│   ├── prometheus-config.yaml
│   ├── prometheus-deployment.yaml
│   └── grafana-deployment.yaml
└── README.md
```

## Health Checks

The deployment includes:
- **Liveness Probe**: Checks if the app is running (restarts if fails)
- **Readiness Probe**: Checks if the app is ready to receive traffic

Make sure your app has a `/health` endpoint!

## Resource Limits

Default resource limits per pod:
- **PaytrackR**: 256Mi-512Mi RAM, 250m-500m CPU
- **PostgreSQL**: 256Mi-512Mi RAM, 250m-500m CPU
- **Prometheus**: 512Mi-1Gi RAM, 250m-500m CPU
- **Grafana**: 256Mi-512Mi RAM, 250m-500m CPU

Adjust in the respective deployment files based on your needs.

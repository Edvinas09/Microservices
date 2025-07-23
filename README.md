# Microservices Platform – Scalable .NET Microservices Example

A scalable, event-driven microservices application with **.NET 8**, **Kubernetes**, **RabbitMQ**, and **SQL Server**.  
This project demonstrates a microservices architecture. It is designed for learning, prototyping, and as a reference for real-world distributed systems.


---

## 🚀 Tech Stack

**Microservices**
- [.NET 8](https://dotnet.microsoft.com/) (Web API, gRPC)
- [Entity Framework Core](https://learn.microsoft.com/en-us/ef/core/) (data access)
- [RabbitMQ](https://www.rabbitmq.com/) (event/message broker)
- [SQL Server](https://www.microsoft.com/en-us/sql-server) (relational database)

**Infrastructure**
- [Docker](https://www.docker.com/) (containerization)
- [Kubernetes](https://kubernetes.io/) (orchestration)
- [kubectl](https://kubernetes.io/docs/tasks/tools/) (Kubernetes CLI)

---

## 🏗️ Project Overview
**Key Features:**
- Modular, independently deployable services
- RESTful APIs and gRPC for inter-service communication
- Asynchronous event-driven messaging with RabbitMQ
- Persistent storage with SQL Server (for PlatformService)
- Kubernetes manifests for easy deployment and scaling
- Local development with Docker and Kubernetes

---

## 📁 Monorepo Structure

```
PlatformService/      # .NET 8 Web API for platform management
CommandsService/      # .NET 8 Web API for command management
K8S/                  # Kubernetes manifests (deployments, services, ingress, pvc)
```

---

## 🛠️ Getting Started

### 1. **Clone the repository**
```bash
git clone https://github.com/yourusername/MicroservicesPlatform.git
cd MicroservicesPlatform
```

### 2. **Build Docker images**
```bash
docker build -t platformservice:latest ./PlatformService
docker build -t commandservice:latest ./CommandsService
```

### 3. **Run with Docker Compose (optional)**
If you want to run everything locally without Kubernetes, create a `docker-compose.yml` and use:
```bash
docker-compose up
```

### 4. **Deploy to Kubernetes (locally with Minikube or Docker Desktop)**
```bash
kubectl apply -f K8S/
```

### 5. **Access the services**
- Use `kubectl port-forward` or NodePort/LoadBalancer to access APIs:
  ```bash
  kubectl port-forward svc/platforms-clusterip-srv 8080:80
  # Then visit http://localhost:8080/api/platforms
  ```

---

## ⚙️ Configuration

- **appsettings.Production.json** in each service configures connection strings, RabbitMQ, and service endpoints.
- **Kubernetes YAMLs** define deployments, services, ingress, and persistent storage.

---

## 🧩 Service Details

### PlatformService
- Exposes REST API for platform management
- Publishes platform events to RabbitMQ
- Sends platform data to CommandsService via HTTP/gRPC
- Uses SQL Server for persistent storage

### CommandsService
- Exposes REST API for command management
- Subscribes to platform events from RabbitMQ
- Uses in-memory database (can be switched to persistent DB)
- Fetches platform data from PlatformService via gRPC

---

## 🗄️ Persistent Storage

- **SQL Server**: Used by PlatformService for data persistence.
- **Kubernetes PVC**: Used for SQL Server data volume.  
  Make sure your cluster supports the requested storage class and access mode.

---

## 🧪 Testing & Usage

- Use [Insomnia](https://insomnia.rest/) or [Postman](https://www.postman.com/) to test REST endpoints.
- Example endpoints:
  - `GET /api/platforms` (PlatformService)
  - `GET /api/c/platforms` (CommandsService)
- Use the RabbitMQ Management UI (default: [http://localhost:15672](http://localhost:15672), guest/guest) to inspect messages.

---

## 🛠️ Environment Variables

Each service uses an `appsettings.Production.json` file for configuration.  
Typical settings include:

```json
{
  "CommandService": "http://commands-clusterip-srv:80/api/c/platforms",
  "ConnectionStrings": {
    "PlatformsConn": "Server=mssql-clusterip-srv,1433;Initial Catalog=platformsdb;User ID=sa;Password=<yourpassword>!;Encrypt=False;TrustServerCertificate=True;"
  },
  "RabbitMQHost": "rabbitmq-clusterip-srv",
  "RabbitMQPort": "5672"
}
```

---

## 🐳 Kubernetes Manifests

- **Deployments**: Define how each service runs in the cluster.
- **Services**: Expose each deployment internally (ClusterIP) or externally (NodePort/LoadBalancer).
- **Ingress**: (Optional) Route external HTTP traffic to services.
- **PVCs**: Provide persistent storage for SQL Server.
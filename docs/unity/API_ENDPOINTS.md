# 📡 Ferritine API Endpoints
## Base URL
```
http://localhost:5000
```
## Available Endpoints
### 🌍 World State (Primary)
**GET** `/api/world/state`
Retorna o estado completo do mundo da simulação.
**Response Schema:**
```json
{
  "timestamp": "2025-11-24T18:30:00",
  "simulation_time": null,
  "agents": [AgentData[]],
  "vehicles": [VehicleData[]],
  "stations": [StationData[]],
  "routes": [RouteData[]],
  "operators": [OperatorData[]],
  "metrics": MetricsData
}
```
### 👥 Agents
**GET** `/api/agents`
Retorna lista de agentes (passageiros).
**Response:** `AgentData[]`
### 🚌 Vehicles
**GET** `/api/vehicles`
Retorna lista de veículos (ônibus, trens, metrô).
**Response:** `VehicleData[]`
### 🏢 Stations
**GET** `/api/stations`
Retorna lista de estações/paradas.
**Response:** `StationData[]`
### 🛤️ Routes
**GET** `/api/routes`
Retorna lista de rotas/linhas.
**Response:** `RouteData[]`
### 🏛️ Operators
**GET** `/api/operators`
Retorna lista de operadoras de transporte.
**Response:** `OperatorData[]`
### 📊 Metrics
**GET** `/api/metrics`
Retorna métricas agregadas do sistema.
**Response:** `MetricsData`
---
## 📋 Data Models
### AgentData
```csharp
{
  "id": "string",
  "name": "string",
  "status": "waiting|traveling|arrived",
  "location_type": "station|vehicle",
  "location_id": "string",
  "energy_level": 100,
  "wallet": 50.0
}
```
### VehicleData
```csharp
{
  "id": "string",
  "name": "string",
  "vehicle_type": "bus|metro_train|regional_train",
  "passengers": 45,
  "capacity": 80,
  "status": "active|maintenance|inactive",
  "current_station_id": "string",
  "current_route_id": "string",
  "fuel_level": 100.0
}
```
### StationData
```csharp
{
  "id": "string",
  "name": "string",
  "station_type": "bus_stop|metro|train",
  "x": 0,
  "y": 0,
  "queue_length": 15,
  "max_queue": 100,
  "is_operational": true
}
```
### RouteData
```csharp
{
  "id": "string",
  "name": "string",
  "code": "L1-RED",
  "route_type": "metro|bus|train",
  "fare": 4.40,
  "frequency": 5,
  "is_active": true
}
```
### OperatorData
```csharp
{
  "id": "string",
  "name": "string",
  "operator_type": "metro|bus|train",
  "revenue": 1000000.0,
  "costs": 800000.0,
  "profit": 200000.0
}
```
### MetricsData
```csharp
{
  "total_passengers_waiting": 65,
  "total_passengers_in_vehicles": 1450,
  "total_vehicles": 15,
  "total_stations": 10,
  "total_routes": 3,
  "total_revenue": 1500000.0,
  "avg_queue_length": 6.5
}
```
---
## 🧪 Testing Endpoints
### cURL Examples
```bash
# Get world state
curl http://localhost:5000/api/world/state
# Get only stations
curl http://localhost:5000/api/stations
# Get metrics
curl http://localhost:5000/api/metrics
```
### Browser Testing
Open: `http://localhost:5000/docs`
FastAPI provides interactive API documentation (Swagger UI).
---
## ⚙️ Configuration in Unity
Update `FerritineAPIClient.cs`:
```csharp
[Header("API Configuration")]
public string apiUrl = "http://localhost:5000";
public float pollInterval = 1f;
```
Change `apiUrl` if API is running on different host/port.
---
## 🔄 Polling Behavior
Unity client polls `/api/world/state` every `pollInterval` seconds:
- Default: 1 second
- Recommended for production: 2-5 seconds
- Adjust based on simulation speed and network latency
---
## 🚨 Error Handling
### Common HTTP Errors
| Status | Meaning | Solution |
|--------|---------|----------|
| 200 | Success | ✅ |
| 404 | Endpoint not found | Check API URL and endpoint path |
| 500 | Server error | Check API logs |
| 503 | Service unavailable | Check if API is running |
### Unity Client Behavior
- **Connection Error**: Logs error, invokes `OnError` event
- **JSON Parse Error**: Logs error, invokes `OnError` event
- **Success**: Invokes `OnWorldStateReceived` event
Check Unity Console for detailed error messages.
---
## 📝 Notes
- All timestamps are in ISO 8601 format
- Coordinates (x, y) are integers (grid-based)
- Currency values are floats
- All IDs are strings (UUIDs recommended)

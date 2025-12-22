# 🎮 Guia de Integração Unity/C#

**Versão**: 0.2.0  
**Data**: 2025-11-23  
**Status**: ✅ Backend pronto para integração

---

## 🎯 Visão Geral

Este guia mostra como conectar Unity ao backend Ferritine via API REST. O backend fornece dados em tempo real sobre:
- 🚇 Veículos (trens, ônibus)
- 🏢 Estações (filas, localização)
- 👤 Agentes (passageiros, funcionários)
- 📊 Métricas (receita, ocupação)

---

## 🚀 PASSO 1: Iniciar Backend

### 1.1 Instalar Dependências

```bash
# Ativar ambiente virtual
source venv/bin/activate  # Linux/Mac
# ou
venv\Scripts\activate  # Windows

# Instalar dependências
pip install fastapi uvicorn pydantic sqlalchemy psycopg2-binary python-dotenv
```

### 1.2 Configurar Banco de Dados

```bash
# Criar arquivo .env na raiz do projeto
cat > .env << EOF
DB_HOST=localhost
DB_PORT=5432
DB_NAME=ferritine
DB_USER=ferritine_user
DB_PASSWORD=ferritine_pass
EOF
```

### 1.3 Criar Banco e Popular

```bash
# Criar banco PostgreSQL
createdb ferritine

# Popular com dados iniciais
python main.py --seed
```

**Saída esperada:**
```
🌱 Iniciando seed de dados para Unity...
📍 Criando operadora...
   ✅ Operadora criada: Metrô de São Paulo
📍 Criando rota...
   ✅ Rota criada: Linha 1 - Azul
📍 Criando estações...
   ✅ 5 estações criadas
...
✅ SEED COMPLETO!
```

### 1.4 Iniciar API

```bash
# Rodar API (por padrão na porta 5000)
python main.py
```

**Saída esperada:**
```
🚀 Iniciando API Ferritine...
📡 API disponível em: http://localhost:5000
📚 Documentação em: http://localhost:5000/docs
INFO:     Uvicorn running on http://0.0.0.0:5000
```

### 1.5 Testar API

```bash
# Testar endpoint principal
curl http://localhost:5000/api/world/state

# Ou abra no navegador:
# http://localhost:5000/docs
```

---

## 🎮 PASSO 2: Integração Unity

### 2.1 Estrutura de Projeto Unity

Crie a seguinte estrutura em `Assets/`:

```
Assets/
├── Scripts/
│   ├── API/
│   │   ├── FerritineAPIClient.cs       # Cliente HTTP
│   │   ├── Models/
│   │   │   ├── WorldState.cs           # DTO do estado do mundo
│   │   │   ├── AgentData.cs
│   │   │   ├── VehicleData.cs
│   │   │   ├── StationData.cs
│   │   │   └── MetricsData.cs
│   ├── Controllers/
│   │   ├── WorldController.cs          # Controlador principal
│   │   ├── StationSpawner.cs           # Spawna estações
│   │   └── VehicleController.cs        # Controla veículos
│   └── UI/
│       └── MetricsUI.cs                # Dashboard de métricas
├── Prefabs/
│   ├── Station.prefab
│   └── Vehicle.prefab
└── Scenes/
    └── MainSimulation.unity
```

### 2.2 Criar Modelos de Dados (DTOs)

**`Assets/Scripts/API/Models/WorldState.cs`**:

```csharp
using System;
using System.Collections.Generic;

[Serializable]
public class WorldState
{
    public string timestamp;
    public string simulation_time;
    public List<AgentData> agents;
    public List<VehicleData> vehicles;
    public List<StationData> stations;
    public List<RouteData> routes;
    public List<OperatorData> operators;
    public MetricsData metrics;
}

[Serializable]
public class AgentData
{
    public string id;
    public string name;
    public string status;
    public string location_type;
    public string location_id;
    public int energy_level;
    public float wallet;
}

[Serializable]
public class VehicleData
{
    public string id;
    public string name;
    public string vehicle_type;
    public int passengers;
    public int capacity;
    public string status;
    public string current_station_id;
    public string current_route_id;
    public float fuel_level;
}

[Serializable]
public class StationData
{
    public string id;
    public string name;
    public string station_type;
    public int x;
    public int y;
    public int queue_length;
    public int max_queue;
    public bool is_operational;
}

[Serializable]
public class RouteData
{
    public string id;
    public string name;
    public string code;
    public string route_type;
    public float fare;
    public int frequency;
    public bool is_active;
}

[Serializable]
public class OperatorData
{
    public string id;
    public string name;
    public string operator_type;
    public float revenue;
    public float costs;
    public float profit;
}

[Serializable]
public class MetricsData
{
    public int total_passengers_waiting;
    public int total_passengers_in_vehicles;
    public int total_vehicles;
    public int total_stations;
    public int total_routes;
    public float total_revenue;
    public float avg_queue_length;
}
```

### 2.3 Cliente da API

**`Assets/Scripts/API/FerritineAPIClient.cs`**:

```csharp
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System;

public class FerritineAPIClient : MonoBehaviour
{
    [Header("API Configuration")]
    public string apiUrl = "http://localhost:5000";
    public float pollInterval = 1f; // Segundos entre requisições
    
    [Header("Events")]
    public Action<WorldState> OnWorldStateReceived;
    public Action<string> OnError;
    
    private bool isPolling = false;
    
    void Start()
    {
        StartPolling();
    }
    
    public void StartPolling()
    {
        if (!isPolling)
        {
            isPolling = true;
            StartCoroutine(PollWorldState());
        }
    }
    
    public void StopPolling()
    {
        isPolling = false;
        StopAllCoroutines();
    }
    
    IEnumerator PollWorldState()
    {
        while (isPolling)
        {
            yield return StartCoroutine(GetWorldState());
            yield return new WaitForSeconds(pollInterval);
        }
    }
    
    IEnumerator GetWorldState()
    {
        string url = $"{apiUrl}/api/world/state";
        
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();
            
            if (request.result == UnityWebRequest.Result.Success)
            {
                try
                {
                    string json = request.downloadHandler.text;
                    WorldState state = JsonUtility.FromJson<WorldState>(json);
                    OnWorldStateReceived?.Invoke(state);
                }
                catch (Exception e)
                {
                    Debug.LogError($"Erro ao parsear JSON: {e.Message}");
                    OnError?.Invoke(e.Message);
                }
            }
            else
            {
                Debug.LogError($"Erro na API: {request.error}");
                OnError?.Invoke(request.error);
            }
        }
    }
    
    // Método auxiliar para buscar apenas estações
    public IEnumerator GetStations(Action<StationData[]> callback)
    {
        string url = $"{apiUrl}/api/stations";
        
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();
            
            if (request.result == UnityWebRequest.Result.Success)
            {
                string json = request.downloadHandler.text;
                // Wrapper necessário porque Unity não deserializa arrays direto
                string wrappedJson = $"{{\"items\":{json}}}";
                StationDataWrapper wrapper = JsonUtility.FromJson<StationDataWrapper>(wrappedJson);
                callback?.Invoke(wrapper.items);
            }
        }
    }
    
    [Serializable]
    private class StationDataWrapper
    {
        public StationData[] items;
    }
}
```

### 2.4 Controlador Principal

**`Assets/Scripts/Controllers/WorldController.cs`**:

```csharp
using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class WorldController : MonoBehaviour
{
    [Header("References")]
    public FerritineAPIClient apiClient;
    
    [Header("Prefabs")]
    public GameObject stationPrefab;
    public GameObject vehiclePrefab;
    
    [Header("UI")]
    public TextMeshProUGUI debugText;
    
    // Dicionários para rastrear GameObjects
    private Dictionary<string, GameObject> stations = new Dictionary<string, GameObject>();
    private Dictionary<string, GameObject> vehicles = new Dictionary<string, GameObject>();
    
    void Start()
    {
        if (apiClient == null)
        {
            apiClient = GetComponent<FerritineAPIClient>();
        }
        
        // Inscrever no evento de atualização
        apiClient.OnWorldStateReceived += UpdateWorld;
        apiClient.OnError += HandleError;
    }
    
    void UpdateWorld(WorldState state)
    {
        // Atualizar estações
        UpdateStations(state.stations);
        
        // Atualizar veículos
        UpdateVehicles(state.vehicles);
        
        // Atualizar UI de debug
        if (debugText != null)
        {
            debugText.text = $"Tempo: {state.timestamp}\n" +
                           $"Estações: {state.stations.Count}\n" +
                           $"Veículos: {state.vehicles.Count}\n" +
                           $"Passageiros em fila: {state.metrics.total_passengers_waiting}\n" +
                           $"Passageiros em veículos: {state.metrics.total_passengers_in_vehicles}";
        }
    }
    
    void UpdateStations(List<StationData> stationData)
    {
        foreach (var data in stationData)
        {
            if (!stations.ContainsKey(data.id))
            {
                // Criar nova estação
                Vector3 position = new Vector3(data.x, 0, data.y);
                GameObject station = Instantiate(stationPrefab, position, Quaternion.identity);
                station.name = data.name;
                stations[data.id] = station;
                
                // Configurar texto
                TextMeshPro text = station.GetComponentInChildren<TextMeshPro>();
                if (text != null)
                {
                    text.text = data.name;
                }
            }
            
            // Atualizar estado (cor baseada em fila)
            GameObject stationObj = stations[data.id];
            Renderer renderer = stationObj.GetComponent<Renderer>();
            
            if (renderer != null)
            {
                // Verde se vazio, amarelo se médio, vermelho se cheio
                float queueRatio = (float)data.queue_length / data.max_queue;
                
                if (queueRatio < 0.3f)
                    renderer.material.color = Color.green;
                else if (queueRatio < 0.7f)
                    renderer.material.color = Color.yellow;
                else
                    renderer.material.color = Color.red;
            }
            
            // Atualizar texto de fila
            TextMeshPro queueText = stationObj.GetComponentInChildren<TextMeshPro>();
            if (queueText != null)
            {
                queueText.text = $"{data.name}\n🚶 {data.queue_length}/{data.max_queue}";
            }
        }
    }
    
    void UpdateVehicles(List<VehicleData> vehicleData)
    {
        foreach (var data in vehicleData)
        {
            if (!vehicles.ContainsKey(data.id))
            {
                // Criar novo veículo
                GameObject vehicle = Instantiate(vehiclePrefab, Vector3.zero, Quaternion.identity);
                vehicle.name = data.name;
                vehicles[data.id] = vehicle;
            }
            
            GameObject vehicleObj = vehicles[data.id];
            
            // Mover para estação atual (se tiver)
            if (!string.IsNullOrEmpty(data.current_station_id) && 
                stations.ContainsKey(data.current_station_id))
            {
                Vector3 targetPos = stations[data.current_station_id].transform.position;
                targetPos.y = 1f; // Elevado para ficar acima da estação
                
                // Movimento suave
                vehicleObj.transform.position = Vector3.Lerp(
                    vehicleObj.transform.position,
                    targetPos,
                    Time.deltaTime * 2f
                );
            }
            
            // Atualizar cor baseado em ocupação
            Renderer renderer = vehicleObj.GetComponent<Renderer>();
            if (renderer != null)
            {
                float occupancy = data.capacity > 0 ? (float)data.passengers / data.capacity : 0f;
                renderer.material.color = Color.Lerp(Color.blue, Color.magenta, occupancy);
            }
            
            // Atualizar texto
            TextMeshPro text = vehicleObj.GetComponentInChildren<TextMeshPro>();
            if (text != null)
            {
                text.text = $"{data.name}\n👥 {data.passengers}/{data.capacity}";
            }
        }
    }
    
    void HandleError(string error)
    {
        Debug.LogError($"API Error: {error}");
        
        if (debugText != null)
        {
            debugText.text = $"❌ Erro: {error}\nVerifique se a API está rodando!";
            debugText.color = Color.red;
        }
    }
}
```

---

## 📦 PASSO 3: Setup na Unity

### 3.1 Criar Cena

1. Criar nova cena: `MainSimulation`
2. Adicionar GameObject vazio: `WorldManager`
3. Anexar scripts:
   - `FerritineAPIClient.cs`
   - `WorldController.cs`

### 3.2 Criar Prefabs

**Station.prefab**:
- Cube (escala: 1, 0.5, 1)
- Cor: Verde
- TextMeshPro child (nome da estação)

**Vehicle.prefab**:
- Capsule (rotação: 90° no X)
- Cor: Azul
- TextMeshPro child (passageiros)

### 3.3 Configurar Câmera

```
Position: (0, 50, -30)
Rotation: (45, 0, 0)
Projection: Orthographic
Size: 30
```

---

## 🧪 PASSO 4: Testar

### 4.1 Checklist

- [ ] PostgreSQL rodando
- [ ] Banco populado (`python main.py --seed`)
- [ ] API rodando (`python main.py`)
- [ ] Unity conectando (Play mode)
- [ ] Estações aparecendo
- [ ] Veículos movendo
- [ ] Métricas atualizando

### 4.2 Teste de Conexão

No Console da Unity, você deve ver:

```
[FerritineAPIClient] Conectado à API
[WorldController] 5 estações criadas
[WorldController] 3 veículos criados
```

### 4.3 Troubleshooting

**Problema**: "Connection refused"
- ✅ API está rodando? (`python main.py`)
- ✅ Porta correta? (5000)

**Problema**: "JSON parse error"
- ✅ Modelos C# batem com Python?
- ✅ Teste endpoint no navegador

**Problema**: "Estações não aparecem"
- ✅ Prefabs configurados?
- ✅ Referências no WorldController?

---

## 📊 Endpoints Disponíveis

| Endpoint | Descrição | Retorno |
|----------|-----------|---------|
| `GET /` | Status da API | Informações básicas |
| `GET /health` | Health check | Status do banco |
| `GET /api/world/state` | **Estado completo** | Tudo (agentes, veículos, estações, métricas) |
| `GET /api/agents` | Lista de agentes | Array de AgentDTO |
| `GET /api/vehicles` | Lista de veículos | Array de VehicleDTO |
| `GET /api/stations` | Lista de estações | Array de StationDTO |
| `GET /api/routes` | Lista de rotas | Array de RouteDTO |
| `GET /api/operators` | Lista de operadoras | Array of OperatorDTO |
| `GET /api/metrics` | Métricas agregadas | MetricsDTO |

**Exemplo de Response** (`/api/world/state`):

```json
{
  "timestamp": "2025-11-23T18:30:00",
  "simulation_time": null,
  "agents": [...],
  "vehicles": [
    {
      "id": "abc-123",
      "name": "Trem 01",
      "vehicle_type": "metro_train",
      "passengers": 450,
      "capacity": 1200,
      "status": "active",
      "current_station_id": "def-456",
      "fuel_level": 100.0
    }
  ],
  "stations": [
    {
      "id": "def-456",
      "name": "Jabaquara",
      "station_type": "metro",
      "x": 0,
      "y": 0,
      "queue_length": 15,
      "max_queue": 100,
      "is_operational": true
    }
  ],
  "metrics": {
    "total_passengers_waiting": 65,
    "total_passengers_in_vehicles": 1450,
    "total_vehicles": 3,
    "total_stations": 5,
    "total_routes": 1,
    "total_revenue": 1000000.0,
    "avg_queue_length": 13.0
  }
}
```

---

## 🚀 Próximos Passos

### Fase 1 ✅ (Atual)
- ✅ API REST funcionando
- ✅ Unity consumindo dados
- ✅ Renderização básica (cubos)

### Fase 2 (Próximo)
- [ ] Movimento real de veículos (não só teleporte)
- [ ] Animação de filas
- [ ] Dashboard UI completo
- [ ] Controles de tempo (pausa, aceleração)

### Fase 3 (Futuro)
- [ ] WebSocket para tempo real
- [ ] Motor de simulação ativo (ticks)
- [ ] Economia dinâmica
- [ ] Eventos (acidentes, greves)

### Fase 4 (Futuro Distante)
- [ ] AR Foundation (overlay na maquete)
- [ ] Hardware Arduino (MQTT)
- [ ] Multiplayer

---

## 📚 Recursos

- **FastAPI Docs**: http://localhost:5000/docs (quando API estiver rodando)
- **Unity Networking**: https://docs.unity3d.com/Manual/UnityWebRequest.html
- **TextMeshPro**: Instalar via Package Manager

---

## ✅ Checklist Final

**Backend**:
- [ ] PostgreSQL configurado
- [ ] Banco criado (`createdb ferritine`)
- [ ] Dependências instaladas (`pip install -r requirements.txt`)
- [ ] Seed executado (`python main.py --seed`)
- [ ] API rodando (`python main.py`)
- [ ] Endpoint testado (`curl http://localhost:5000/api/world/state`)

**Unity**:
- [ ] Projeto criado (Unity 2022.3 LTS)
- [ ] Scripts criados (FerritineAPIClient, WorldController, etc)
- [ ] Prefabs criados (Station, Vehicle)
- [ ] Cena configurada
- [ ] TextMeshPro instalado
- [ ] Play mode funcionando

---

**🎉 PARABÉNS! Você está pronto para integrar Unity com Ferritine!**

Qualquer problema, consulte a seção de Troubleshooting ou abra uma issue no repositório.


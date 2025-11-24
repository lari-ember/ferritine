# Ferritine VU - Scripts

## 📁 Estrutura de Pastas

```
Assets/Scripts/
├── API/
│   ├── Models/          # Modelos de dados (DTOs) com suporte a UUID
│   └── FerritineAPIClient.cs
├── Controllers/
│   ├── WorldController.cs
│   ├── StationSpawner.cs
│   └── VehicleController.cs
└── UI/
    └── MetricsUI.cs
```

## 🔑 UUID Implementation

Todos os modelos de dados usam **UUIDs (Universally Unique Identifiers)** compatíveis com o backend Python/PostgreSQL.

### Documentação UUID

- **[UUID_QUICK_REFERENCE.md](../../UUID_QUICK_REFERENCE.md)** - Guia rápido de uso
- **[UUID_IMPLEMENTATION.md](../../UUID_IMPLEMENTATION.md)** - Guia completo de implementação
- **[MIGRATION_SUMMARY.md](../../MIGRATION_SUMMARY.md)** - Resumo da migração

### Exemplo de Uso

```csharp
// Receber dados da API
WorldState state = apiClient.GetWorldState();

// Acessar UUID como string
string vehicleId = state.vehicles[0].id;

// Converter para Guid quando necessário
Guid vehicleGuid = state.vehicles[0].GetIdAsGuid();

// Validar UUID
if (state.vehicles[0].HasValidId())
{
    ProcessVehicle(state.vehicles[0]);
}
```

## 📦 Modelos de Dados (API/Models)

Todos os modelos têm:
- Campos UUID como `string` (para compatibilidade com JSON)
- Método `GetIdAsGuid()` para conversão segura
- Método `HasValidId()` para validação

### Principais Modelos

| Modelo | Arquivo | Campos UUID |
|--------|---------|-------------|
| Agent | `AgentData.cs` | `id`, `location_id` |
| Vehicle | `VehicleData.cs` | `id`, `current_station_id`, `current_route_id` |
| Station | `StationData.cs` | `id` |
| Building | `BuildingData.cs` | `id`, `owner_id` |
| Route | `RouteData.cs` | `id` |
| Event | `EventData.cs` | `id`, `agent_id`, `building_id`, `vehicle_id` |
| Profession | `ProfessionData.cs` | `id` |
| Routine | `RoutineData.cs` | `id` |
| Operator | `OperatorData.cs` | `id` |

## 🎮 Controllers

### WorldController.cs
Controller principal que gerencia a visualização do mundo simulado.

**Responsabilidades**:
- Sincronizar estado do mundo com a API
- Criar/atualizar GameObjects de estações e veículos
- Gerenciar UI de debug
- Error handling

**Uso de UUIDs**: Usa string como chave em dicionários para performance.

```csharp
private Dictionary<string, GameObject> stations;  // UUID string como key
private Dictionary<string, GameObject> vehicles;  // UUID string como key
```

## 📡 API Client

### FerritineAPIClient.cs
Cliente HTTP para comunicação com o backend FastAPI.

**Features**:
- Polling automático (configurável)
- Eventos: `OnWorldStateReceived`, `OnError`
- Deserialização JSON automática
- Error handling robusto

**Configuração**:
```csharp
apiUrl = "http://localhost:5000"
pollInterval = 1.0f  // segundos
```

## 🔧 Desenvolvimento

### Adicionando Novos Modelos

1. Criar arquivo em `API/Models/`
2. Adicionar campos UUID como `string`
3. Implementar helper methods:

```csharp
using System;

[Serializable]
public class MyEntityData
{
    public string id;
    public string other_entity_id;
    
    // Helper methods
    public Guid GetIdAsGuid() => Guid.TryParse(id, out var r) ? r : Guid.Empty;
    public Guid GetOtherEntityIdAsGuid() => Guid.TryParse(other_entity_id, out var r) ? r : Guid.Empty;
    public bool HasValidId() => Guid.TryParse(id, out _);
}
```

4. Adicionar ao `WorldState.cs` se necessário

### Boas Práticas

✅ **DO**:
- Use `GetIdAsGuid()` apenas quando realmente precisar de um Guid
- Valide com `HasValidId()` antes de processar
- Use string para chaves de Dictionary
- Log UUIDs inválidos para debug

❌ **DON'T**:
- Não use `Guid.Parse()` diretamente (pode crashar)
- Não converta UUID para Guid múltiplas vezes no mesmo loop
- Não armazene UUIDs como Guid em classes serializáveis
- Não tente deserializar Guid direto do JSON com JsonUtility

## 🧪 Testes

Modelos têm métodos de validação para facilitar testes:

```csharp
[Test]
public void TestVehicleHasValidUuid()
{
    var vehicle = new VehicleData { id = Guid.NewGuid().ToString() };
    Assert.IsTrue(vehicle.HasValidId());
}

[Test]
public void TestInvalidUuidReturnsEmpty()
{
    var vehicle = new VehicleData { id = "invalid" };
    Assert.AreEqual(Guid.Empty, vehicle.GetIdAsGuid());
}
```

## 📚 Recursos

- **Backend Models**: `backend/database/models.py`
- **GDD**: `docs/gdd_ferritine.md`
- **API Docs**: Verificar endpoints em `http://localhost:5000/docs`

## 🆘 Troubleshooting

### "Guid should contain 32 digits"
O UUID recebido está malformado. Verifique a API backend.

### "NullReferenceException ao chamar GetIdAsGuid()"
O objeto não foi deserializado corretamente. Verifique o JSON da API.

### "JsonUtility não deserializa"
Use `string` nos modelos, não `Guid`. Os helper methods fazem a conversão.

---

Para mais detalhes, consulte a documentação completa em:
- [UUID_QUICK_REFERENCE.md](../../UUID_QUICK_REFERENCE.md)
- [UUID_IMPLEMENTATION.md](../../UUID_IMPLEMENTATION.md)


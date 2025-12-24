using UnityEngine;
using Utils;

/// <summary>
/// Script de diagnóstico para verificar por que entidades não aparecem.
/// Adicione a qualquer GameObject e execute no Play Mode.
/// Resultados aparecem no Console.
/// </summary>
public class EntitiesNotAppearingDiagnostic : MonoBehaviour
{
    [Header("Configuração")]
    [Tooltip("Executar diagnóstico automaticamente no Start?")]
    public bool runOnStart = true;
    
    [Tooltip("Tentar criar entidades de teste?")]
    public bool createTestEntities = false;

    void Start()
    {
        if (runOnStart)
        {
            Invoke(nameof(RunDiagnostic), 1f); // Aguardar 1 segundo para garantir que tudo inicializou
        }
    }

    [ContextMenu("Executar Diagnóstico")]
    public void RunDiagnostic()
    {
        Debug.Log("╔════════════════════════════════════════════════════════════╗");
        Debug.Log("║  DIAGNÓSTICO: Por que entidades não aparecem?              ║");
        Debug.Log("╚════════════════════════════════════════════════════════════╝");
        
        CheckWorldController();
        CheckObjectPool();
        CheckAPIClient();
        CheckPrefabs();
        CheckContainers();
        CheckExistingEntities();
        
        if (createTestEntities)
        {
            CreateTestStation();
        }
        
        Debug.Log("╔════════════════════════════════════════════════════════════╗");
        Debug.Log("║  FIM DO DIAGNÓSTICO                                        ║");
        Debug.Log("╚════════════════════════════════════════════════════════════╝");
    }

    void CheckWorldController()
    {
        Debug.Log("\n[1/6] 🔍 Verificando WorldController...");
        
        var wc = FindFirstObjectByType<Controllers.WorldController>();
        
        if (wc == null)
        {
            Debug.LogError("   ❌ WorldController NÃO ENCONTRADO!");
            Debug.LogError("   → Solução: Adicione WorldController a um GameObject na cena");
            return;
        }
        
        Debug.Log($"   ✅ WorldController encontrado: {wc.gameObject.name}");
        Debug.Log($"   📍 Posição: {wc.transform.position}");
        Debug.Log($"   🔌 Ativo: {wc.gameObject.activeInHierarchy}");
    }

    void CheckObjectPool()
    {
        Debug.Log("\n[2/6] 🔍 Verificando ObjectPool...");
        
        var pool = FindFirstObjectByType<ObjectPool>();
        
        if (pool == null)
        {
            Debug.LogError("   ❌ ObjectPool NÃO ENCONTRADO!");
            Debug.LogError("   → Solução: Criar GameObject e adicionar componente Utils.ObjectPool");
            return;
        }
        
        Debug.Log($"   ✅ ObjectPool encontrado: {pool.gameObject.name}");
        Debug.Log($"   🔌 Ativo: {pool.gameObject.activeInHierarchy}");
        
        // Tentar verificar pools inicializados
        Debug.Log("   ℹ️  Para verificar pools inicializados, veja as mensagens de [ObjectPool] no Console");
    }

    void CheckAPIClient()
    {
        Debug.Log("\n[3/6] 🔍 Verificando FerritineAPIClient...");
        
        var apiClient = FindFirstObjectByType<FerritineAPIClient>();
        
        if (apiClient == null)
        {
            Debug.LogWarning("   ⚠️  FerritineAPIClient NÃO ENCONTRADO!");
            Debug.LogWarning("   → Sem API, os dados não serão recebidos");
            Debug.LogWarning("   → Você pode criar entidades de teste manualmente");
            return;
        }
        
        Debug.Log($"   ✅ FerritineAPIClient encontrado: {apiClient.gameObject.name}");
        Debug.Log($"   🔌 Ativo: {apiClient.gameObject.activeInHierarchy}");
    }

    void CheckPrefabs()
    {
        Debug.Log("\n[4/6] 🔍 Verificando Prefabs no WorldController...");
        
        var wc = FindFirstObjectByType<Controllers.WorldController>();
        if (wc == null)
        {
            Debug.LogError("   ❌ WorldController não encontrado - impossível verificar prefabs");
            return;
        }

        // Usar reflection para acessar os campos privados/públicos
        var wcType = wc.GetType();
        
        var stationPrefabField = wcType.GetField("stationPrefab");
        var vehiclePrefabField = wcType.GetField("vehiclePrefab");
        var agentPrefabField = wcType.GetField("agentPrefab");
        
        var stationPrefab = stationPrefabField?.GetValue(wc) as GameObject;
        var vehiclePrefab = vehiclePrefabField?.GetValue(wc) as GameObject;
        var agentPrefab = agentPrefabField?.GetValue(wc) as GameObject;
        
        bool allGood = true;
        
        if (stationPrefab == null)
        {
            Debug.LogError("   ❌ stationPrefab NÃO ATRIBUÍDO!");
            Debug.LogError("   → Solução: No Inspector do WorldController, arraste o prefab de estação");
            allGood = false;
        }
        else
        {
            Debug.Log($"   ✅ stationPrefab: {stationPrefab.name}");
        }
        
        if (vehiclePrefab == null)
        {
            Debug.LogError("   ❌ vehiclePrefab NÃO ATRIBUÍDO!");
            Debug.LogError("   → Solução: No Inspector do WorldController, arraste o prefab de veículo");
            allGood = false;
        }
        else
        {
            Debug.Log($"   ✅ vehiclePrefab: {vehiclePrefab.name}");
        }
        
        if (agentPrefab == null)
        {
            Debug.LogError("   ❌ agentPrefab NÃO ATRIBUÍDO!");
            Debug.LogError("   → Solução: No Inspector do WorldController, arraste o prefab de agente");
            allGood = false;
        }
        else
        {
            Debug.Log($"   ✅ agentPrefab: {agentPrefab.name}");
        }
        
        if (allGood)
        {
            Debug.Log("   🎉 Todos os prefabs estão atribuídos!");
        }
    }

    void CheckContainers()
    {
        Debug.Log("\n[5/6] 🔍 Verificando Containers na hierarquia...");
        
        var stationsContainer = GameObject.Find("StationsContainer");
        var vehiclesContainer = GameObject.Find("VehiclesContainer");
        var agentsContainer = GameObject.Find("AgentsContainer");
        
        if (stationsContainer != null)
        {
            int childCount = stationsContainer.transform.childCount;
            Debug.Log($"   ✅ StationsContainer encontrado: {childCount} filhos");
            
            if (childCount == 0)
            {
                Debug.LogWarning("   ⚠️  Container vazio - nenhuma estação foi criada ainda");
            }
        }
        else
        {
            Debug.LogWarning("   ⚠️  StationsContainer não encontrado (será criado no Start do WorldController)");
        }
        
        if (vehiclesContainer != null)
        {
            int childCount = vehiclesContainer.transform.childCount;
            Debug.Log($"   ✅ VehiclesContainer encontrado: {childCount} filhos");
            
            if (childCount == 0)
            {
                Debug.LogWarning("   ⚠️  Container vazio - nenhum veículo foi criado ainda");
            }
        }
        else
        {
            Debug.LogWarning("   ⚠️  VehiclesContainer não encontrado (será criado no Start do WorldController)");
        }
        
        if (agentsContainer != null)
        {
            int childCount = agentsContainer.transform.childCount;
            Debug.Log($"   ✅ AgentsContainer encontrado: {childCount} filhos");
            
            if (childCount == 0)
            {
                Debug.LogWarning("   ⚠️  Container vazio - nenhum agente foi criado ainda");
            }
        }
        else
        {
            Debug.LogWarning("   ⚠️  AgentsContainer não encontrado (será criado no Start do WorldController)");
        }
    }

    void CheckExistingEntities()
    {
        Debug.Log("\n[6/6] 🔍 Verificando entidades existentes na cena...");
        
        var selectableEntities = FindObjectsByType<SelectableEntity>(FindObjectsSortMode.None);
        
        Debug.Log($"   📊 Total de SelectableEntity na cena: {selectableEntities.Length}");
        
        if (selectableEntities.Length == 0)
        {
            Debug.LogWarning("   ⚠️  Nenhuma entidade encontrada!");
            Debug.LogWarning("   → Isso é normal se a API ainda não enviou dados");
            Debug.LogWarning("   → Verifique se o backend está rodando e enviando dados");
        }
        else
        {
            int stations = 0, vehicles = 0, agents = 0, buildings = 0;
            
            foreach (var entity in selectableEntities)
            {
                switch (entity.entityType)
                {
                    case SelectableEntity.EntityType.Station: stations++; break;
                    case SelectableEntity.EntityType.Vehicle: vehicles++; break;
                    case SelectableEntity.EntityType.Agent: agents++; break;
                    case SelectableEntity.EntityType.Building: buildings++; break;
                }
            }
            
            Debug.Log($"   📈 Estações: {stations}");
            Debug.Log($"   📈 Veículos: {vehicles}");
            Debug.Log($"   📈 Agentes: {agents}");
            Debug.Log($"   📈 Buildings: {buildings}");
            
            // Mostrar amostra
            Debug.Log("\n   📋 Amostra de entidades (máx 5):");
            int sampleCount = Mathf.Min(5, selectableEntities.Length);
            for (int i = 0; i < sampleCount; i++)
            {
                var e = selectableEntities[i];
                bool isActive = e.gameObject.activeInHierarchy;
                bool hasRenderer = e.GetComponent<Renderer>() != null;
                
                Debug.Log($"   - {e.gameObject.name} | Tipo: {e.entityType} | Ativo: {isActive} | Renderer: {hasRenderer} | Pos: {e.transform.position}");
            }
        }
    }

    void CreateTestStation()
    {
        Debug.Log("\n🧪 Criando estação de teste...");
        
        var pool = FindFirstObjectByType<ObjectPool>();
        if (pool == null)
        {
            Debug.LogError("   ❌ ObjectPool não encontrado - impossível criar teste");
            return;
        }
        
        try
        {
            // Tentar obter do pool de estações
            var testStation = pool.Get("stations");
            
            if (testStation == null)
            {
                Debug.LogError("   ❌ Falha ao obter estação do pool");
                Debug.LogError("   → Pool 'stations' pode não estar inicializado");
                return;
            }
            
            testStation.name = "TEST_STATION";
            testStation.transform.position = new Vector3(0, 0, 0);
            testStation.SetActive(true);
            
            Debug.Log($"   ✅ Estação de teste criada: {testStation.name}");
            Debug.Log($"   📍 Posição: {testStation.transform.position}");
            Debug.Log($"   🔌 Ativa: {testStation.activeInHierarchy}");
            Debug.Log($"   👁️  Visível: {testStation.GetComponent<Renderer>() != null}");
            
            Debug.Log("\n   💡 Pressione 'F' com o objeto selecionado na hierarquia para focar a câmera nele!");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"   ❌ Erro ao criar estação de teste: {ex.Message}");
        }
    }
}


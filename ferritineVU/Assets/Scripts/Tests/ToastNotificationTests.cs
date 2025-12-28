using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Script de testes para validar o sistema de Toast Notifications.
/// Adicione este script a um GameObject vazio na cena e use os botões para testar.
/// </summary>
public class ToastNotificationTests : MonoBehaviour
{
    [Header("Test Panel")]
    [SerializeField] private GameObject testPanel;
    
    void Start()
    {
        // Criar painel de teste automaticamente
        if (testPanel == null)
        {
            CreateTestPanel();
        }
    }
    
    void CreateTestPanel()
    {
        // Criar canvas para testes
        GameObject canvasObj = new GameObject("TestToastPanel");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        
        RectTransform canvasRect = canvasObj.GetComponent<RectTransform>();
        canvasRect.offsetMin = Vector2.zero;
        canvasRect.offsetMax = Vector2.zero;
        
        // Criar scroll view com botões
        GridLayoutGroup grid = canvasObj.AddComponent<GridLayoutGroup>();
        grid.cellSize = new Vector2(200, 50);
        grid.spacing = new Vector2(10, 10);
        
        canvasObj.AddComponent<GraphicRaycaster>();
        
        // Adicionar botões de teste
        string[] testNames = {
            "Teleport Start",
            "Teleport Success",
            "Teleport Failed",
            "Backend Online",
            "Backend Offline",
            "Backend Error",
            "Invalid Action",
            "Warning Action",
            "Operation Success",
            "Operation Failed",
            "Clear All"
        };
        
        System.Action[] testActions = {
            TestTeleportStart,
            TestTeleportSuccess,
            TestTeleportFailed,
            TestBackendOnline,
            TestBackendOffline,
            TestBackendError,
            TestInvalidAction,
            TestWarningAction,
            TestOperationSuccess,
            TestOperationFailed,
            TestClearAll
        };
        
        for (int i = 0; i < testNames.Length; i++)
        {
            CreateTestButton(canvasObj.transform, testNames[i], testActions[i]);
        }
    }
    
    void CreateTestButton(Transform parent, string name, System.Action action)
    {
        GameObject btnObj = new GameObject(name);
        btnObj.transform.SetParent(parent, false);
        
        Image img = btnObj.AddComponent<Image>();
        img.color = new Color(0.2f, 0.2f, 0.2f, 0.8f);
        
        Button btn = btnObj.AddComponent<Button>();
        btn.targetGraphic = img;
        btn.onClick.AddListener(() => action());
        
        ColorBlock colors = btn.colors;
        colors.normalColor = new Color(0.3f, 0.3f, 0.3f, 0.8f);
        colors.highlightedColor = new Color(0.5f, 0.5f, 0.5f, 0.9f);
        colors.pressedColor = new Color(0.2f, 0.2f, 0.2f, 1f);
        btn.colors = colors;
        
        GameObject txtObj = new GameObject("Text");
        txtObj.transform.SetParent(btnObj.transform, false);
        
        TextMesh textMesh = txtObj.AddComponent<TextMesh>();
        textMesh.text = name;
        textMesh.fontSize = 20;
        textMesh.alignment = TextAlignment.Center;
    }
    
    // ==================== TESTES DE TELEPORTE ====================
    
    void TestTeleportStart()
    {
        Debug.Log("[TEST] Teleport Start");
        GameEventManager.RaiseTeleportStarted("Agent-001");
    }
    
    void TestTeleportSuccess()
    {
        Debug.Log("[TEST] Teleport Success");
        GameEventManager.RaiseTeleportSuccess("Agent-001", "Station Central");
    }
    
    void TestTeleportFailed()
    {
        Debug.Log("[TEST] Teleport Failed");
        GameEventManager.RaiseTeleportFailed("Agent-001", "Destino bloqueado");
    }
    
    // ==================== TESTES DE BACKEND ====================
    
    void TestBackendOnline()
    {
        Debug.Log("[TEST] Backend Online");
        GameEventManager.RaiseBackendOnline();
    }
    
    void TestBackendOffline()
    {
        Debug.Log("[TEST] Backend Offline");
        GameEventManager.RaiseBackendOffline();
    }
    
    void TestBackendError()
    {
        Debug.Log("[TEST] Backend Error");
        GameEventManager.RaiseBackendError(500, "Internal Server Error");
    }
    
    // ==================== TESTES DE VALIDAÇÃO ====================
    
    void TestInvalidAction()
    {
        Debug.Log("[TEST] Invalid Action");
        GameEventManager.RaiseInvalidAction("Apenas agentes podem teleportar");
    }
    
    void TestWarningAction()
    {
        Debug.Log("[TEST] Warning Action");
        GameEventManager.RaiseWarningAction("Operação que pode causar problemas");
    }
    
    // ==================== TESTES DE OPERAÇÃO ====================
    
    void TestOperationSuccess()
    {
        Debug.Log("[TEST] Operation Success");
        GameEventManager.RaiseOperationSuccess("Fila da estação modificada com sucesso");
    }
    
    void TestOperationFailed()
    {
        Debug.Log("[TEST] Operation Failed");
        GameEventManager.RaiseOperationFailed("Falha ao atualizar fila da estação");
    }
    
    // ==================== TESTE ESPECIAL ====================
    
    void TestClearAll()
    {
        Debug.Log("[TEST] Clear All Toasts");
        // Nota: Não há método para limpar todos, mas você pode esperar que desapareçam
        ToastNotificationManager.ShowInfo("Limpando notificações...");
    }
}

/*
 * ========================================
 * COMO USAR ESTE SCRIPT DE TESTES
 * ========================================
 * 
 * 1. Crie um GameObject vazio na cena
 * 2. Adicione este script ao GameObject
 * 3. Pressione Play
 * 4. Um painel com botões aparecerá no canto da tela
 * 5. Clique nos botões para testar cada evento
 * 
 * ========================================
 * O QUE CADA BOTÃO TESTA
 * ========================================
 * 
 * TELEPORT:
 * - Teleport Start: Simula início de teleporte (toast azul)
 * - Teleport Success: Simula sucesso (toast verde)
 * - Teleport Failed: Simula falha (toast vermelho)
 * 
 * BACKEND:
 * - Backend Online: Simula conexão restaurada (toast verde)
 * - Backend Offline: Simula desconexão (toast laranja)
 * - Backend Error: Simula erro HTTP 500 (toast vermelho)
 * 
 * VALIDAÇÃO:
 * - Invalid Action: Simula ação inválida (toast vermelho)
 * - Warning Action: Simula aviso (toast laranja)
 * 
 * OPERAÇÃO:
 * - Operation Success: Simula sucesso genérico (toast verde)
 * - Operation Failed: Simula falha genérica (toast vermelho)
 * 
 * ESPECIAL:
 * - Clear All: Mostra mensagem de limpeza
 * 
 * ========================================
 * VERIFICAÇÃO VISUAL
 * ========================================
 * 
 * ✓ Toast deve aparecer no topo da tela
 * ✓ Cor deve corresponder ao tipo
 * ✓ Mensagem deve ser legível
 * ✓ Toast deve desaparecer após ~3 segundos
 * ✓ Múltiplos toasts devem se empilhar
 * ✓ Som deve tocar (se AudioManager configurado)
 * 
 * ========================================
 * DICAS DE DEBUG
 * ========================================
 * 
 * 1. Abra Console (Ctrl+Shift+C)
 * 2. Procure por logs "[GameEventManager]"
 * 3. Cada evento aparecerá no Console
 * 4. Se toast não aparecer, check:
 *    - Prefab configurado no UIManager?
 *    - GameEventManager na cena?
 *    - ToastNotificationManager inicializado?
 */


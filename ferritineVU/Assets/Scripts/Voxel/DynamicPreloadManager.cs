using UnityEngine;
using Voxel;

/// <summary>
/// Exemplo de como trocar PreloadProfiles dinamicamente baseado no contexto do jogo.
/// Anexe este script ao mesmo GameObject que tem o VoxelWorld.
/// </summary>
[RequireComponent(typeof(VoxelWorld))]
public class DynamicPreloadManager : MonoBehaviour {
    
    [Header("Profiles Pré-configurados")]
    [Tooltip("Profile para exploração em mundo aberto (área grande, menos detalhe)")]
    public PreloadProfile explorationProfile;
    
    [Tooltip("Profile para modo construção (área média, detalhamento moderado)")]
    public PreloadProfile buildingProfile;
    
    [Tooltip("Profile para interiores (área pequena, alta definição)")]
    public PreloadProfile indoorProfile;
    
    [Header("Transição")]
    [Tooltip("Força recarregamento de chunks ao trocar profile")]
    public bool forceReloadOnSwitch = false;
    
    private VoxelWorld _voxelWorld;
    private PreloadProfile _currentProfile;
    
    void Awake() {
        _voxelWorld = GetComponent<VoxelWorld>();
        _currentProfile = _voxelWorld.preloadProfile;
    }
    
    /// <summary>
    /// Troca para o profile de exploração (mundo aberto).
    /// </summary>
    [ContextMenu("Switch to Exploration Profile")]
    public void SwitchToExploration() {
        SwitchProfile(explorationProfile, "Exploration");
    }
    
    /// <summary>
    /// Troca para o profile de construção.
    /// </summary>
    [ContextMenu("Switch to Building Profile")]
    public void SwitchToBuilding() {
        SwitchProfile(buildingProfile, "Building");
    }
    
    /// <summary>
    /// Troca para o profile de interiores (alta definição).
    /// </summary>
    [ContextMenu("Switch to Indoor Profile")]
    public void SwitchToIndoor() {
        SwitchProfile(indoorProfile, "Indoor");
    }
    
    /// <summary>
    /// Troca o profile ativo do VoxelWorld.
    /// </summary>
    /// <param name="newProfile">Novo profile a ser aplicado</param>
    /// <param name="profileName">Nome para debug</param>
    private void SwitchProfile(PreloadProfile newProfile, string profileName) {
        if (newProfile == null) {
            Debug.LogWarning($"DynamicPreloadManager: {profileName} Profile não atribuído!");
            return;
        }
        
        if (_currentProfile == newProfile) {
            Debug.Log($"DynamicPreloadManager: Já está usando {profileName} Profile");
            return;
        }
        
        _currentProfile = newProfile;
        _voxelWorld.preloadProfile = newProfile;
        
        Debug.Log($"[DynamicPreloadManager] Trocado para {profileName} Profile\n" +
                  $"{newProfile.GetDebugInfo(_voxelWorld.terrain.escalaVoxel)}");
        
        if (forceReloadOnSwitch) {
            // Força o VoxelWorld a recalcular chunks
            // (implementação específica depende da sua arquitetura)
            Debug.Log("Forçando recarregamento de chunks...");
        }
    }
    
    /// <summary>
    /// Exemplo de troca dinâmica baseado em gameplay.
    /// Chame este método quando o jogador entrar/sair de edifícios.
    /// </summary>
    /// <param name="isIndoor">True se está em interior</param>
    public void OnPlayerLocationChanged(bool isIndoor) {
        if (isIndoor) {
            SwitchToIndoor();
        } else {
            SwitchToExploration();
        }
    }
    
    /// <summary>
    /// Exemplo de troca baseado em modo de jogo.
    /// </summary>
    /// <param name="gameMode">Modo atual (0=Exploração, 1=Construção, 2=Indoor)</param>
    public void OnGameModeChanged(int gameMode) {
        switch (gameMode) {
            case 0: SwitchToExploration(); break;
            case 1: SwitchToBuilding(); break;
            case 2: SwitchToIndoor(); break;
            default: Debug.LogWarning($"Modo de jogo desconhecido: {gameMode}"); break;
        }
    }
    
    /// <summary>
    /// Retorna estatísticas do profile atual para UI/Debug.
    /// </summary>
    public string GetCurrentStats() {
        if (_currentProfile == null) return "Nenhum profile ativo";
        return _currentProfile.GetDebugInfo(_voxelWorld.terrain.escalaVoxel);
    }
}


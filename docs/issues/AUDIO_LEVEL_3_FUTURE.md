# 🔮 Sistema de Áudio - NÍVEL 3 (Bem Depois)

> **⚠️ ATENÇÃO**: Estas funcionalidades são **AVANÇADAS** e **NÃO devem ser implementadas agora**. Este documento serve como planejamento de longo prazo para features de áudio de alta complexidade.

## Pré-requisitos

Antes de considerar o Nível 3, é necessário que o **Nível 2** esteja 100% completo e testado:
- ✅ AudioMixer completo e configurável
- ✅ UI de controle de volume (mute/sliders)
- ✅ Sistema de som ambiente por zona

## 🎯 Objetivo do Nível 3

Implementar features avançadas de áudio que proporcionam realismo extremo, otimização de performance e experiência imersiva de alta qualidade. Estas features são típicas de jogos AAA e requerem conhecimento profundo de áudio e otimização.

---

## Issue 1: Sistema de Áudio 3D Real e Espacialização Avançada

### Descrição
Implementar um sistema completo de áudio espacial 3D com HRTF (Head-Related Transfer Function), doppler effect, e integração com Unity's Audio Spatializer.

### Funcionalidades Requeridas

#### 1.1 HRTF e Binaural Audio
- **Unity Audio Spatializer**: Integração com plugin nativo ou terceiros
- **HRTF Profile**: Perfis de ouvido customizáveis
- **Headphone Detection**: Detectar quando jogador usa fones
- **Binaural Rendering**: Som posicional realista em 360°

#### 1.2 Distance Attenuation Avançado
- **Curvas de Atenuação Customizáveis**: 
  - Linear
  - Logarítmica
  - Custom AnimationCurve
- **Min/Max Distance** por tipo de som
- **Rolloff Modes**: Natural, Linear, Custom
- **Spread**: Controle de direcionalidade (som onidirecional vs direcional)

#### 1.3 Doppler Effect
- **Velocity-based Pitch Shift**: Sons mudam de tom com movimento
- **Customização por Tipo**: Veículos têm doppler forte, outros sons fraco
- **Doppler Level**: Controle de intensidade do efeito

#### 1.4 Audio Source Directionality
- **Cone de Emissão**: Sons direcionais (ex: alto-falante, sirene)
- **Inner/Outer Angle**: Controle de ângulo de emissão
- **Volume Falloff**: Atenuação fora do cone

### Implementação Técnica

```csharp
[System.Serializable]
public class Advanced3DAudioSettings
{
    [Header("Spatializer")]
    public bool useSpatializer = true;
    public AudioSpatializerExtensionDefinition spatializer;
    
    [Header("Distance")]
    public float minDistance = 1f;
    public float maxDistance = 500f;
    public AudioRolloffMode rolloffMode = AudioRolloffMode.Logarithmic;
    public AnimationCurve customRolloffCurve;
    
    [Header("Doppler")]
    public bool enableDoppler = true;
    public float dopplerLevel = 1f;
    
    [Header("Directionality")]
    public bool isDirectional = false;
    public float innerAngle = 45f;
    public float outerAngle = 90f;
    public float directionalFalloff = 1f;
    
    [Header("Spread")]
    [Range(0, 360)]
    public float spread = 0f;
}

public class Audio3DEmitter : MonoBehaviour
{
    public Advanced3DAudioSettings settings;
    private AudioSource audioSource;
    
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        ApplySettings();
    }
    
    void ApplySettings()
    {
        audioSource.spatialBlend = 1f; // Full 3D
        audioSource.minDistance = settings.minDistance;
        audioSource.maxDistance = settings.maxDistance;
        audioSource.rolloffMode = settings.rolloffMode;
        
        if (settings.rolloffMode == AudioRolloffMode.Custom)
        {
            audioSource.SetCustomCurve(AudioSourceCurveType.CustomRolloff, 
                                       settings.customRolloffCurve);
        }
        
        audioSource.dopplerLevel = settings.enableDoppler ? settings.dopplerLevel : 0f;
        audioSource.spread = settings.spread;
        
        // Spatializer
        audioSource.spatialize = settings.useSpatializer;
    }
    
    // Método para sons direcionais
    public void SetupDirectionalSound(Vector3 direction)
    {
        // Implementação de som direcional
    }
}
```

### Plugins Recomendados
- **Steam Audio**: Grátis, excelente para HRTF e oclusão
- **Oculus Audio SDK**: Para VR
- **Google Resonance Audio**: Cross-platform
- **Microsoft Spatial Sound**: Para Windows

### Critérios de Aceitação
- [ ] HRTF funcionando com fones de ouvido
- [ ] Doppler effect funcional em objetos em movimento
- [ ] Curvas de atenuação customizáveis
- [ ] Sons direcionais funcionando corretamente
- [ ] Performance otimizada (60 FPS com 50+ sons 3D)
- [ ] Testes A/B com jogadores confirmando melhoria na imersão

### Prioridade
**Baixa** - Feature de polish, não essencial para gameplay

### Dependências
- Nível 2 completo
- Plugin de spatializer instalado
- Hardware de teste adequado (fones de ouvido de qualidade)

### Estimativa
2-3 semanas de desenvolvimento + 1 semana de testes e ajustes

---

## Issue 2: Sistema de Oclusão e Obstruction de Áudio

### Descrição
Implementar detecção de obstáculos entre fonte de som e ouvinte, aplicando filtros de áudio quando há paredes, portas ou outros objetos bloqueando o som.

### Funcionalidades Requeridas

#### 2.1 Raycasting para Detecção de Obstáculos
- **Line-of-Sight Check**: Verificar se há linha direta entre ouvinte e fonte
- **Multi-point Raycasting**: Usar múltiplos raios para detectar obstrução parcial
- **Update Frequency**: Otimizar frequência de checks (não todo frame)

#### 2.2 Material-based Filtering
- **Audio Material System**: Diferentes materiais absorvem sons diferentemente
  - Concreto: Absorve médios e agudos
  - Madeira: Absorve principalmente agudos
  - Metal: Reflete mais, absorve menos
  - Vidro: Transmite bem, mas abafa levemente
- **Transmission Loss**: Perda de volume baseada em material

#### 2.3 Filtros de Áudio Dinâmicos
- **Lowpass Filter**: Aplicar quando som está obstruído (sons abafados)
- **Highpass Filter**: Para efeitos especiais
- **Reverb Zones**: Aplicar reverb baseado no ambiente
- **Smooth Transition**: Transição suave ao entrar/sair de oclusão

#### 2.4 Portal System
- **Audio Portals**: Portas e janelas que permitem som passar
- **Portal States**: Aberta, fechada, parcialmente aberta
- **Room-to-Room Propagation**: Som se propaga por portas conectadas

### Implementação Técnica

```csharp
public enum AudioMaterialType
{
    Concrete,
    Wood,
    Metal,
    Glass,
    Fabric,
    Air
}

[System.Serializable]
public class AudioMaterial
{
    public AudioMaterialType type;
    public float transmissionLoss = 0.5f; // 0 = transmite tudo, 1 = bloqueia tudo
    public float lowpassCutoff = 1000f; // Hz
    public float reverbAmount = 0.1f;
}

public class AudioOcclusionSystem : MonoBehaviour
{
    [Header("Occlusion Settings")]
    public LayerMask occlusionLayers;
    public int raycastCount = 5; // Número de raios para detecção
    public float updateInterval = 0.1f; // Atualizar a cada 0.1s
    
    [Header("Filters")]
    public AudioLowPassFilter lowPassFilter;
    public AnimationCurve occlusionCurve;
    
    private Transform listener;
    private AudioSource audioSource;
    private float lastUpdateTime;
    private float currentOcclusion = 0f; // 0 = sem oclusão, 1 = totalmente ocluído
    
    void Start()
    {
        listener = Camera.main.transform;
        audioSource = GetComponent<AudioSource>();
        lowPassFilter = gameObject.AddComponent<AudioLowPassFilter>();
    }
    
    void Update()
    {
        if (Time.time - lastUpdateTime >= updateInterval)
        {
            UpdateOcclusion();
            lastUpdateTime = Time.time;
        }
        
        ApplyOcclusionEffects();
    }
    
    void UpdateOcclusion()
    {
        int hitsCount = 0;
        Vector3 listenerPos = listener.position;
        Vector3 sourcePos = transform.position;
        
        // Raycast central
        if (Physics.Linecast(sourcePos, listenerPos, out RaycastHit hit, occlusionLayers))
        {
            AudioMaterial material = hit.collider.GetComponent<AudioMaterialComponent>()?.material;
            if (material != null)
            {
                hitsCount++;
            }
        }
        
        // Raycasts adicionais em um padrão circular
        for (int i = 0; i < raycastCount - 1; i++)
        {
            float angle = (360f / (raycastCount - 1)) * i;
            Vector3 direction = Quaternion.Euler(0, angle, 0) * Vector3.forward * 0.5f;
            Vector3 offset = sourcePos + direction;
            
            if (Physics.Linecast(offset, listenerPos, out hit, occlusionLayers))
            {
                hitsCount++;
            }
        }
        
        // Calcular oclusão (0-1)
        currentOcclusion = (float)hitsCount / raycastCount;
    }
    
    void ApplyOcclusionEffects()
    {
        // Aplicar lowpass filter baseado em oclusão
        float targetCutoff = Mathf.Lerp(22000f, 500f, currentOcclusion);
        lowPassFilter.cutoffFrequency = Mathf.Lerp(lowPassFilter.cutoffFrequency, 
                                                     targetCutoff, Time.deltaTime * 5f);
        
        // Reduzir volume
        float volumeMultiplier = occlusionCurve.Evaluate(currentOcclusion);
        audioSource.volume = volumeMultiplier;
    }
}

public class AudioMaterialComponent : MonoBehaviour
{
    public AudioMaterial material;
}

public class AudioPortal : MonoBehaviour
{
    public AudioOcclusionRoom roomA;
    public AudioOcclusionRoom roomB;
    public bool isOpen = true;
    
    [Range(0, 1)]
    public float openAmount = 1f; // 0 = fechada, 1 = totalmente aberta
    
    public void SetOpen(bool open)
    {
        isOpen = open;
        openAmount = open ? 1f : 0f;
    }
    
    public float GetTransmissionMultiplier()
    {
        return isOpen ? openAmount : 0.1f; // Porta fechada ainda transmite um pouco
    }
}
```

### Otimizações de Performance
- **Spatial Hashing**: Agrupar AudioSources próximas
- **LOD System**: Reduzir qualidade de oclusão para sons distantes
- **Priority-based Updates**: Sons importantes checam oclusão com mais frequência
- **Cached Results**: Cachear resultados de raycasts por alguns frames

### Critérios de Aceitação
- [ ] Oclusão funcional com lowpass filter aplicado
- [ ] Pelo menos 3 tipos de materiais de áudio implementados
- [ ] Portais funcionando (som passa por portas abertas)
- [ ] Transições suaves ao entrar/sair de oclusão
- [ ] Performance otimizada (máximo 5% CPU usage para oclusão)
- [ ] Testes com mapas complexos sem drops de FPS

### Prioridade
**Baixa** - Feature de imersão avançada

### Dependências
- Issue 1 (Áudio 3D) deve estar completa
- Sistema de física bem otimizado
- Objetos do cenário com colliders configurados

### Estimativa
3-4 semanas de desenvolvimento + 2 semanas de otimização

---

## Issue 3: Sistema de Prioridade e Culling de Sons

### Descrição
Implementar um sistema inteligente que gerencia quais sons devem tocar quando há muitos AudioSources ativos, priorizando sons importantes e desligando sons menos relevantes.

### Funcionalidades Requeridas

#### 3.1 Sistema de Prioridade
- **Priority Levels**: 
  - Critical (0-63): Diálogos, alarmes, eventos importantes
  - High (64-127): Interações do jogador, UI importante
  - Medium (128-191): SFX de ambiente, passos
  - Low (192-255): Sons ambientes distantes, detalhes
- **Dynamic Priority**: Prioridade muda baseada em distância e contexto
- **Priority Override**: Eventos importantes podem forçar prioridade

#### 3.2 Audio Source Culling
- **Max Active Sounds**: Limite de AudioSources simultâneas (ex: 32)
- **Distance-based Culling**: Sons muito distantes não tocam
- **Audibility Check**: Verificar se som seria audível antes de tocar
- **Fade Out on Cull**: Fade suave ao desligar sons

#### 3.3 Voice Stealing
- **Steal Lowest Priority**: Quando atingir limite, roubar slot de som menos importante
- **Smart Stealing**: Considerar volume, distância e prioridade
- **Prevent Stealing**: Sons críticos não podem ser roubados

#### 3.4 Performance Monitoring
- **Audio Budget**: Monitorar CPU/memória usado por áudio
- **Metrics Dashboard**: Visualizar quantos sons estão ativos, quais foram culled
- **Warnings**: Alertar quando sistema está sobrecarregado

### Implementação Técnica

```csharp
public enum AudioPriority
{
    Critical = 0,
    High = 64,
    Medium = 128,
    Low = 192
}

[System.Serializable]
public class AudioSourceInfo
{
    public AudioSource source;
    public int priority;
    public float distance;
    public float volume;
    public bool isStealable = true;
    public float audibility; // Calculado: volume * (1 - distance/maxDistance)
    
    public void UpdateAudibility(Vector3 listenerPosition)
    {
        distance = Vector3.Distance(source.transform.position, listenerPosition);
        audibility = source.volume * (1f - Mathf.Clamp01(distance / source.maxDistance));
    }
}

public class AudioPrioritySystem : MonoBehaviour
{
    public static AudioPrioritySystem Instance { get; private set; }
    
    [Header("Culling Settings")]
    public int maxActiveSounds = 32;
    public float minAudibility = 0.01f; // Sons abaixo disso são culled
    public float cullCheckInterval = 0.5f;
    
    [Header("Performance")]
    public bool enablePerformanceMonitoring = true;
    public int targetActiveSounds = 24; // Ideal para performance
    
    private List<AudioSourceInfo> activeSounds = new List<AudioSourceInfo>();
    private List<AudioSourceInfo> cullQueue = new List<AudioSourceInfo>();
    private Transform listener;
    private float lastCullCheck;
    
    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    
    void Start()
    {
        listener = Camera.main.transform;
    }
    
    void Update()
    {
        if (Time.time - lastCullCheck >= cullCheckInterval)
        {
            PerformCullingCheck();
            lastCullCheck = Time.time;
        }
    }
    
    public static bool RequestPlaySound(AudioSource source, int priority, bool preventStealing = false)
    {
        if (Instance == null) return false;
        return Instance.TryPlaySound(source, priority, preventStealing);
    }
    
    bool TryPlaySound(AudioSource source, int priority, bool preventStealing)
    {
        // Se estamos abaixo do limite, tocar direto
        if (activeSounds.Count < maxActiveSounds)
        {
            RegisterActiveSound(source, priority, preventStealing);
            source.Play();
            return true;
        }
        
        // Tentar roubar slot de som menos importante
        AudioSourceInfo candidateToSteal = FindStealCandidate(priority);
        
        if (candidateToSteal != null)
        {
            StealVoice(candidateToSteal, source, priority, preventStealing);
            return true;
        }
        
        return false; // Não conseguiu tocar
    }
    
    void RegisterActiveSound(AudioSource source, int priority, bool preventStealing)
    {
        var info = new AudioSourceInfo
        {
            source = source,
            priority = priority,
            isStealable = !preventStealing
        };
        
        activeSounds.Add(info);
    }
    
    AudioSourceInfo FindStealCandidate(int newSoundPriority)
    {
        AudioSourceInfo bestCandidate = null;
        float lowestAudibility = float.MaxValue;
        
        foreach (var sound in activeSounds)
        {
            if (!sound.isStealable) continue;
            if (sound.priority < newSoundPriority) continue; // Só pode roubar de prioridade menor
            
            sound.UpdateAudibility(listener.position);
            
            if (sound.audibility < lowestAudibility)
            {
                lowestAudibility = sound.audibility;
                bestCandidate = sound;
            }
        }
        
        return bestCandidate;
    }
    
    void StealVoice(AudioSourceInfo victim, AudioSource newSource, int priority, bool preventStealing)
    {
        // Fade out rápido
        StartCoroutine(FadeOutAndStop(victim.source, 0.1f));
        
        activeSounds.Remove(victim);
        RegisterActiveSound(newSource, priority, preventStealing);
        newSource.Play();
        
        if (enablePerformanceMonitoring)
        {
            Debug.Log($"[AudioPriority] Voice stolen. Priority: {victim.priority} -> {priority}");
        }
    }
    
    void PerformCullingCheck()
    {
        cullQueue.Clear();
        
        foreach (var sound in activeSounds)
        {
            if (sound.source == null || !sound.source.isPlaying)
            {
                cullQueue.Add(sound);
                continue;
            }
            
            sound.UpdateAudibility(listener.position);
            
            if (sound.audibility < minAudibility && sound.isStealable)
            {
                cullQueue.Add(sound);
            }
        }
        
        // Remover sons culled
        foreach (var sound in cullQueue)
        {
            if (sound.source != null && sound.source.isPlaying)
            {
                StartCoroutine(FadeOutAndStop(sound.source, 0.5f));
            }
            activeSounds.Remove(sound);
        }
    }
    
    System.Collections.IEnumerator FadeOutAndStop(AudioSource source, float duration)
    {
        float startVolume = source.volume;
        float elapsed = 0f;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            source.volume = Mathf.Lerp(startVolume, 0f, elapsed / duration);
            yield return null;
        }
        
        source.Stop();
        source.volume = startVolume;
    }
    
    // Debug/Monitoring
    void OnGUI()
    {
        if (!enablePerformanceMonitoring) return;
        
        GUI.Box(new Rect(10, 10, 300, 100), "Audio Performance");
        GUI.Label(new Rect(20, 30, 280, 20), $"Active Sounds: {activeSounds.Count} / {maxActiveSounds}");
        GUI.Label(new Rect(20, 50, 280, 20), $"Target: {targetActiveSounds}");
        
        int critical = activeSounds.Count(s => s.priority < 64);
        int high = activeSounds.Count(s => s.priority >= 64 && s.priority < 128);
        int medium = activeSounds.Count(s => s.priority >= 128 && s.priority < 192);
        int low = activeSounds.Count(s => s.priority >= 192);
        
        GUI.Label(new Rect(20, 70, 280, 20), $"Critical: {critical} | High: {high} | Med: {medium} | Low: {low}");
    }
}
```

### Integração com AudioManager

```csharp
// Modificar PlaySound no AudioManager.cs:
void PlaySound(string clipName, string channelName, Dictionary<string, AudioClip> clipDict, 
               float spatialBlend = 0f, Vector3 position = default, bool loop = false,
               AudioPriority priority = AudioPriority.Medium, bool preventStealing = false)
{
    // ... código existente ...
    
    // Integrar com sistema de prioridade
    bool canPlay = AudioPrioritySystem.RequestPlaySound(source, (int)priority, preventStealing);
    
    if (!canPlay)
    {
        Debug.LogWarning($"[AudioManager] Could not play {clipName} - no voice slots available");
        ReturnAudioSource(channelName, source);
        return;
    }
    
    // ... resto do código ...
}
```

### Critérios de Aceitação
- [ ] Sistema de prioridade funcional
- [ ] Culling automático de sons inaudíveis
- [ ] Voice stealing funcionando corretamente
- [ ] Limite de 32 AudioSources simultâneas respeitado
- [ ] Dashboard de debug mostrando métricas
- [ ] Sons críticos nunca são interrompidos
- [ ] Performance excelente mesmo com 100+ tentativas de tocar sons

### Prioridade
**Média** - Importante para performance em cenas complexas

### Dependências
- Nível 2 completo
- Sistema de áudio 3D (Issue 1)

### Estimativa
2-3 semanas de desenvolvimento + 1 semana de testes

---

## Issue 4: Performance Tuning e Otimização de Memória

### Descrição
Otimizar o sistema de áudio para usar mínima CPU e memória, incluindo streaming de áudio, compressão, e gerenciamento eficiente de recursos.

### Funcionalidades Requeridas

#### 4.1 Audio Streaming
- **Streaming de Música**: Não carregar música inteira na memória
- **Adaptive Streaming**: Ajustar qualidade baseado em performance
- **Background Loading**: Carregar próximos sons em background

#### 4.2 Compressão e Formatos
- **Vorbis**: Para música (melhor compressão)
- **ADPCM**: Para SFX curtos (balance entre qualidade e size)
- **PCM**: Para sons críticos/curtos (sem compressão)
- **Streaming Assets**: Música não compactada no build

#### 4.3 Audio Memory Management
- **Unload Unused Clips**: Descarregar clips não usados há tempo
- **Preload Critical Sounds**: UI sounds sempre na memória
- **Memory Budget**: Limite de memória para áudio (ex: 100MB)
- **Pooling Optimization**: Otimizar pool de AudioSources

#### 4.4 CPU Optimization
- **Reduce DSP Load**: Limitar número de efeitos ativos
- **Spatializer LOD**: Reduzir qualidade de espacialização para sons distantes
- **Update Rate Scaling**: Reduzir update rate de sistemas não críticos
- **Multithreading**: Usar jobs system para cálculos pesados

#### 4.5 Platform-Specific Optimizations
- **Mobile**: Reduzir qualidade, limitar AudioSources
- **WebGL**: Cuidado com limitações de áudio do browser
- **Console**: Aproveitar hardware de áudio dedicado
- **PC**: Configurações gráficas também afetam áudio

### Implementação Técnica

```csharp
[System.Serializable]
public class AudioPerformanceSettings
{
    [Header("Memory")]
    public int maxAudioMemoryMB = 100;
    public bool enableAudioStreaming = true;
    public bool unloadUnusedClips = true;
    public float unloadDelay = 60f; // Segundos sem uso
    
    [Header("CPU")]
    public int maxDSPEffects = 16;
    public bool useSpatializerLOD = true;
    public float updateRateScale = 1f; // 1 = normal, 0.5 = metade
    
    [Header("Quality")]
    public AudioCompressionFormat musicCompression = AudioCompressionFormat.Vorbis;
    public AudioCompressionFormat sfxCompression = AudioCompressionFormat.ADPCM;
    public int audioQualityLevel = 2; // 0 = low, 1 = med, 2 = high
    
    [Header("Platform")]
    public RuntimePlatform targetPlatform;
    public bool autoDetectPlatform = true;
}

public class AudioPerformanceManager : MonoBehaviour
{
    public AudioPerformanceSettings settings;
    
    private Dictionary<AudioClip, float> clipLastUsed = new Dictionary<AudioClip, float>();
    private float currentMemoryUsageMB = 0f;
    
    void Start()
    {
        if (settings.autoDetectPlatform)
        {
            ApplyPlatformSettings();
        }
        
        InvokeRepeating(nameof(CheckMemoryUsage), 5f, 5f);
        InvokeRepeating(nameof(UnloadUnusedClips), 30f, 30f);
    }
    
    void ApplyPlatformSettings()
    {
        RuntimePlatform platform = Application.platform;
        
        switch (platform)
        {
            case RuntimePlatform.Android:
            case RuntimePlatform.IPhonePlayer:
                // Mobile: Configurações conservadoras
                settings.maxAudioMemoryMB = 50;
                settings.maxDSPEffects = 8;
                settings.audioQualityLevel = 1;
                AudioManager.Instance.audioSourcePoolSize = 16;
                break;
                
            case RuntimePlatform.WebGLPlayer:
                // WebGL: Muito limitado
                settings.maxAudioMemoryMB = 30;
                settings.maxDSPEffects = 4;
                settings.enableAudioStreaming = false; // WebGL não suporta bem
                break;
                
            case RuntimePlatform.PS4:
            case RuntimePlatform.PS5:
            case RuntimePlatform.XboxOne:
            case RuntimePlatform.Switch:
                // Console: Boas capacidades
                settings.maxAudioMemoryMB = 150;
                settings.maxDSPEffects = 32;
                settings.audioQualityLevel = 2;
                break;
                
            default: // PC
                settings.maxAudioMemoryMB = 200;
                settings.maxDSPEffects = 32;
                settings.audioQualityLevel = 2;
                break;
        }
        
        Debug.Log($"[AudioPerformance] Applied settings for {platform}");
    }
    
    void CheckMemoryUsage()
    {
        currentMemoryUsageMB = CalculateAudioMemoryUsage();
        
        if (currentMemoryUsageMB > settings.maxAudioMemoryMB)
        {
            Debug.LogWarning($"[AudioPerformance] Audio memory over budget: {currentMemoryUsageMB}MB / {settings.maxAudioMemoryMB}MB");
            ForceUnloadOldestClips();
        }
    }
    
    float CalculateAudioMemoryUsage()
    {
        // Estimar uso de memória de todos os AudioClips carregados
        float totalBytes = 0f;
        
        AudioClip[] clips = Resources.FindObjectsOfTypeAll<AudioClip>();
        foreach (var clip in clips)
        {
            if (clip.loadState == AudioDataLoadState.Loaded)
            {
                // Estimativa: samples * channels * bytes per sample
                totalBytes += clip.samples * clip.channels * 2; // 16-bit = 2 bytes
            }
        }
        
        return totalBytes / (1024f * 1024f); // Bytes para MB
    }
    
    void UnloadUnusedClips()
    {
        if (!settings.unloadUnusedClips) return;
        
        float currentTime = Time.time;
        List<AudioClip> toUnload = new List<AudioClip>();
        
        foreach (var kvp in clipLastUsed)
        {
            if (currentTime - kvp.Value > settings.unloadDelay)
            {
                toUnload.Add(kvp.Key);
            }
        }
        
        foreach (var clip in toUnload)
        {
            if (clip != null && clip.loadState == AudioDataLoadState.Loaded)
            {
                clip.UnloadAudioData();
                clipLastUsed.Remove(clip);
                Debug.Log($"[AudioPerformance] Unloaded unused clip: {clip.name}");
            }
        }
    }
    
    void ForceUnloadOldestClips()
    {
        var sortedClips = clipLastUsed.OrderBy(kvp => kvp.Value).ToList();
        int unloadCount = Mathf.CeilToInt(sortedClips.Count * 0.2f); // Unload 20%
        
        for (int i = 0; i < unloadCount && i < sortedClips.Count; i++)
        {
            AudioClip clip = sortedClips[i].Key;
            if (clip != null)
            {
                clip.UnloadAudioData();
                clipLastUsed.Remove(clip);
            }
        }
    }
    
    public void RegisterClipUsage(AudioClip clip)
    {
        clipLastUsed[clip] = Time.time;
    }
    
    // Profiling
    void OnGUI()
    {
        GUI.Box(new Rect(10, 120, 300, 80), "Audio Performance");
        GUI.Label(new Rect(20, 140, 280, 20), $"Memory: {currentMemoryUsageMB:F1}MB / {settings.maxAudioMemoryMB}MB");
        GUI.Label(new Rect(20, 160, 280, 20), $"Loaded Clips: {clipLastUsed.Count}");
        GUI.Label(new Rect(20, 180, 280, 20), $"Quality Level: {settings.audioQualityLevel}");
    }
}
```

### Tools e Profiling
- **Unity Profiler**: Audio module
- **Memory Profiler**: Monitorar AudioClips
- **Custom Tools**: Dashboard in-game para métricas

### Critérios de Aceitação
- [ ] Sistema de streaming de música funcionando
- [ ] Memória de áudio abaixo de budget definido
- [ ] Unload automático de clips não usados
- [ ] Configurações específicas por plataforma
- [ ] CPU usage de áudio < 5% em cenas complexas
- [ ] Testes de stress (1000+ play requests) sem crashes
- [ ] Profiling detalhado documentado

### Prioridade
**Alta** - Essencial para release em múltiplas plataformas

### Dependências
- Todos os outros sistemas implementados
- Ferramentas de profiling configuradas

### Estimativa
2-3 semanas de otimização + 2 semanas de testes em plataformas

---

## 📋 Resumo do Nível 3

### Ordem de Implementação Recomendada
1. Issue 3: Sistema de Prioridade → Base para performance
2. Issue 4: Performance Tuning → Otimizar tudo
3. Issue 1: Áudio 3D Real → Feature de imersão
4. Issue 2: Oclusão → Feature avançada (pode ser última)

### Recursos Necessários
- **Time Especializado**: Audio programmers com experiência em otimização
- **Hardware de Teste**: Múltiplas plataformas (PC, Mobile, Console)
- **Plugins**: Steam Audio, Resonance Audio ou similar
- **Tempo Total Estimado**: 2-3 meses de desenvolvimento

### Testes Necessários
- Testes de performance em todas as plataformas alvo
- Stress tests com 100+ AudioSources simultâneas
- Memory leak tests (rodar jogo por horas)
- A/B testing com jogadores para verificar imersão
- Compatibility testing (diferentes sistemas operacionais)

---

## ⚠️ AVISOS IMPORTANTES

### Complexidade
- Estas features são **extremamente complexas**
- Requerem **conhecimento profundo** de:
  - Unity Audio System
  - DSP (Digital Signal Processing)
  - Otimização de performance
  - Raycasting e física
  - Matemática 3D
  
### Quando Implementar
- **Apenas** após Nível 2 estar 100% completo e testado
- Quando o jogo estiver **quase pronto** para release
- Quando houver **budget e tempo** adequados
- Quando **performance** for um problema conhecido

### Alternativas
- Usar plugins existentes (Steam Audio) ao invés de implementar do zero
- Considerar se todas essas features são realmente necessárias
- Focar em polish de gameplay ao invés de features de áudio avançadas

---

## 🔗 Referências Avançadas

### Papers e Artigos
- "Real-Time Sound Synthesis for Interactive Applications" (Rocchesso)
- "Spatial Audio in Games" (GDC Talks)
- "Audio Occlusion and Obstruction in Games" (Gamasutra)

### Plugins Recomendados
- **Steam Audio**: Oclusão, reverb, HRTF (FREE!)
- **FMOD**: Audio middleware profissional
- **Wwise**: Usado em jogos AAA
- **Resonance Audio**: Google, cross-platform

### Cursos
- Unity Learn: Advanced Audio
- Coursera: Audio Signal Processing for Music Applications
- Udemy: Game Audio Implementation

---

## 📊 Métricas de Sucesso

Para considerar o Nível 3 completo, deve atingir:

- ✅ **Performance**: < 5% CPU para áudio em cenas complexas
- ✅ **Memória**: < 100MB de áudio em mobile, < 200MB em PC
- ✅ **Imersão**: 80%+ dos testadores notam melhoria de áudio
- ✅ **Estabilidade**: Zero crashes relacionados a áudio em 100h de testes
- ✅ **Escalabilidade**: Suporta 32+ AudioSources simultâneas sem degradação

---

**Última Atualização**: 2025-12-24  
**Status**: 📋 Planejamento de Longo Prazo  
**Versão do Documento**: 1.0  
**Complexidade**: ⭐⭐⭐⭐⭐ (Muito Alta)
